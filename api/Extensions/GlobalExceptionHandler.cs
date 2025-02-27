using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace api.Extensions;

internal sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext ctx, Exception ex, CancellationToken cancelToken)
    {
        logger.LogError(ex, "Exception occurred: {Message}", ex.Message);

        var details = new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "Server error"
        };

        ctx.Response.StatusCode = details.Status.Value;
        await ctx.Response.WriteAsJsonAsync(details, cancelToken);

        return true;
    }
}