using DotnetDemo.Tests.Infrastructure;
using FluentAssertions;

namespace DotnetDemo.Tests.Tests;

public class HealthAndDiagnosticsTests : IntegrationTestBase
{
    public HealthAndDiagnosticsTests(ApiWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task UpEndpoint_ReturnsOk()
    {
        var response = await Client.GetAsync("/up");
        response.EnsureSuccessStatusCode();

        var payload = await ReadAsAsync<UpResponse>(response);
        payload!.status.Should().Be("ok");
    }

    [Fact]
    public async Task MetricsEndpoint_ReturnsPrometheusDocument()
    {
        var response = await Client.GetAsync("/metrics");
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("http_server_requests_total");
    }

    [Fact]
    public async Task TechEndpoint_ReturnsHtml()
    {
        var response = await Client.GetAsync("/tech");
        response.EnsureSuccessStatusCode();

        response.Content.Headers.ContentType!.MediaType.Should().Be("text/html");
        var html = await response.Content.ReadAsStringAsync();
        html.Should().Contain("/tech — dotnet-demo diagnostics");
    }

    private record UpResponse(string status, string service);
}

