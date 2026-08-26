using System.Reflection;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Shared.Messaging.Extensions;

public static class MassTransitExtensions
{
    public static IServiceCollection AddMessageBrokerWithAssemblies(
        this IServiceCollection services,
        IConfiguration configuration,
        params Assembly[] assemblies)
    {
        services.AddMassTransit(config =>
        {
            config.SetKebabCaseEndpointNameFormatter();
            config.SetInMemorySagaRepositoryProvider();

            config.AddConsumers(assemblies);
            config.AddSagaStateMachines(assemblies);
            config.AddSagas(assemblies);
            config.AddActivities(assemblies);

            config.UsingRabbitMq((context, configurator) =>
            {
                // Pull connection details from appsettings.json
                var host = configuration["MessageBroker:Host"] ?? "localhost";
                var username = configuration["MessageBroker:Username"] ?? "guest";
                var password = configuration["MessageBroker:Password"] ?? "guest";

                configurator.Host(host, "/", h =>
                {
                    h.Username(username);
                    h.Password(password);
                });

                // Automatically generate queues and bind them to the consumers
                configurator.ConfigureEndpoints(context);
            });
        });

        return services;
    }
}