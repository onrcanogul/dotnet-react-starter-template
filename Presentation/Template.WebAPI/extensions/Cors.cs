namespace Template.WebAPI.Extensions;

public static class Cors
{
    public static IServiceCollection AddCorsServices(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("AllowAll",
                corsPolicyBuilder =>  
                {
                    corsPolicyBuilder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                });
        });
        return services;
    }
    public static IApplicationBuilder UseCorsServices(this IApplicationBuilder app)
    {
        app.UseCors("AllowAll");
        return app;
    }
}