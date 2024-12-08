using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;

namespace Template.WebAPI.Extensions;

public static class Versioning
{
    public static IServiceCollection AddVersioningServices(this IServiceCollection services)
    {
        services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(1, 0); 
            options.AssumeDefaultVersionWhenUnspecified = true; 
            options.ReportApiVersions = true;
        });
        services.AddApiVersioning(options =>
        {
            options.ApiVersionReader = new HeaderApiVersionReader("x-api-version");
        });

        return services;
    }
    
}