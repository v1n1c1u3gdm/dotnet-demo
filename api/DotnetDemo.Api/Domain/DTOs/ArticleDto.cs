using System;
using System.Collections.Generic;

namespace DotnetDemo.Domain.DTOs;

public record ArticleDto(
    Guid Id,
    string Title,
    string Slug,
    string PublishedLabel,
    string PostEntry,
    IReadOnlyList<string> Tags,
    Guid AuthorId,
    DateTime CreatedAt,
    DateTime UpdatedAt);

