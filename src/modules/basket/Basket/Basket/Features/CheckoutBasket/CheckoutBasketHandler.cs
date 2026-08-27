// Location: Basket/Features/CheckoutBasket/CheckoutBasketHandler.cs
using BuildingBlocks.Messaging.Events;
using MassTransit;

namespace Basket.Features.CheckoutBasket;

public record CheckoutBasketCommand(BasketCheckoutDto BasketCheckoutDto) : ICommand<CheckoutBasketResult>;

public record CheckoutBasketResult(bool IsSuccess);

public class CheckoutBasketCommandValidator : AbstractValidator<CheckoutBasketCommand>
{
    public CheckoutBasketCommandValidator()
    {
        RuleFor(x => x.BasketCheckoutDto.UserName).NotEmpty().WithMessage("UserName is required");
        RuleFor(x => x.BasketCheckoutDto.CustomerId).NotEmpty().WithMessage("CustomerId is required");
        RuleFor(x => x.BasketCheckoutDto.CardNumber).NotEmpty().WithMessage("CardNumber is required");
    }
}
public class CheckoutBasketCommandHandler(
    IBasketRepository repository,
    IBus bus)
    : ICommandHandler<CheckoutBasketCommand, CheckoutBasketResult>
{
    public async Task<CheckoutBasketResult> Handle(CheckoutBasketCommand command, CancellationToken cancellationToken)
    {
        // 1. Retrieve the existing basket
        var basket = await repository.GetBasketAsync(command.BasketCheckoutDto.UserName, cancellationToken);

        if (basket is null)
        {
            return new CheckoutBasketResult(false);
        }

        // 2. Map the incoming DTO to your Integration Event
        var eventMessage = command.BasketCheckoutDto.Adapt<BasketCheckoutIntegrationEvent>();

        // 3. SECURITY: Overwrite the TotalPrice with the trusted backend calculation
        // This prevents users from manipulating the price on the frontend
        eventMessage.TotalPrice = basket.Items.Sum(x => x.Price * x.Quantity);

        // 4. Publish the Integration Event to RabbitMQ
        await bus.Publish(eventMessage, cancellationToken);

        // 5. Delete the basket since the checkout process has started
        await repository.DeleteBasketAsync(command.BasketCheckoutDto.UserName, cancellationToken);

        // Commit the deletion to the database
        await repository.SaveChangesAsync(command.BasketCheckoutDto.UserName, cancellationToken);

        return new CheckoutBasketResult(true);
    }
}