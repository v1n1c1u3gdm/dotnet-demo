using System;
using System.Collections.Generic;

namespace DotnetDemo.Domain.DTOs;

public record AuthorDto(
    Guid Id,
    string Name,
    DateTime Birthdate,
    string PhotoUrl,
    string PublicKey,
    string Bio,
    IReadOnlyList<SocialDto> Socials,
    DateTime CreatedAt,
    DateTime UpdatedAt);

