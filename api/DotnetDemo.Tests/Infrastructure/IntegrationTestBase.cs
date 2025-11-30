using System.Net.Http.Json;
using DotnetDemo.Tests.Infrastructure;

namespace DotnetDemo.Tests.Infrastructure;

public abstract class IntegrationTestBase : IClassFixture<ApiWebApplicationFactory>
{
    protected IntegrationTestBase(ApiWebApplicationFactory factory)
    {
        Client = factory.CreateClient();
    }

    protected HttpClient Client { get; }

    protected static async Task<T?> ReadAsAsync<T>(HttpResponseMessage response)
    {
        return await response.Content.ReadFromJsonAsync<T>();
    }
}

