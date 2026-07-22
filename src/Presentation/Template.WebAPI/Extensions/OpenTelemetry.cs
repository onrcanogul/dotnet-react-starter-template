using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Template.WebAPI.Extensions;

public static class OpenTelemetry
{
    /// <summary>Activity source name - use it when adding manual spans.</summary>
    public const string ServiceName = "Template";

    /// <summary>
    /// Traces and metrics go to the OTLP collector when
    /// <c>OTEL_EXPORTER_OTLP_ENDPOINT</c> is set (docker-compose sets it), and
    /// to the console otherwise so a bare `dotnet run` still shows something.
    /// </summary>
    public static IServiceCollection AddOpenTelemetryServices(this IServiceCollection services, IConfiguration configuration)
    {
        // OTEL_SDK_DISABLED is the spec-defined kill switch. Integration tests
        // set it so their output is test results rather than console traces.
        if (string.Equals(configuration["OTEL_SDK_DISABLED"], "true", StringComparison.OrdinalIgnoreCase))
            return services;

        var otlpEndpoint = configuration["OTEL_EXPORTER_OTLP_ENDPOINT"];
        var useOtlp = !string.IsNullOrWhiteSpace(otlpEndpoint);
        var resource = ResourceBuilder.CreateDefault().AddService(ServiceName);

        services.AddOpenTelemetry()
            .WithTracing(tracing =>
            {
                tracing
                    .AddSource(ServiceName)
                    .SetResourceBuilder(resource)
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation();

                if (useOtlp)
                    tracing.AddOtlpExporter(o => o.Endpoint = new Uri(otlpEndpoint!));
                else
                    tracing.AddConsoleExporter();
            })
            .WithMetrics(metrics =>
            {
                metrics
                    .SetResourceBuilder(resource)
                    .AddAspNetCoreInstrumentation()
                    .AddRuntimeInstrumentation();

                if (useOtlp)
                    metrics.AddOtlpExporter(o => o.Endpoint = new Uri(otlpEndpoint!));
                else
                    metrics.AddConsoleExporter();
            });

        return services;
    }
}
