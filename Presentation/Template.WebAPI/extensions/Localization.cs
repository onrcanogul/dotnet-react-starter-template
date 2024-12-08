using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Localization;
using Template.Infrastructure.Localization;

namespace Template.WebAPI.Extensions;

public static class Localization
{
    public static IServiceCollection AddLocalizationServices(this IServiceCollection services)
    {
        services.AddLocalization(options => options.ResourcesPath = "resources");
        services.AddSingleton<IStringLocalizerFactory>(new JsonStringLocalizerFactory("resources"));
        services.AddSingleton<IStringLocalizer, JsonStringLocalizer>();
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