using DotnetDemo.Configuration;
using DotnetDemo.Migrations;
using DotnetDemo.Persistence;
using DotnetDemo.Seeding;
using FluentMigrator.Runner;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NHibernate;

namespace DotnetDemo.Extensions;

public static class ServiceCollectionPersistenceExtensions
{
    public static IServiceCollection AddDatabasePersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddOptions<DatabaseOptions>()
            .Bind(configuration.GetSection(DatabaseOptions.SectionName))
            .ValidateDataAnnotations();

        services
            .AddOptions<SeedOptions>()
            .Bind(configuration.GetSection(SeedOptions.SectionName));

        services
            .AddOptions<SeedAdminOptions>()
            .Bind(configuration.GetSection(SeedAdminOptions.SectionName));

        services.AddSingleton<NHibernateSessionFactoryBuilder>();
        services.AddSingleton(provider =>
        {
            var builder = provider.GetRequiredService<NHibernateSessionFactoryBuilder>();
            return builder.Build();
        });

        services.AddScoped(provider =>
        {
            var factory = provider.GetRequiredService<ISessionFactory>();
            return factory.OpenSession();
        });

        var databaseOptionsSnapshot = configuration.GetSection(DatabaseOptions.SectionName)
            .Get<DatabaseOptions>() ?? new DatabaseOptions();

        services.AddSingleton<DataSeeder>();

        return services;
    }
}

