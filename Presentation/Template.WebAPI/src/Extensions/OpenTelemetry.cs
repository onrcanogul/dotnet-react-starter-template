using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OpenTelemetry.Metrics;

namespace Template.WebAPI.Extensions;

public static class OpenTelemetry
{
    public static IServiceCollection AddOpenTelemetryServices(this IServiceCollection services)
    {
        services.AddOpenTelemetry()
            .WithTracing(tracerProviderBuilder =>
            {
                tracerProviderBuilder
                    .AddSource("Template") 
                    .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("MyAppService"))
                    .AddAspNetCoreInstrumentation() // .net core pipeline tracing
                    .AddHttpClientInstrumentation() // HTTP request tracing
                    .AddConsoleExporter(); // log to console
            })
            .WithMetrics(metricsProviderBuilder =>
            {
                metricsProviderBuilder
                    .AddAspNetCoreInstrumentation() // .net core metrics
                    .AddRuntimeInstrumentation() // clr runtime metrics
                    .AddConsoleExporter(); // log metrics to console
            });
        return services;
    }
}