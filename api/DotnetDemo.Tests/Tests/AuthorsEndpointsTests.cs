using System.Net.Http.Json;
using DotnetDemo.Tests.Infrastructure;
using FluentAssertions;

namespace DotnetDemo.Tests.Tests;

public class AuthorsEndpointsTests : IntegrationTestBase
{
    public AuthorsEndpointsTests(ApiWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task GetAuthors_ReturnsSocialsAndArticles()
    {
        var response = await Client.GetAsync("/authors");
        response.EnsureSuccessStatusCode();

        var authors = await ReadAsAsync<List<AuthorResponse>>(response);
        authors.Should().NotBeNull();
        authors!.Should().NotBeEmpty();

        var sample = authors!.First();
        sample.Socials.Should().NotBeNull();
    }

    [Fact]
    public async Task UpdateAuthor_AllowsChangingBio()
    {
        var response = await Client.GetAsync("/authors");
        var authors = await ReadAsAsync<List<AuthorResponse>>(response);
        var authorId = authors!.First().Id;

        var payload = new
        {
            name = authors.First().Name,
            birthdate = authors.First().Birthdate,
            photoUrl = authors.First().PhotoUrl,
            publicKey = authors.First().PublicKey,
            bio = "Bio atualizada via teste."
        };

        var updateResponse = await PatchJsonAsync($"/authors/{authorId}", payload);
        updateResponse.EnsureSuccessStatusCode();

        var updated = await ReadAsAsync<AuthorResponse>(updateResponse);
        updated!.Bio.Should().Contain("atualizada");
    }

    private record AuthorResponse(Guid Id, string Name, DateTime Birthdate, string PhotoUrl, string PublicKey, string Bio, List<SocialResponse> Socials);

    private record SocialResponse(Guid Id, string Slug);
}

