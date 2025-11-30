using System;

namespace DotnetDemo.Domain.DTOs;

public record SocialDto(
    Guid Id,
    string ProfileLink,
    string Slug,
    string Description,
    Guid AuthorId,
    DateTime CreatedAt,
    DateTime UpdatedAt);

