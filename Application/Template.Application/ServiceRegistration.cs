using Microsoft.Extensions.DependencyInjection;
using Template.Application.Abstraction.src;
using Template.Application.src;
using Template.Application.src.Abstraction;
using Template.Application.src.Abstraction.Base;
using Template.Application.src.Base;
using Template.Application.src.Mappings;

namespace Template.Application;

public static class ServiceRegistration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(ProductMapper).Assembly);
        services.AddScoped(typeof(ICrudService<,>), typeof(CrudService<,>));
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IUserService, UserService>();
        return services;
    }
}