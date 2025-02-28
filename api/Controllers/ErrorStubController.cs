using API.Data;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

public class ErrorStubController : BaseApiController
{
    [HttpGet("e400")]
    public ActionResult Error400() => BadRequest("Default bad request msg");

    [HttpGet("e401")]
    public ActionResult Error401() => Unauthorized("Not you");

    [HttpGet("e403")]
    public ActionResult Forbidden403() => new ContentResult() { Content = "Not here", StatusCode = 403 };

    [HttpGet("e404")]
    public ActionResult Error404() => NotFound("Not found");

    [HttpGet("e405")]
    public ActionResult MethodNotAllowed405() => new ContentResult() { Content = "Not this way", StatusCode = 405 };

    [HttpGet("e407")]
    public ActionResult ProxyAuthenticationRequired407() => new ContentResult() { Content = "Not available now", StatusCode = 407 };

    [HttpGet("e408")]
    public ActionResult RequestTimeOut408() => new ContentResult() { Content = "Took too long", StatusCode = 408 };

    [HttpGet("e429")]
    public ActionResult TooManyRequests429() => new ContentResult() { Content = "Calm down!", StatusCode = 429 };

    [HttpGet("e500")]
    public ActionResult Error500() => throw new Exception("This one is on me");
}
