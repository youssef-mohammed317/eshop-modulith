using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace BuildingBlocks.Messaging.MassTransit;

public static class Extensions
{
    public static IServiceCollection AddMessageBroker(this IServiceCollection services, IConfiguration configuration, Assembly? assembly = null)
    {
        services.AddMassTransit(config =>
        {
            // Set the naming convention to kebab-case (e.g., basket-checkout-event)
            config.SetKebabCaseEndpointNameFormatter();

            // Automatically register all consumers in the provided assembly
            if (assembly != null)
                config.AddConsumers(assembly);

            // Configure RabbitMQ using the settings from appsettings.json
            config.UsingRabbitMq((context, configurator) =>
            {
                var host = configuration["MessageBroker:Host"]!;
                var userName = configuration["MessageBroker:UserName"]!;
                var password = configuration["MessageBroker:Password"]!;

                configurator.Host(new Uri(host), h =>
                {
                    h.Username(userName);
                    h.Password(password);
                });

                // Automatically configure the endpoints based on the registered consumers
                configurator.ConfigureEndpoints(context);
            });
        });
        return services;
    }
}
