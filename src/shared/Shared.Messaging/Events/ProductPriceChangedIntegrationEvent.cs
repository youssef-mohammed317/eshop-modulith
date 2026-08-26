namespace Shared.Messaging.Events;

public record ProductPriceChangedIntegrationEvent(
    Guid ProductId,
    string Name,
    List<string> Category,
    string Description,
    string ImageFile,
    decimal Price
) : IntegrationEvent;
