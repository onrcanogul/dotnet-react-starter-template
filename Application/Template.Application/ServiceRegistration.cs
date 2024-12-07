using Microsoft.Extensions.DependencyInjection;
using Template.Application.src;
using Template.Application.src.Abstraction;
using Template.Application.src.Abstraction.Base;
using Template.Application.src.Base;
using Template.Application.src.Base.Mapping;

namespace Template.Application;

public static class ServiceRegistration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped(typeof(ICrudService<,>), typeof(CrudService<,>));
        services.AddScoped<IProductService, ProductService>();
        
        AddAutoMapper(services);
        
        return services;
    }
    private static void AddAutoMapper(IServiceCollection services)
    {
        services.AddAutoMapper(typeof(BaseMapping).Assembly);
    }
    
}