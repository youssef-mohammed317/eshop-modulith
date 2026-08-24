using Catalog.Infrastructure.Data.Seeding;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Data;
using Shared.Data.Seed;

namespace Catalog;

public static class CatalogModule
{
    public static IServiceCollection AddCatalogModule(this IServiceCollection services, IConfiguration configuration)
    {
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
