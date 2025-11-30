using DotnetDemo.Observability;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Instrumentation.Runtime;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;

namespace DotnetDemo.Extensions;

public static class ServiceCollectionTelemetryExtensions
{
    public static IServiceCollection AddObservability(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddOptions<TelemetryOptions>()
            .Bind(configuration.GetSection(TelemetryOptions.SectionName));

        services.AddSingleton<RequestMetricsRecorder>();

        var serviceName = configuration.GetSection(TelemetryOptions.SectionName).Get<TelemetryOptions>()?.ServiceName
                          ?? "dotnet-demo-api";

        services.AddOpenTelemetry()
            .WithMetrics(builder =>
            {
                builder
                    .ConfigureResource(resource => resource.AddService(serviceName))
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddRuntimeInstrumentation()
                    .AddMeter(serviceName)
                    .AddPrometheusExporter();
            });

        return services;
    }
}

