using DotnetDemo.Tech;
using Microsoft.AspNetCore.Mvc;

namespace DotnetDemo.Controllers;

[ApiController]
[Route("tech")]
public class TechController : ControllerBase
{
    private readonly ITechReportService _techReportService;

    public TechController(ITechReportService techReportService)
    {
        _techReportService = techReportService;
    }

    [HttpGet]
    public async Task<IActionResult> Show(CancellationToken cancellationToken)
    {
        var html = await _techReportService.BuildHtmlAsync(cancellationToken);
        return Content(html, "text/html");
    }
}

