using System.Net.Http.Json;
using System.Text.Json;

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
}

