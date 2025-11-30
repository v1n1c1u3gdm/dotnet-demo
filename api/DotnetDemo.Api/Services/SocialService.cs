using System.Linq;
using DotnetDemo.Domain.DTOs;
using DotnetDemo.Domain.Entities;
using DotnetDemo.Domain.Requests;
using ISession = NHibernate.ISession;
using NHibernate.Linq;

namespace DotnetDemo.Services;

public class SocialService : ISocialService
{
    private readonly ISession _session;

    public SocialService(ISession session)
    {
        _session = session;
    }

    public async Task<IReadOnlyList<SocialDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var socials = await _session.Query<Social>()
            .OrderBy(s => s.Slug)
            .ToListAsync(cancellationToken);

        return socials.Select(s => s.ToDto()).ToList();
    }

    public async Task<SocialDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var social = await _session.GetAsync<Social>(id, cancellationToken);
        return social?.ToDto();
    }

    public async Task<SocialDto> CreateAsync(SocialRequest request, CancellationToken cancellationToken = default)
    {
        var author = await _session.GetAsync<Author>(request.AuthorId, cancellationToken)
            ?? throw new ArgumentException("Author not found", nameof(request.AuthorId));

        var social = new Social
        {
            ProfileLink = request.ProfileLink,
            Slug = request.Slug,
            Description = request.Description,
            Author = author,
            AuthorId = author.Id,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _session.SaveAsync(social, cancellationToken);
        return social.ToDto();
    }

    public async Task<SocialDto?> UpdateAsync(Guid id, SocialRequest request, CancellationToken cancellationToken = default)
    {
        var social = await _session.GetAsync<Social>(id, cancellationToken);
        if (social is null)
        {
            return null;
        }

        if (social.AuthorId != request.AuthorId)
        {
            var author = await _session.GetAsync<Author>(request.AuthorId, cancellationToken)
                ?? throw new ArgumentException("Author not found", nameof(request.AuthorId));
            social.Author = author;
            social.AuthorId = author.Id;
        }

        social.ProfileLink = request.ProfileLink;
        social.Slug = request.Slug;
        social.Description = request.Description;
        social.UpdatedAt = DateTime.UtcNow;

        await _session.UpdateAsync(social, cancellationToken);
        return social.ToDto();
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var social = await _session.GetAsync<Social>(id, cancellationToken);
        if (social is null)
        {
            return false;
        }

        await _session.DeleteAsync(social, cancellationToken);
        return true;
    }
}

