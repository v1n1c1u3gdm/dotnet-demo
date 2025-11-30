using DotnetDemo.Domain.DTOs;
using DotnetDemo.Domain.Entities;
using DotnetDemo.Domain.Requests;

namespace DotnetDemo.Services;

public interface IArticleService
{
    Task<IReadOnlyList<ArticleDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<ArticleDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ArticleDto> CreateAsync(ArticleRequest request, CancellationToken cancellationToken = default);
    Task<ArticleDto?> UpdateAsync(Guid id, ArticleRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ArticlesCountDto>> CountByAuthorAsync(CancellationToken cancellationToken = default);
}

