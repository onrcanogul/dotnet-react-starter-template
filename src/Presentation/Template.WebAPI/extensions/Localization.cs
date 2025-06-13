using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Localization;
using System.Globalization;

namespace Template.WebAPI.Extensions;

public static class Localization
{
    public static IServiceCollection AddLocalizationServices(this IServiceCollection services)
    {
        services.AddLocalization(options => options.ResourcesPath = "resources/");
        services.AddSingleton<IStringLocalizerFactory>(sp =>
        {
            var resourcesPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "resources/");
            return new JsonStringLocalizerFactory(resourcesPath);
        });
        services.AddSingleton<IStringLocalizer>(sp =>
        {
            var factory = sp.GetRequiredService<IStringLocalizerFactory>();
            return factory.Create(typeof(object));
        });
        return services;
    }
    public static IApplicationBuilder UseLocalizationServices(this IApplicationBuilder app)
    {
        app.UseRequestLocalization(new RequestLocalizationOptions
        {
            DefaultRequestCulture = new RequestCulture("en-US"),
            SupportedCultures = new[] { new CultureInfo("en-US"), new CultureInfo("tr-TR") }, 
            SupportedUICultures = new[] { new CultureInfo("en-US"), new CultureInfo("tr-TR") }
        });
        return app;
    }
}