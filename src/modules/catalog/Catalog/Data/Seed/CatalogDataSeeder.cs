using Catalog.Data;
using Microsoft.EntityFrameworkCore;
using Shared.Data.Seed;

namespace Catalog.Infrastructure.Data.Seeding;

public class CatalogDataSeeder(CatalogDbContext context) : IDataSeeder
{
    public async Task SeedAllAsync()
    {
        await SeedAsync();
    }

    public async Task SeedAsync()
    {
        // 1. Apply any pending migrations automatically 
        if (context.Database.IsRelational())
        {
            await context.Database.MigrateAsync();
        }

        // 2. Check if the database already has data, if not, pull from InitialData
        if (!await context.Products.AnyAsync())
        {
            await context.Products.AddRangeAsync(InitialData.Products);
            await context.SaveChangesAsync();
        }
    }
}