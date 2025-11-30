using Microsoft.Data.Sqlite;
using NHibernate.Driver;

namespace DotnetDemo.Persistence;

public class MicrosoftSqliteDriver : ReflectionBasedDriver
{
    public MicrosoftSqliteDriver()
        : base(
            "Microsoft.Data.Sqlite",
            typeof(SqliteConnection).FullName!,
            typeof(SqliteCommand).FullName!)
    {
    }

    public override bool UseNamedPrefixInSql => true;
    public override bool UseNamedPrefixInParameter => true;
    public override string NamedPrefix => "@";
    public override bool SupportsMultipleOpenReaders => false;
}

