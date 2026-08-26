using Serilog;

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

    builder.Services.AddMediatRService(
        typeof(CatalogModule).Assembly,
        typeof(BasketModule).Assembly,
        typeof(OrderingModule).Assembly
    );

    builder.Services.AddCarter();

    builder.Services.AddStackExchangeRedisCache(options =>
    {
        options.Configuration = builder.Configuration.GetConnectionString("Redis");
    });

    builder.Services
        .AddCatalogModule(builder.Configuration)
        .AddBasketModule(builder.Configuration)
        .AddOrderingModule(builder.Configuration);

    builder.Services.AddExceptionHandler<CustomExceptionHandler>();

    var app = builder.Build();

    // 1. Exception Handling (The outermost layer)
    app.UseExceptionHandler(options => { });

    // 2. Logging (Captures requests and bubbles exceptions to the handler)
    app.UseSerilogRequestLogging();

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