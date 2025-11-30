using System.Text.Json.Serialization;

namespace DotnetDemo.Seeding;

public sealed class ArticleSeedPayload
{
    [JsonPropertyName("metadata")]
    public ArticleSeedMetadata Metadata { get; set; } = new();

    [JsonPropertyName("data")]
    public List<ArticleSeedRecord> Data { get; set; } = new();
}

public sealed class ArticleSeedMetadata
{
    [JsonPropertyName("generated_at")]
    public string? GeneratedAt { get; set; }
}

public sealed class ArticleSeedRecord
{
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("slug")]
    public string Slug { get; set; } = string.Empty;

    [JsonPropertyName("published_label")]
    public string PublishedLabel { get; set; } = string.Empty;

    [JsonPropertyName("post_entry")]
    public string PostEntry { get; set; } = string.Empty;

    [JsonPropertyName("tags")]
    public List<string> Tags { get; set; } = new();
}

