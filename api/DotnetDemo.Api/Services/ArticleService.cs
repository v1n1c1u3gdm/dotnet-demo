using System.Linq;
using DotnetDemo.Domain.DTOs;
using DotnetDemo.Domain.Entities;
using DotnetDemo.Domain.Requests;
using DotnetDemo.Domain.Utils;
using ISession = NHibernate.ISession;
using NHibernate.Linq;

namespace DotnetDemo.Services;

public class ArticleService : IArticleService
{
    private readonly ISession _session;

    public ArticleService(ISession session)
    {
        _session = session;
    }

    public async Task<IReadOnlyList<ArticleDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var articles = await _session.Query<Article>()
            .Fetch(a => a.Author)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync(cancellationToken);

        return articles.Select(article => article.ToDto()).ToList();
    }

    public async Task<ArticleDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var article = await _session.Query<Article>()
            .Fetch(a => a.Author)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

        return article?.ToDto();
    }

    public async Task<ArticleDto> CreateAsync(ArticleRequest request, CancellationToken cancellationToken = default)
    {
        var author = await _session.GetAsync<Author>(request.AuthorId, cancellationToken)
            ?? throw new ArgumentException("Author not found", nameof(request.AuthorId));

        var slug = request.Slug;
        if (string.IsNullOrWhiteSpace(slug))
        {
            slug = await GenerateUniqueSlugAsync(SlugGenerator.Generate(request.Title), cancellationToken);
        }
        else
        {
            slug = await GenerateUniqueSlugAsync(slug.Trim().ToLowerInvariant(), cancellationToken);
        }

        var article = new Article
        {
            Title = request.Title,
            Slug = slug,
            PublishedLabel = request.PublishedLabel,
            PostEntry = request.PostEntry,
            Tags = request.Tags ?? new List<string>(),
            Author = author,
            AuthorId = author.Id,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _session.SaveAsync(article, cancellationToken);
        return article.ToDto();
    }

    public async Task<ArticleDto?> UpdateAsync(Guid id, ArticleRequest request, CancellationToken cancellationToken = default)
    {
        var article = await _session.Query<Article>()
            .Fetch(a => a.Author)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

        if (article is null)
        {
            return null;
        }

        article.Title = request.Title;
        article.PublishedLabel = request.PublishedLabel;
        article.PostEntry = request.PostEntry;
        article.Tags = request.Tags ?? new List<string>();
        article.UpdatedAt = DateTime.UtcNow;

        if (article.AuthorId != request.AuthorId)
        {
            var author = await _session.GetAsync<Author>(request.AuthorId, cancellationToken)
                ?? throw new ArgumentException("Author not found", nameof(request.AuthorId));
            article.Author = author;
            article.AuthorId = author.Id;
        }

        if (!string.IsNullOrWhiteSpace(request.Slug))
        {
            var incomingSlug = request.Slug.Trim().ToLowerInvariant();
            if (!string.Equals(incomingSlug, article.Slug, StringComparison.OrdinalIgnoreCase))
            {
                article.Slug = await GenerateUniqueSlugAsync(incomingSlug, cancellationToken, article.Id);
            }
        }

        await _session.UpdateAsync(article, cancellationToken);
        return article.ToDto();
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var article = await _session.GetAsync<Article>(id, cancellationToken);
        if (article is null)
        {
            return false;
        }

        await _session.DeleteAsync(article, cancellationToken);
        return true;
    }

    public async Task<IReadOnlyList<ArticlesCountDto>> CountByAuthorAsync(CancellationToken cancellationToken = default)
    {
        var authors = await _session.Query<Author>()
            .Select(a => new { a.Id, a.Name })
            .OrderBy(a => a.Name)
            .ToListAsync(cancellationToken);

        var articleCounts = await _session.Query<Article>()
            .GroupBy(a => a.Author.Id)
            .Select(group => new { AuthorId = group.Key, Count = group.Count() })
            .ToListAsync(cancellationToken);

        var lookup = articleCounts.ToDictionary(x => x.AuthorId, x => x.Count);

        return authors
            .Select(a =>
            {
                lookup.TryGetValue(a.Id, out var count);
                return new ArticlesCountDto(a.Id, a.Name, count);
            })
            .ToList();
    }

    private async Task<string> GenerateUniqueSlugAsync(string baseSlug, CancellationToken cancellationToken, Guid? excludeId = null)
    {
        var slug = string.IsNullOrWhiteSpace(baseSlug)
            ? SlugGenerator.Generate(Guid.NewGuid().ToString("N"))
            : SlugGenerator.Generate(baseSlug);

        var suffix = 1;
        var candidate = slug;

        while (await SlugExistsAsync(candidate, cancellationToken, excludeId))
        {
            suffix++;
            candidate = $"{slug}-{suffix}";
        }

        return candidate;
    }

    private Task<bool> SlugExistsAsync(string slug, CancellationToken cancellationToken, Guid? excludeId = null)
    {
        var query = _session.Query<Article>().Where(article => article.Slug == slug);
        if (excludeId.HasValue)
        {
            var value = excludeId.Value;
            query = query.Where(article => article.Id != value);
        }

        return query.AnyAsync(cancellationToken);
    }

}

