using DotnetDemo.Domain.Requests;
using DotnetDemo.Domain.Responses;

namespace DotnetDemo.Services;

public interface IAuthService
{
    Task<AuthTokenResponse?> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
}

