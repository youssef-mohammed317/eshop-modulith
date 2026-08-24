using MediatR;
using Microsoft.Extensions.Logging;
using Catalog.Products.Events;

namespace Catalog.Products.Features.EventHandlers;

public class ProductPriceChangedEventHandler(ILogger<ProductPriceChangedEventHandler> logger)
    : INotificationHandler<ProductPriceChangedEvent>
{
    public async Task Handle(ProductPriceChangedEvent notification, CancellationToken cancellationToken)
    {
        // 1. Log the price change
        logger.LogInformation("[DOMAIN EVENT] Product price updated for {ProductId}. New Price: {NewPrice}",
            notification.product.Id,
            notification.product.Price);

        // 2. Add cross-module communication logic here
        // e.g., Publish ProductPriceChangedIntegrationEvent to MassTransit/RabbitMQ

        await Task.CompletedTask;
    }
}