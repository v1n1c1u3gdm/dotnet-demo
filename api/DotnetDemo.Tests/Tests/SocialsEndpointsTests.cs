using DotnetDemo.Tests.Infrastructure;
using FluentAssertions;

namespace DotnetDemo.Tests.Tests;

public class SocialsEndpointsTests : IntegrationTestBase
{
    public SocialsEndpointsTests(ApiWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task GetSocials_ReturnsSeededEntries()
    {
        var response = await Client.GetAsync("/socials");
        response.EnsureSuccessStatusCode();

        var socials = await ReadAsAsync<List<SocialResponse>>(response);
        socials.Should().NotBeNull();
        socials!.Should().NotBeEmpty();
    }

    private record SocialResponse(Guid Id, string Slug);
}

