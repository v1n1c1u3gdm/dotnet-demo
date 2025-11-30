using System.Linq;
using System.Text.Json;
using DotnetDemo.Domain.Entities;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NHibernate;
using ISession = NHibernate.ISession;
using NHibernate.Linq;

namespace DotnetDemo.Seeding;

public class DataSeeder
{
    private const string DefaultPublicKey = "ssh-ed25519 AAAAC3NzaC1lZDI1NTE5AAAAIFakePublicKeyForViniciusSeedRecord";
    private static readonly TimeZoneInfo SaoPaulo = TimeZoneInfo.FindSystemTimeZoneById("America/Sao_Paulo");

    private readonly ISessionFactory _sessionFactory;
    private readonly SeedOptions _seedOptions;
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<DataSeeder> _logger;

    public DataSeeder(
        ISessionFactory sessionFactory,
        IOptions<SeedOptions> seedOptions,
        IWebHostEnvironment environment,
        ILogger<DataSeeder> logger)
    {
        _sessionFactory = sessionFactory;
        _seedOptions = seedOptions.Value;
        _environment = environment;
        _logger = logger;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        using var session = _sessionFactory.OpenSession();
        using var transaction = session.BeginTransaction();

        var author = await UpsertAuthorAsync(session, cancellationToken);
        await UpsertSocialsAsync(session, author, cancellationToken);
        await UpsertArticlesAsync(session, author, cancellationToken);

        await transaction.CommitAsync(cancellationToken);
        session.Clear();
    }

    private async Task<Author> UpsertAuthorAsync(ISession session, CancellationToken cancellationToken)
    {
        var name = "Vinicius G.D. Menezes";
        var author = await session.Query<Author>()
            .FirstOrDefaultAsync(a => a.Name == name, cancellationToken)
            ?? new Author { Name = name };

        var birthdate = new DateTime(1986, 10, 7);
        var localBirthdate = TimeZoneInfo.ConvertTimeToUtc(birthdate, SaoPaulo);

        author.Birthdate = localBirthdate;
        author.PhotoUrl = "https://viniciusmenezes.com/media/website/IMG_20220624_123059.jpg";
        author.PublicKey = Environment.GetEnvironmentVariable("VINICIUS_PUBLIC_KEY") ?? DefaultPublicKey;
        author.Bio =
            "Filho competente, irmão implicante, pai de menina, apaixonado por motos, namorado fiel e eterno educador. " +
            "Adoro filme, vídeo-game, andar de moto, cerveja de trigo, chocolate e programar. Esse sou eu.<br><br>" +
            "Profissionalmente orientado pela agilidade, unindo tecnologia e liderança para impulsionar equipes em direção à excelência. " +
            "Minha abordagem é enraizada na colaboração e na comunicação transparente, criando um ambiente onde a inovação floresce e cada " +
            "membro da equipe é capacitado a contribuir plenamente. Através da implementação de práticas ágeis, busco otimizar nossos processos, " +
            "maximizar a eficiência e alcançar resultados excepcionais, sempre com foco no crescimento conjunto e no alcance de metas ambiciosas.";

        author.UpdatedAt = DateTime.UtcNow;
        if (author.Id == Guid.Empty)
        {
            author.CreatedAt = DateTime.UtcNow;
        }

        await session.SaveOrUpdateAsync(author, cancellationToken);
        _logger.LogInformation("Seeded author record {AuthorName}", author.Name);
        return author;
    }

    private static IEnumerable<(string slug, string profileLink, string description)> SocialSeeds => new[]
    {
        ("twitter", "https://twitter.com/v1n1c1u5gdm", "Perfil oficial no Twitter / X"),
        ("instagram", "https://www.instagram.com/vm3n3z35/", "Momentos diversos no Instagram"),
        ("linkedin", "https://www.linkedin.com/in/menezesvinicius/", "Perfil profissional no LinkedIn"),
        ("github", "https://github.com/v1n1c1u3gdm", "Portfólio e projetos no GitHub")
    };

    private async Task UpsertSocialsAsync(ISession session, Author author, CancellationToken cancellationToken)
    {
        foreach (var seed in SocialSeeds)
        {
            var social = await session.Query<Social>()
                .FirstOrDefaultAsync(s => s.Slug == seed.slug, cancellationToken)
                ?? new Social { Slug = seed.slug };

            social.ProfileLink = seed.profileLink;
            social.Description = seed.description;
            social.Author = author;
            social.AuthorId = author.Id;
            social.UpdatedAt = DateTime.UtcNow;
            if (social.Id == Guid.Empty)
            {
                social.CreatedAt = DateTime.UtcNow;
            }

            await session.SaveOrUpdateAsync(social, cancellationToken);
        }

        _logger.LogInformation("Seeded {Count} social profiles", SocialSeeds.Count());
    }

    private async Task UpsertArticlesAsync(ISession session, Author author, CancellationToken cancellationToken)
    {
        var payload = await LoadArticlesAsync(cancellationToken);
        if (payload is null)
        {
            _logger.LogWarning("Article seed payload is empty");
            return;
        }

        var generatedAt = ParseGeneratedAt(payload) ?? DateTime.UtcNow;

        foreach (var record in payload.Data)
        {
            var article = await session.Query<Article>()
                .FirstOrDefaultAsync(a => a.Slug == record.Slug, cancellationToken)
                ?? new Article { Slug = record.Slug };

            article.Title = record.Title;
            article.PublishedLabel = record.PublishedLabel;
            article.PostEntry = record.PostEntry;
            article.Tags = record.Tags ?? new List<string>();
            article.Author = author;
            article.AuthorId = author.Id;
            article.CreatedAt = article.Id == Guid.Empty ? generatedAt : article.CreatedAt;
            article.UpdatedAt = DateTime.UtcNow;

            await session.SaveOrUpdateAsync(article, cancellationToken);
        }

        _logger.LogInformation("Seeded {Count} articles", payload.Data.Count);
    }

    private async Task<ArticleSeedPayload?> LoadArticlesAsync(CancellationToken cancellationToken)
    {
        var path = ResolveSeedPath(_seedOptions.ArticleSeedPath);
        if (!File.Exists(path))
        {
            _logger.LogWarning("Article seed file {Path} not found", path);
            return null;
        }

        await using var stream = File.OpenRead(path);
        return await JsonSerializer.DeserializeAsync<ArticleSeedPayload>(stream, cancellationToken: cancellationToken);
    }

    private string ResolveSeedPath(string configuredPath)
    {
        if (Path.IsPathRooted(configuredPath))
        {
            return configuredPath;
        }

        var contentRoot = _environment.ContentRootPath;
        return Path.Combine(contentRoot, configuredPath);
    }

    private static DateTime? ParseGeneratedAt(ArticleSeedPayload payload)
    {
        if (DateTime.TryParse(payload.Metadata.GeneratedAt, out var timestamp))
        {
            return timestamp;
        }

        return null;
    }
}

