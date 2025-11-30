using System;

namespace DotnetDemo.Domain.Requests;

public record SocialRequest(
    string ProfileLink,
    string Slug,
    string Description,
    Guid AuthorId);

