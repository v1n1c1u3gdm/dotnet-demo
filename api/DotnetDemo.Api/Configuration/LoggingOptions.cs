namespace DotnetDemo.Configuration;

public class LoggingOptions
{
    public const string SectionName = "Logging";

    public string Directory { get; set; } = "../logs";
}

