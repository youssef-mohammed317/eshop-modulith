using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Shared.Data;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMediatRService(this IServiceCollection services, params Assembly[] assemblies)
    {
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssemblies(assemblies);
        });
        return services;
    }

    public static IServiceCollection AddDbContextService<TContext>(this IServiceCollection services, IConfiguration configuration)
            where TContext : DbContext
    {
        var connectionString = configuration.GetConnectionString("Database");

        services.AddDbContext<TContext>((sp, options) =>
        {
            // Dynamically resolves any interceptors registered in the microservice (e.g., Auditable, DispatchEvents)
            options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
            options.UseNpgsql(connectionString);
        });
        return services;
    }
}


