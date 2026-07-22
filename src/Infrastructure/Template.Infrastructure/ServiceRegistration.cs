using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Template.Infrastructure.Authentication;
using Template.Shared.Configuration;

namespace Template.Infrastructure;

public static class ServiceRegistration
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IJwtTokenHandler, JwtTokenHandler>();

        // Registered under the default "Bearer" scheme. Naming the scheme here
        // (e.g. AddJwtBearer("Admin", ...)) while AddAuthentication defaults to
        // "Bearer" leaves the default scheme with no handler, and every
        // [Authorize] request fails with "No authentication handler is
        // registered for the scheme 'Bearer'".
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new()
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = true,
                ValidIssuer = configuration.GetRequired("Token:Issuer"),
                ValidAudience = configuration.GetRequired("Token:Audience"),
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetRequired("Token:SecurityKey"))),
                ClockSkew = TimeSpan.FromSeconds(30),
                NameClaimType = ClaimTypes.Name,
            };
        });

        services.AddAuthorization();
        services.AddExceptionHandler<Middlewares.ExceptionHandler>();
        return services;
    }

    public static IApplicationBuilder UseInfrastructureServices(this IApplicationBuilder app)
    {
        app.UseExceptionHandler(options => { });
        return app;
    }
}
