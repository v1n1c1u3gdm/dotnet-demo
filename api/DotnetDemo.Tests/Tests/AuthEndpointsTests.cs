using DotnetDemo.Domain.Responses;
using DotnetDemo.Tests.Infrastructure;
using FluentAssertions;

namespace DotnetDemo.Tests.Tests;

public class AuthEndpointsTests : IntegrationTestBase
{
    public AuthEndpointsTests(ApiWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsToken()
    {
        var response = await PostJsonAsync("/auth/login", new
        {
            username = AdminUsername,
            password = AdminPassword
        });

        response.EnsureSuccessStatusCode();
        var payload = await ReadAsAsync<AuthTokenResponse>(response);

        payload.Should().NotBeNull();
        payload!.AccessToken.Should().NotBeNullOrWhiteSpace();
        payload.TokenType.Should().Be("Bearer");
    }

    [Fact]
    public async Task Login_WithInvalidPassword_ReturnsUnauthorized()
    {
        var response = await PostJsonAsync("/auth/login", new
        {
            username = AdminUsername,
            password = "wrong-password"
        });

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);
    }
}

