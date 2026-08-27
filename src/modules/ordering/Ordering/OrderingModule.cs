using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ordering.Data;
using Shared.Data;

namespace Ordering;

public static class OrderingModule
{
    public static IServiceCollection AddOrderingModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContextService<OrderingDbContext>(configuration);
        return services;
    }
    public static IApplicationBuilder UseOrderingModule(this IApplicationBuilder app)
    {
        app.UseMigrationAsync<OrderingDbContext>().GetAwaiter().GetResult();
        return app;
    }
}
