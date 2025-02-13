using System.Threading.RateLimiting;

namespace Template.WebAPI.Extensions;

public static class RateLimiter
{
    public static IServiceCollection AddRateLimiterServices(this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
            RateLimitPartition.GetConcurrencyLimiter(
            partitionKey: httpContext.User.Identity?.Name ?? httpContext.Request.Headers.Host.ToString(),
            factory: partition => new ConcurrencyLimiterOptions
            {
                PermitLimit = 1 // only concurrency 1 request
            }));
            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
                RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: httpContext.User.Identity?.Name ?? httpContext.Request.Headers.Host.ToString(),
                    factory: partition => new FixedWindowRateLimiterOptions
                    {
                        AutoReplenishment = true,
                        PermitLimit = 10,
                        QueueLimit = 1,
                        Window = TimeSpan.FromSeconds(5)
                    }));
            options.RejectionStatusCode = 429;
        });
        return services;
    }
}