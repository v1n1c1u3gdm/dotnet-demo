using DotnetDemo.Domain.DTOs;
using DotnetDemo.Domain.Requests;

namespace DotnetDemo.Services;

public interface ISocialService
{
    Task<IReadOnlyList<SocialDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<SocialDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<SocialDto> CreateAsync(SocialRequest request, CancellationToken cancellationToken = default);
    Task<SocialDto?> UpdateAsync(Guid id, SocialRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}

