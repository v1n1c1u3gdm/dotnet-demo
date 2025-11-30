using System;

namespace DotnetDemo.Domain.DTOs;

public record ArticlesCountDto(Guid AuthorId, string AuthorName, int ArticlesCount);

