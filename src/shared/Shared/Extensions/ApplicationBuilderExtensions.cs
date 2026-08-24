using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shared.Data.Seed;

namespace Shared.Data;

public static class ApplicationBuilderExtensions
{
    public static async Task<IApplicationBuilder> UseMigrationAsync<TContext>(this IApplicationBuilder app)
            where TContext : DbContext
    {
        using var scope = app.ApplicationServices.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<TContext>();

        // Note: In a real distributed environment, wrap this in a Polly retry policy 
        // to handle database container cold starts.
        await context.Database.MigrateAsync();

        return app;
    }
    public static async Task UseDataSeedingAsync(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();

        var seeders = scope.ServiceProvider.GetServices<IDataSeeder>();

        foreach (var seeder in seeders)
        {
            await seeder.SeedAllAsync();
        }
    }
}


