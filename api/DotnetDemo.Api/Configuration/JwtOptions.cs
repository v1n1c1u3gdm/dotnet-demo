namespace DotnetDemo.Configuration;

public class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Issuer { get; set; } = "dotnet-demo";
    public string Audience { get; set; } = "dotnet-demo-ui";
    public string SecretKey { get; set; } = string.Empty;
    public int AccessTokenMinutes { get; set; } = 60;
}

