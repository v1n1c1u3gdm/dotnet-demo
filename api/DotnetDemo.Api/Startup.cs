using DotnetDemo.Configuration;
using DotnetDemo.Extensions;
using DotnetDemo.Middleware;
using DotnetDemo.Migrations;
using DotnetDemo.Observability;
using DotnetDemo.Seeding;
using FluentMigrator.Runner;
using OpenTelemetry.Exporter.Prometheus;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Filters;

namespace DotnetDemo;

public class Startup
{
    private readonly IConfiguration _configuration;
    private readonly IWebHostEnvironment _environment;

    public Startup(IConfiguration configuration, IWebHostEnvironment environment)
    {
        _configuration = configuration;
        _environment = environment;
    }

    public void ConfigureLogging(WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog((context, services, loggerConfiguration) =>
        {
            var loggingOptions = context.Configuration
                .GetSection(LoggingOptions.SectionName)
                .Get<LoggingOptions>() ?? new LoggingOptions();

            var logDirectory = Path.IsPathRooted(loggingOptions.Directory)
                ? loggingOptions.Directory
                : Path.Combine(context.HostingEnvironment.ContentRootPath, loggingOptions.Directory);

            Directory.CreateDirectory(logDirectory);

            loggerConfiguration
                .ReadFrom.Configuration(context.Configuration)
                .ReadFrom.Services(services)
                .WriteTo.Console()
                .WriteTo.File(Path.Combine(logDirectory, "app.log"), rollingInterval: RollingInterval.Day, shared: true)
                .WriteTo.Logger(lc => lc
                    .Filter.ByIncludingOnly(Matching.FromSource("DotnetDemo.Persistence.SqlLoggingInterceptor"))
                    .WriteTo.File(Path.Combine(logDirectory, "nhibernate.log"), rollingInterval: RollingInterval.Day, shared: true));
        });
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddApiCore(_configuration);
        services.AddDatabasePersistence(_configuration);
        services.AddObservability(_configuration);
    }

    public async Task ApplyDatabaseMigrationsAsync(IServiceProvider services, CancellationToken cancellationToken = default)
    {
        using var scope = services.CreateScope();
        var databaseOptions = scope.ServiceProvider.GetRequiredService<IOptions<DatabaseOptions>>().Value;

        using var migrationProvider = BuildMigrationServiceProvider(databaseOptions);
        var runner = migrationProvider.GetRequiredService<IMigrationRunner>();
        runner.MigrateUp();

        var seeder = scope.ServiceProvider.GetRequiredService<DataSeeder>();
        await seeder.SeedAsync(cancellationToken);
    }

    private static ServiceProvider BuildMigrationServiceProvider(DatabaseOptions options)
    {
        return new ServiceCollection()
            .AddFluentMigratorCore()
            .ConfigureRunner(builder =>
            {
                if (options.Provider.Equals("sqlite", StringComparison.OrdinalIgnoreCase))
                {
                    builder.AddSQLite().WithGlobalConnectionString(options.ConnectionString);
                }
                else
                {
                    builder.AddMySql5().WithGlobalConnectionString(options.ConnectionString);
                }

                builder.ScanIn(typeof(CreateAuthorsTable).Assembly).For.Migrations();
            })
            .AddLogging(lb => lb.AddFluentMigratorConsole())
            .BuildServiceProvider();
    }

    public void ConfigurePipeline(WebApplication app)
    {
        if (!_environment.IsProduction())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseSerilogRequestLogging();
        app.UseMiddleware<ExceptionHandlingMiddleware>();
        app.UseMiddleware<RequestMetricsMiddleware>();
        app.UseMiddleware<NHibernateSessionMiddleware>();

        app.UseSwagger(c =>
        {
            c.RouteTemplate = "api-docs/{documentName}/swagger.json";
        });

        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/api-docs/v1/swagger.json", "dotnet-demo v1");
            c.RoutePrefix = "api-docs";
        });

        app.UseRouting();
        app.UseCors("default");
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();
        app.MapPrometheusScrapingEndpoint("/metrics");

        app.MapGet("/", async context =>
        {
            context.Response.Redirect("/api-docs");
            await Task.CompletedTask;
        });
    }
}

