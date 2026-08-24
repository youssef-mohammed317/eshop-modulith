using Catalog.Data;
using Catalog.Infrastructure.Data.Interceptors;
using Catalog.Infrastructure.Data.Seeding;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Data;
using Shared.Data.Seed;
using System.Reflection;

namespace Catalog;

public static class CatalogModule
{
    public static IServiceCollection AddCatalogModule(this IServiceCollection services, IConfiguration configuration)
    {

        services.AddMediatRService(Assembly.GetExecutingAssembly());

        services.AddScoped<AuditableEntityInterceptor>();
        services.AddScoped<DispatchDomainEventsInterceptor>();

        services.AddDbContextService<CatalogDbContext>(configuration);
        services.AddScoped<IDataSeeder, CatalogDataSeeder>();
        return services;
    }
    public static IApplicationBuilder UseCatalogModule(this IApplicationBuilder app)
    {
        app.UseMigrationAsync<CatalogDbContext>().GetAwaiter().GetResult();
        app.UseDataSeedingAsync().GetAwaiter().GetResult();
        return app;
    }
}
