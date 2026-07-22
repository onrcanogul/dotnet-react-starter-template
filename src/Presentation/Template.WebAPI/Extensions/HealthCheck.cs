using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Template.Shared.Configuration;

namespace Template.WebAPI.Extensions;

public static class HealthCheck
{
    public static IServiceCollection AddHealthCheckServices(this IServiceCollection services, IConfiguration configuration)
    {
        var postgresConnectionString = configuration.GetRequiredConnectionString("PostgreSQL");
        var redisConnectionString = configuration.GetRequired("RedisConfiguration:Url");
        services
            .AddHealthChecks()
            .AddNpgSql(postgresConnectionString, name: "postgresql")
            .AddRedis(redisConnectionString, name: "redis");
        
        services.AddHealthChecksUI(setup =>
        {
            setup.AddHealthCheckEndpoint("Template App", "/health");
        }).AddInMemoryStorage();
        return services;
    }

    public static WebApplication UseHealthCheckServices(this WebApplication app)
    {
        app.MapHealthChecks("/health", new HealthCheckOptions
        {
            Predicate = _ => true,
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });
        app.MapHealthChecksUI(options => options.UIPath = "/health-ui");
        return app;
    }
}