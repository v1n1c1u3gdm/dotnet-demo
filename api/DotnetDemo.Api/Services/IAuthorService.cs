using DotnetDemo.Domain.DTOs;
using DotnetDemo.Domain.Requests;

namespace DotnetDemo.Services;

public interface IAuthorService
{
    Task<IReadOnlyList<AuthorDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<AuthorDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<AuthorDto> CreateAsync(AuthorRequest request, CancellationToken cancellationToken = default);
    Task<AuthorDto?> UpdateAsync(Guid id, AuthorRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}

