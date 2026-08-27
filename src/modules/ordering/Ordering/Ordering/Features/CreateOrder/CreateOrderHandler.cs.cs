// Location: Ordering/Features/CreateOrder/CreateOrderHandler.cs
namespace Ordering.Ordering.Features.CreateOrder;

public record CreateOrderCommand(OrderDto Order) : ICommand<CreateOrderResult>;
public record CreateOrderResult(Guid Id);

public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator()
    {
        RuleFor(x => x.Order.OrderName).NotEmpty().WithMessage("Order Name is required");
        RuleFor(x => x.Order.CustomerId).NotEmpty().WithMessage("CustomerId is required");
        RuleFor(x => x.Order.Items).NotEmpty().WithMessage("Order must contain items");
    }
}

public class CreateOrderCommandHandler(OrderingDbContext dbContext)
    : ICommandHandler<CreateOrderCommand, CreateOrderResult>
{
    public async Task<CreateOrderResult> Handle(CreateOrderCommand command, CancellationToken cancellationToken)
    {
        var dto = command.Order;

        // 1. Map Value Objects
        var shippingAddress = Address.Of(dto.ShippingAddress.FirstName, dto.ShippingAddress.LastName, dto.ShippingAddress.EmailAddress, dto.ShippingAddress.AddressLine, dto.ShippingAddress.Country, dto.ShippingAddress.State, dto.ShippingAddress.ZipCode);
        var billingAddress = Address.Of(dto.BillingAddress.FirstName, dto.BillingAddress.LastName, dto.BillingAddress.EmailAddress, dto.BillingAddress.AddressLine, dto.BillingAddress.Country, dto.BillingAddress.State, dto.BillingAddress.ZipCode);
        var payment = Payment.Of(dto.Payment.CardName, dto.Payment.CardNumber, dto.Payment.Expiration, dto.Payment.Cvv, dto.Payment.PaymentMethod);

        // 2. Create Aggregate Root
        var newOrder = Order.Create(
            id: Guid.NewGuid(),
            customerId: dto.CustomerId,
            orderName: dto.OrderName,
            shippingAddress: shippingAddress,
            billingAddress: billingAddress,
            payment: payment);

        // 3. Add Items
        foreach (var item in dto.Items)
        {
            newOrder.Add(item.ProductId, item.Quantity, item.Price);
        }

        // 4. Save to Database
        dbContext.Orders.Add(newOrder);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new CreateOrderResult(newOrder.Id);
    }
}