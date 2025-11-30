using System.Linq;
using DotnetDemo.Domain.DTOs;
using DotnetDemo.Domain.Entities;
using DotnetDemo.Domain.Requests;
using ISession = NHibernate.ISession;
using NHibernate.Linq;

namespace DotnetDemo.Services;

public class AuthorService : IAuthorService
{
    private readonly ISession _session;

    public AuthorService(ISession session)
    {
        _session = session;
    }

    public async Task<IReadOnlyList<AuthorDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var authors = await _session.Query<Author>()
            .FetchMany(a => a.Socials)
            .ToListAsync(cancellationToken);

        return authors
            .GroupBy(author => author.Id)
            .Select(group => group.First())
            .OrderBy(author => author.Name)
            .Select(author => author.ToDto())
            .ToList();
    }

    public async Task<AuthorDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var author = await _session.Query<Author>()
            .FetchMany(a => a.Socials)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

        return author?.ToDto();
    }

    public async Task<AuthorDto> CreateAsync(AuthorRequest request, CancellationToken cancellationToken = default)
    {
        var author = new Author
        {
            Name = request.Name,
            Birthdate = request.Birthdate,
            PhotoUrl = request.PhotoUrl,
            PublicKey = request.PublicKey,
            Bio = request.Bio,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _session.SaveAsync(author, cancellationToken);
        return author.ToDto();
    }

    public async Task<AuthorDto?> UpdateAsync(Guid id, AuthorRequest request, CancellationToken cancellationToken = default)
    {
        var author = await _session.GetAsync<Author>(id, cancellationToken);
        if (author is null)
        {
            return null;
        }

        author.Name = request.Name;
        author.Birthdate = request.Birthdate;
        author.PhotoUrl = request.PhotoUrl;
        author.PublicKey = request.PublicKey;
        author.Bio = request.Bio;
        author.UpdatedAt = DateTime.UtcNow;

        await _session.UpdateAsync(author, cancellationToken);
        return author.ToDto();
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var author = await _session.GetAsync<Author>(id, cancellationToken);
        if (author is null)
        {
            return false;
        }

        await _session.DeleteAsync(author, cancellationToken);
        return true;
    }
}

