namespace Template.WebAPI.Extensions;

public static class JsonSerializer
{
    public static IServiceCollection AddJsonSerializerServices(this IServiceCollection services)
    {
        services.AddControllers().AddNewtonsoftJson(x => 
            x.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
        return services;
    }
}