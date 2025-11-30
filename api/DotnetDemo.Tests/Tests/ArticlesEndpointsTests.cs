using System.Net.Http.Json;
using DotnetDemo.Tests.Infrastructure;
using FluentAssertions;

namespace DotnetDemo.Tests.Tests;

public class ArticlesEndpointsTests : IntegrationTestBase
{
    public ArticlesEndpointsTests(ApiWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task ListArticles_ReturnsSeededPayload()
    {
        var response = await Client.GetAsync("/articles");

        response.EnsureSuccessStatusCode();
        var payload = await ReadAsAsync<List<ArticleResponse>>(response);

        payload.Should().NotBeNull();
        payload!.Should().NotBeEmpty();
        payload!.First().Slug.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task CreateArticle_PersistsAndReturnsResource()
    {
        var authorsResponse = await Client.GetAsync("/authors");
        authorsResponse.EnsureSuccessStatusCode();
        var authors = await ReadAsAsync<List<AuthorResponse>>(authorsResponse);
        var authorId = authors!.First().Id;

        var request = new
        {
            title = "Novo artigo .NET",
            slug = $"novo-artigo-{Guid.NewGuid():N}",
            publishedLabel = "Hoje",
            postEntry = "Conteúdo gerado em teste.",
            tags = new[] { "dotnet", "test" },
            authorId
        };

        var createResponse = await Client.PostAsJsonAsync("/articles", request);
        createResponse.EnsureSuccessStatusCode();
        var created = await ReadAsAsync<ArticleResponse>(createResponse);

        created.Should().NotBeNull();
        created!.Title.Should().Be("Novo artigo .NET");

        var fetchedResponse = await Client.GetAsync($"/articles/{created.Id}");
        fetchedResponse.EnsureSuccessStatusCode();
        var fetched = await ReadAsAsync<ArticleResponse>(fetchedResponse);

        fetched!.Slug.Should().Be(request.slug);
    }

    [Fact]
    public async Task CountByAuthor_ReturnsAggregatedData()
    {
        var response = await Client.GetAsync("/articles/count_by_author");

        response.EnsureSuccessStatusCode();
        var payload = await ReadAsAsync<List<ArticleCountResponse>>(response);

        payload.Should().NotBeNull();
        payload!.Should().NotBeEmpty();
        payload!.First().ArticlesCount.Should().BeGreaterThanOrEqualTo(0);
    }

    private record ArticleResponse(Guid Id, string Title, string Slug, Guid AuthorId);

    private record AuthorResponse(Guid Id, string Name);

    private record ArticleCountResponse(Guid AuthorId, string AuthorName, int ArticlesCount);
}

