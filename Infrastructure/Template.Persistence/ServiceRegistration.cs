using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Template.Persistence.Contexts;
using Template.Persistence.Repository;
using Template.Persistence.UnitOfWork;

namespace Template.Persistence;

public static class ServiceRegistration
{
    public static IServiceCollection AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IUnitOfWork, UnitOfWork.UnitOfWork>();
        DbRegistration(services, configuration);
        return services;
    }
    private static void DbRegistration(IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<TemplateDbContext>(options => options.UseNpgsql(configuration.GetConnectionString("PostgreSQL")));
    } 
}