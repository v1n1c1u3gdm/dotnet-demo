namespace DotnetDemo.Tech;

public interface ITechReportService
{
    Task<string> BuildHtmlAsync(CancellationToken cancellationToken = default);
}

