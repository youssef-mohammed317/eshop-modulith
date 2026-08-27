using Basket.Data; // Ensure this matches your namespace for BasketDbContext
using Basket.Data.Repository;
using BuildingBlocks.Messaging.Events;
using FluentValidation;
using Mapster;
using MassTransit;
using Shared.CQRS;
using Shared.Messaging.Outbox;
using System.Text.Json;

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
    BasketDbContext dbContext) // IBus removed since the background worker will handle publishing
    : ICommandHandler<CheckoutBasketCommand, CheckoutBasketResult>
{
    public async Task<CheckoutBasketResult> Handle(CheckoutBasketCommand command, CancellationToken cancellationToken)
    {
        // Start the EF Core database transaction
        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);

        try
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
            eventMessage.TotalPrice = basket.Items.Sum(x => x.Price * x.Quantity);

            // 4. Serialize and save to the Outbox
            var outboxMessage = new OutboxMessage
            {
                Id = Guid.NewGuid(),
                Type = eventMessage.GetType().AssemblyQualifiedName ?? eventMessage.GetType().Name,
                Content = JsonSerializer.Serialize(eventMessage),
                OccurredOn = DateTime.UtcNow,
                ProcessedOn = null // Stays null until the background worker publishes it
            };

            dbContext.OutboxMessages.Add(outboxMessage);

            // 5. Delete the basket since the checkout process has started
            await repository.DeleteBasketAsync(command.BasketCheckoutDto.UserName, cancellationToken);

            // 6. Commit the deletion and the outbox message to the database
            await repository.SaveChangesAsync(command.BasketCheckoutDto.UserName, cancellationToken);

            // 7. Commit the transaction
            await transaction.CommitAsync(cancellationToken);

            return new CheckoutBasketResult(true);
        }
        catch
        {
            // Rollback if any step fails
            await transaction.RollbackAsync(cancellationToken);
            return new CheckoutBasketResult(false);
        }
    }
}