using Microsoft.AspNetCore.Mvc;

namespace DotnetDemo.Controllers;

[ApiController]
public class HealthController : ControllerBase
{
[HttpGet("/up")]
public IActionResult Up()
    {
        return Ok(new
        {
            status = "ok",
            service = "dotnet-demo-api",
            timestamp = DateTime.UtcNow
        });
    }
}

