using DotnetDemo.Configuration;
using DotnetDemo.Persistence.Mappings;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using Microsoft.Extensions.Options;
using NHibernate;
using NHibernate.Cfg;
using ILoggerFactory = Microsoft.Extensions.Logging.ILoggerFactory;
using NHConfiguration = NHibernate.Cfg.Configuration;

namespace DotnetDemo.Persistence;

public class NHibernateSessionFactoryBuilder
{
    private readonly IOptions<DatabaseOptions> _databaseOptions;
    private readonly ILoggerFactory _loggerFactory;

    public NHibernateSessionFactoryBuilder(IOptions<DatabaseOptions> databaseOptions, ILoggerFactory loggerFactory)
    {
        _databaseOptions = databaseOptions;
        _loggerFactory = loggerFactory;
    }

    public ISessionFactory Build()
    {
        var options = _databaseOptions.Value;
        var fluentConfiguration = Fluently.Configure()
            .Database(CreateDatabaseConfiguration(options))
            .Mappings(m => m.FluentMappings
                .AddFromAssemblyOf<ArticleMap>())
            .ExposeConfiguration(ConfigureConventions);

        return fluentConfiguration.BuildSessionFactory();
    }

    private IPersistenceConfigurer CreateDatabaseConfiguration(DatabaseOptions options)
    {
        return options.Provider?.ToLowerInvariant() switch
        {
            "sqlite" => SQLiteConfiguration.Standard
                .ConnectionString(options.ConnectionString)
                .Dialect<NHibernate.Dialect.SQLiteDialect>()
                .Driver<MicrosoftSqliteDriver>(),
            _ => MySQLConfiguration.Standard
                .ConnectionString(options.ConnectionString)
                .Dialect<NHibernate.Dialect.MySQL8Dialect>()
                .Driver<NHibernate.Driver.MySqlDataDriver>()
                .AdoNetBatchSize(50)
        };
    }

    private void ConfigureConventions(NHConfiguration configuration)
    {
        configuration.SetInterceptor(new SqlLoggingInterceptor(_loggerFactory.CreateLogger<SqlLoggingInterceptor>()));
        configuration.SetProperty(NHibernate.Cfg.Environment.UseProxyValidator, "false");
        configuration.SetProperty(NHibernate.Cfg.Environment.Hbm2ddlKeyWords, "none");
        configuration.DataBaseIntegration(db =>
        {
            db.LogSqlInConsole = false;
            db.LogFormattedSql = false;
        });
    }
}

