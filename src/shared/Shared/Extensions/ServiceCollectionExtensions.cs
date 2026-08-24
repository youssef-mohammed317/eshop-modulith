using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Behaviors;
using Shared.Data.Interceptors;
using System.Reflection;

namespace Shared.Data;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMediatRService(this IServiceCollection services, params Assembly[] assemblies)
    {
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssemblies(assemblies);

            // Later on, you can easily add cross-cutting concerns here for ALL modules:
            config.AddOpenBehavior(typeof(ValidationBehavior<,>));
            config.AddOpenBehavior(typeof(LoggingBehavior<,>));
        });
        return services;
    }
    public static IServiceCollection AddDbContextService<TContext>(this IServiceCollection services, IConfiguration configuration)
            where TContext : DbContext
    {
        services.AddScoped<AuditableEntityInterceptor>();
        services.AddScoped<DispatchDomainEventsInterceptor>();

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


