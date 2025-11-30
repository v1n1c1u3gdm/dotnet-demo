using Microsoft.Extensions.Logging;
using NHibernate;

namespace DotnetDemo.Persistence;

public class SqlLoggingInterceptor : EmptyInterceptor
{
    private readonly ILogger<SqlLoggingInterceptor> _logger;

    public SqlLoggingInterceptor(ILogger<SqlLoggingInterceptor> logger)
    {
        _logger = logger;
    }

    public override NHibernate.SqlCommand.SqlString OnPrepareStatement(NHibernate.SqlCommand.SqlString sql)
    {
        _logger.LogDebug("NH SQL: {Sql}", sql.ToString());
        return base.OnPrepareStatement(sql);
    }
}

