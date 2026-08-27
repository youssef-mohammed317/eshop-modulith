using Basket.BackgroundJobs;
using Basket.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Data;

namespace Basket;

public static class BasketModule
{
    public static IServiceCollection AddBasketModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IBasketRepository, BasketRepository>();
        services.Decorate<IBasketRepository, CachedBasketRepository>(); // Requires Scrutor package
        services.AddDbContextService<BasketDbContext>(configuration);
        services.AddHostedService<OutboxProcessorBackgroundService>();
        return services;
    }
    public static IApplicationBuilder UseBasketModule(this IApplicationBuilder app)
    {
        app.UseMigrationAsync<BasketDbContext>().GetAwaiter().GetResult();
        return app;
    }
}
