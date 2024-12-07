using System.ComponentModel.Design;
using Microsoft.Extensions.DependencyInjection;

namespace Template.Infrastructure;

public static class ServiceRegistration
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        return services;
    }
}