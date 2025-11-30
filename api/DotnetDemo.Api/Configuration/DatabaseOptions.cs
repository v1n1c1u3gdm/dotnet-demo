namespace DotnetDemo.Configuration;

public class DatabaseOptions
{
    public const string SectionName = "Database";

    public string Provider { get; set; } = "mysql";
    public string ConnectionString { get; set; } = string.Empty;
}

