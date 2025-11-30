using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DotnetDemo.Configuration;
using DotnetDemo.Domain.Entities;
using DotnetDemo.Domain.Requests;
using DotnetDemo.Domain.Responses;
using DotnetDemo.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NHibernate.Linq;
using ISession = NHibernate.ISession;

namespace DotnetDemo.Services;

public class AuthService : IAuthService
{
    private readonly ISession _session;
    private readonly IPasswordHasher _passwordHasher;
    private readonly JwtOptions _jwtOptions;
    private readonly ILogger<AuthService> _logger;
    private readonly SymmetricSecurityKey _signingKey;

    public AuthService(
        ISession session,
        IPasswordHasher passwordHasher,
        IOptions<JwtOptions> jwtOptions,
        ILogger<AuthService> logger)
    {
        _session = session;
        _passwordHasher = passwordHasher;
        _jwtOptions = jwtOptions.Value;
        _logger = logger;

        if (string.IsNullOrWhiteSpace(_jwtOptions.SecretKey))
        {
            throw new InvalidOperationException("Jwt:SecretKey must be configured.");
        }

        _signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SecretKey));
    }

    public async Task<AuthTokenResponse?> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
        {
            return null;
        }

        var normalizedUsername = request.Username.Trim().ToLowerInvariant();
        var adminUser = await _session.Query<AdminUser>()
            .FirstOrDefaultAsync(u => u.Username == normalizedUsername, cancellationToken);

        if (adminUser is null)
        {
            _logger.LogWarning("Login failed for {Username}: user not found", normalizedUsername);
            return null;
        }

        var isValid = _passwordHasher.Verify(
            request.Password,
            adminUser.PasswordSalt,
            adminUser.PasswordHash,
            adminUser.PasswordHashIterations);

        if (!isValid)
        {
            _logger.LogWarning("Login failed for {Username}: invalid credentials", normalizedUsername);
            return null;
        }

        var expiresAtUtc = DateTime.UtcNow.AddMinutes(_jwtOptions.AccessTokenMinutes);
        var token = BuildToken(adminUser, expiresAtUtc);

        adminUser.LastLoginAt = DateTime.UtcNow;
        adminUser.Touch();
        await _session.UpdateAsync(adminUser, cancellationToken);

        return new AuthTokenResponse(token, expiresAtUtc, "Bearer");
    }

    private string BuildToken(AdminUser adminUser, DateTime expiresAtUtc)
    {
        var credentials = new SigningCredentials(_signingKey, SecurityAlgorithms.HmacSha256);
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, adminUser.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, adminUser.Username),
            new Claim(ClaimTypes.NameIdentifier, adminUser.Id.ToString()),
            new Claim(ClaimTypes.Name, adminUser.DisplayName),
            new Claim(ClaimTypes.Role, "admin")
        };

        var token = new JwtSecurityToken(
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: expiresAtUtc,
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

