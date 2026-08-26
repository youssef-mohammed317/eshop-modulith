using MassTransit;
using Microsoft.Extensions.Logging;


namespace Catalog.Products.Features.EventHandlers;

public class ProductCreatedEventHandler(
    ILogger<ProductCreatedEventHandler> logger,
    IBus bus) // Inject MassTransit
    : INotificationHandler<ProductCreatedEvent>
{
    public async Task Handle(ProductCreatedEvent notification, CancellationToken cancellationToken)
    {
        // 1. Log the domain event
        logger.LogInformation("[DOMAIN EVENT] Product created: {ProductId} - {ProductName}",
            notification.product.Id,
            notification.product.Name);

        // 2. Map the Rich Domain Model to the Integration Event
        var integrationEvent = new ProductCreatedIntegrationEvent(
            ProductId: notification.product.Id,
            Name: notification.product.Name,
            Category: notification.product.Category.ToList(),
            Description: notification.product.Description,
            ImageFile: notification.product.ImageFile,
            Price: notification.product.Price
        );

        // 3. Publish to the message broker
        await bus.Publish(integrationEvent, cancellationToken);
    }
}