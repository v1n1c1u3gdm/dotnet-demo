using DotnetDemo.Domain.Entities;

namespace DotnetDemo.Domain.DTOs;

public static class DtoMapper
{
    public static ArticleDto ToDto(this Article entity)
    {
        return new ArticleDto(
            entity.Id,
            entity.Title,
            entity.Slug,
            entity.PublishedLabel,
            entity.PostEntry,
            entity.Tags.ToList(),
            entity.AuthorId,
            entity.CreatedAt,
            entity.UpdatedAt);
    }

    public static AuthorDto ToDto(this Author entity)
    {
        var socials = entity.Socials?.Select(ToDto).ToList() ?? new List<SocialDto>();

        return new AuthorDto(
            entity.Id,
            entity.Name,
            entity.Birthdate,
            entity.PhotoUrl,
            entity.PublicKey,
            entity.Bio,
            socials,
            entity.CreatedAt,
            entity.UpdatedAt);
    }

    public static SocialDto ToDto(this Social entity)
    {
        return new SocialDto(
            entity.Id,
            entity.ProfileLink,
            entity.Slug,
            entity.Description,
            entity.AuthorId,
            entity.CreatedAt,
            entity.UpdatedAt);
    }

}

