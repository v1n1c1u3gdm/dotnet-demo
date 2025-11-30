using DotnetDemo;
using DotnetDemo.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DotnetDemo.Tests.Infrastructure;

public class ApiWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _databasePath;
    private readonly string _logsPath;
    public const string AdminSeedUsername = "test-admin";
    public const string AdminSeedPassword = "test-password";
    public const string AdminSeedDisplayName = "Test Administrator";
    public const string JwtSecret = "integration-tests-secret-key-change-me";

    public ApiWebApplicationFactory()
    {
        _databasePath = Path.Combine(Path.GetTempPath(), $"dotnet-demo-tests-{Guid.NewGuid():N}.db");
        _logsPath = Path.Combine(Path.GetTempPath(), "dotnet-demo-test-logs");
        Directory.CreateDirectory(_logsPath);
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.ConfigureAppConfiguration((context, configBuilder) =>
        {
            var overrides = new Dictionary<string, string?>
            {
                ["Database:Provider"] = "sqlite",
                ["Database:ConnectionString"] = $"Data Source={_databasePath};Cache=Shared;Foreign Keys=True",
                ["Logging:Directory"] = _logsPath,
                ["Seeds:AdminUser:Username"] = AdminSeedUsername,
                ["Seeds:AdminUser:Password"] = AdminSeedPassword,
                ["Seeds:AdminUser:DisplayName"] = AdminSeedDisplayName,
                ["Seeds:AdminUser:HashIterations"] = "80000",
                ["Jwt:Issuer"] = "dotnet-demo-tests",
                ["Jwt:Audience"] = "dotnet-demo-tests-clients",
                ["Jwt:SecretKey"] = JwtSecret,
                ["Jwt:AccessTokenMinutes"] = "30"
            };

            configBuilder.AddInMemoryCollection(overrides!);
        });

        builder.ConfigureServices(services =>
        {
            services.PostConfigure<DatabaseOptions>(options =>
            {
                options.Provider = "sqlite";
                options.ConnectionString = $"Data Source={_databasePath};Cache=Shared;Foreign Keys=True";
            });
        });
    }

    public override async ValueTask DisposeAsync()
    {
        if (File.Exists(_databasePath))
        {
            File.Delete(_databasePath);
        }

        await base.DisposeAsync();
    }
}

