using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DotnetDemo.Observability;

public sealed class RequestMetricsRecorder : IDisposable
{
    private readonly Meter _meter;
    private readonly Counter<long> _requestCounter;
    private readonly Histogram<double> _requestDuration;
    private readonly ILogger<RequestMetricsRecorder> _logger;

    public RequestMetricsRecorder(
        IMeterFactory meterFactory,
        IOptions<TelemetryOptions> telemetryOptions,
        ILogger<RequestMetricsRecorder> logger)
    {
        _logger = logger;
        var options = telemetryOptions.Value;
        _meter = meterFactory.Create(options.ServiceName);
        _requestCounter = _meter.CreateCounter<long>("http_server_requests_total", unit: "requests",
            description: "Total HTTP requests processed by the API");
        _requestDuration = _meter.CreateHistogram<double>("http_server_request_duration_seconds",
            unit: "s",
            description: "HTTP request duration in seconds");
    }

    public void Record(string method, string path, int statusCode, double durationSeconds)
    {
        var attributes = new KeyValuePair<string, object?>[]
        {
            new("http.method", method),
            new("http.route", path),
            new("http.status_code", statusCode),
        };

        try
        {
            _requestCounter.Add(1, attributes);
            _requestDuration.Record(durationSeconds, attributes);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to record request metrics");
        }
    }

    public void Dispose()
    {
        _meter.Dispose();
    }
}

