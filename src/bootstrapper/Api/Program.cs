using Microsoft.FeatureManagement;
using Serilog;
using Shared.Messaging.Extensions;

// 1. Initial bootstrap logger to catch startup issues
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information("Starting e-shop API bootstrapper...");
    var builder = WebApplication.CreateBuilder(args);
    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services));
    var orderAssembly = typeof(OrderingModule).Assembly;
    var catalogAssembly = typeof(CatalogModule).Assembly;
    var basketAssembly = typeof(BasketModule).Assembly;
    builder.Services.AddMediatRService(catalogAssembly, basketAssembly, orderAssembly);

    builder.Services.AddCarter();

    builder.Services.AddStackExchangeRedisCache(options =>
    {
        options.Configuration = builder.Configuration.GetConnectionString("Redis");
    });


    builder.Services
        .AddCatalogModule(builder.Configuration)
        .AddBasketModule(builder.Configuration)
        .AddOrderingModule(builder.Configuration);

    builder.Services.AddMessageBrokerWithAssemblies(builder.Configuration, catalogAssembly, basketAssembly, orderAssembly);

    builder.Services.AddExceptionHandler<CustomExceptionHandler>();
    builder.Services.AddFeatureManagement();

    builder.Services.AddKeycloakWebApiAuthentication(builder.Configuration, options =>
    {
        options.TokenValidationParameters.ValidIssuers = new[]
        {
        "http://localhost:9090/realms/eshop-realm",
        "http://keycloak:8080/realms/eshop-realm"
    };
    }); builder.Services.AddAuthorization();

    var app = builder.Build();

    // 1. Exception Handling (The outermost layer)
    app.UseExceptionHandler(options => { });

    // 2. Logging (Captures requests and bubbles exceptions to the handler)
    app.UseSerilogRequestLogging();

    app.UseAuthentication();
    app.UseAuthorization();

    // 3. Module Initialization (Runs migrations, seeding, etc. before taking traffic)
    app
        .UseCatalogModule()
        .UseBasketModule()
        .UseOrderingModule();

    // 4. Endpoint Routing (The final destination for the request)
    app.MapCarter();


    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    // 4. Ensure all logs are flushed before the app completely closes
    Log.CloseAndFlush();
}