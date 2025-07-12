using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using Template.Application.Abstraction.Cache;
using Template.Application.Abstraction.src;
using Template.Application.Cache;
using Template.Application.src;
using Template.Application.src.Abstraction;
using Template.Application.src.Abstraction.Base;
using Template.Application.src.Abstraction.Base.Search;
using Template.Application.src.Base;
using Template.Application.src.Base.Search;
using Template.Application.src.Mappings;

namespace Template.Application;

public static class ServiceRegistration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(ProductMapping).Assembly);
        services.AddScoped(typeof(ICrudService<,>), typeof(CrudService<,>));
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IElasticSearchService, ElasticSearchService>();
        
        //add services -> will use reflection to register all services
        return services;
    }
    public static IServiceCollection AddCachingServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMemoryCache(); // This caching approach is suitable for single-server applications only. It will not work reliably if the application is scaled to multiple servers.

        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration["RedisConfiguration:Url"];
            options.InstanceName = configuration["RedisConfiguration:InstanceName"];
        });
        services.AddSingleton<IConnectionMultiplexer>(sp =>
        {
            var config = ConfigurationOptions.Parse(configuration["RedisConfiguration:Url"]!, true);
            config.AbortOnConnectFail = false;
            return ConnectionMultiplexer.Connect(config);
        });
        services.AddScoped<IMemoryCacheService, MemoryCacheService>(); // This caching approach is suitable for single-server applications only. It will not work reliably if the application is scaled to multiple servers.
        services.AddScoped<IRedisCacheService, RedisCacheService>();
        return services;
    }
}