using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using DotnetDemo.Domain.Responses;

namespace DotnetDemo.Tests.Infrastructure;

public abstract class IntegrationTestBase : IClassFixture<ApiWebApplicationFactory>
{
    private static readonly JsonSerializerOptions SnakeCaseJsonOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        DictionaryKeyPolicy = JsonNamingPolicy.SnakeCaseLower,
        PropertyNameCaseInsensitive = true
    };

    protected IntegrationTestBase(ApiWebApplicationFactory factory)
    {
        Client = factory.CreateClient();
    }

    protected HttpClient Client { get; }
    private string? _cachedToken;
    protected static string AdminUsername => ApiWebApplicationFactory.AdminSeedUsername;
    protected static string AdminPassword => ApiWebApplicationFactory.AdminSeedPassword;

    protected Task<HttpResponseMessage> PostJsonAsync<T>(
        string requestUri,
        T payload,
        CancellationToken cancellationToken = default)
        => Client.PostAsJsonAsync(requestUri, payload, SnakeCaseJsonOptions, cancellationToken);

    protected Task<HttpResponseMessage> PatchJsonAsync<T>(
        string requestUri,
        T payload,
        CancellationToken cancellationToken = default)
    {
        var content = JsonContent.Create(payload, options: SnakeCaseJsonOptions);
        return Client.PatchAsync(requestUri, content, cancellationToken);
    }

    protected static Task<T?> ReadAsAsync<T>(HttpResponseMessage response)
        => response.Content.ReadFromJsonAsync<T>(SnakeCaseJsonOptions);

    protected async Task AuthenticateClientAsync(CancellationToken cancellationToken = default)
    {
        if (!string.IsNullOrWhiteSpace(_cachedToken))
        {
            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _cachedToken);
            return;
        }

        var loginRequest = new
        {
            username = AdminUsername,
            password = AdminPassword
        };

        var response = await PostJsonAsync("/auth/login", loginRequest, cancellationToken);
        response.EnsureSuccessStatusCode();
        var payload = await ReadAsAsync<AuthTokenResponse>(response)
            ?? throw new InvalidOperationException("Auth login returned an empty payload.");

        _cachedToken = payload.AccessToken;
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _cachedToken);
    }
}

