using MediatR;
using Microsoft.Extensions.Logging;
using Catalog.Products.Events;

namespace Catalog.Products.Features.EventHandlers;

public class ProductCreatedEventHandler(ILogger<ProductCreatedEventHandler> logger)
    : INotificationHandler<ProductCreatedEvent>
{
    public async Task Handle(ProductCreatedEvent notification, CancellationToken cancellationToken)
    {
        // 1. Log the domain event
        logger.LogInformation("[DOMAIN EVENT] Product created: {ProductId} - {ProductName}",
            notification.product.Id,
            notification.product.Name);

        // 2. Add business logic here
        // e.g., mapping to an Integration Event and pushing to a message broker

        await Task.CompletedTask;
    }
}