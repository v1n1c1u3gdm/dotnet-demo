using System.Diagnostics;
using DotnetDemo.Observability;

namespace DotnetDemo.Middleware;

public class RequestMetricsMiddleware
{
    private readonly RequestDelegate _next;
    private readonly RequestMetricsRecorder _recorder;

    public RequestMetricsMiddleware(RequestDelegate next, RequestMetricsRecorder recorder)
    {
        _next = next;
        _recorder = recorder;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        try
        {
            await _next(context);
        }
        finally
        {
            stopwatch.Stop();
            _recorder.Record(
                context.Request.Method,
                NormalizePath(context),
                context.Response?.StatusCode ?? StatusCodes.Status500InternalServerError,
                stopwatch.Elapsed.TotalSeconds);
        }
    }

    private static string NormalizePath(HttpContext context)
    {
        var path = context.Request.Path.ToString();
        return System.Text.RegularExpressions.Regex.Replace(path, @"\/\d+", "/:id");
    }
}

