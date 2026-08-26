using Catalog.Products.Events;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using Shared.Messaging.Events;

namespace Catalog.Products.Features.EventHandlers;

public class ProductPriceChangedEventHandler(
    ILogger<ProductPriceChangedEventHandler> logger,
    IBus bus) // Inject MassTransit
    : INotificationHandler<ProductPriceChangedEvent>
{
    public async Task Handle(ProductPriceChangedEvent notification, CancellationToken cancellationToken)
    {
        // 1. Log the domain event
        logger.LogInformation("[DOMAIN EVENT] Product price updated for {ProductId}. New Price: {NewPrice}",
            notification.product.Id,
            notification.product.Price);

        // 2. Map the Rich Domain Model to the Integration Event
        var integrationEvent = new ProductPriceChangedIntegrationEvent(
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