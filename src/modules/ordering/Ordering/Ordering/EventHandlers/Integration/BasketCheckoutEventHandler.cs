using BuildingBlocks.Messaging.Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Ordering.Ordering.EventHandlers.Integration;

public class BasketCheckoutEventHandler(ISender sender, ILogger<BasketCheckoutEventHandler> logger)
    : IConsumer<BasketCheckoutIntegrationEvent>
{
    public async Task Consume(ConsumeContext<BasketCheckoutIntegrationEvent> context)
    {
        // 1. Log the incoming integration event
        logger.LogInformation("Integration Event handled: {IntegrationEvent}", context.Message.GetType().Name);

        // 2. Map the Integration Event payload to the Application Command
        var command = MapToCreateOrderCommand(context.Message);

        // 3. Send the command to MediatR pipeline to create the order
        await sender.Send(command);
    }

    private CreateOrderCommand MapToCreateOrderCommand(BasketCheckoutIntegrationEvent message)
    {
        // Map Address
        var addressDto = new AddressDto(
                message.FirstName,
                message.LastName,
                message.EmailAddress,
                message.AddressLine,
                message.Country,
                message.State,
                message.ZipCode);

        // Map Payment
        var paymentDto = new PaymentDto(
                message.CardName,
                message.CardNumber,
                message.Expiration,
                message.Cvv,
                message.PaymentMethod);

        var orderId = Guid.NewGuid();

        var orderDto = new OrderDto(
             orderId,
             message.CustomerId,
             $"Order_{message.UserName}_{orderId.ToString().Substring(0, 8)}", // Generate a unique name
             addressDto,
             addressDto,
             paymentDto,
             Items: [
                new OrderItemDto(orderId,Guid.Parse("c67d6323-e8b1-4bdf-9a75-b0d0d2e7e914"), 2, 1999.00m),
                new OrderItemDto(orderId,Guid.Parse("5334c996-8457-4cf0-815c-ed2b77c4ff61"), 1, 999.00m)
             ]);

        return new CreateOrderCommand(orderDto);
    }
}