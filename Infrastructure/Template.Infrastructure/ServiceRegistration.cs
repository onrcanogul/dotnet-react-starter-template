using System.ComponentModel.Design;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Template.Infrastructure;

public static class ServiceRegistration
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<ITokenHandler, TokenHandler>();
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer("Admin", options =>
        {
            options.TokenValidationParameters = new()
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = true,
                ValidIssuer = configuration["Token:Issuer"],
                ValidAudience = configuration["Token:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Token:SecurityKey"]!)),
                LifetimeValidator = (notBefore, expires, securityToken, validationParameters) => expires != null && expires > DateTime.UtcNow,
                NameClaimType = ClaimTypes.Name,      
            };
        });
        services.AddExceptionHandler<Template.Infrastructure.Middlewares.ExceptionHandler>();
        return services;
    }
    
    public static IApplicationBuilder UseInfrastructureServices(this IApplicationBuilder app)
    {
        app.UseExceptionHandler(options => { });
        return app;
    }
}