using Microsoft.EntityFrameworkCore;

namespace Basket.Basket.Features.AddItemToBasket;

public record AddItemToBasketCommand(
    string UserName,
    ShoppingCartItemDto ShoppingCartItem) : ICommand<AddItemToBasketResult>;

public record AddItemToBasketResult(Guid CartId);

public class AddItemToBasketCommandValidator : AbstractValidator<AddItemToBasketCommand>
{
    public AddItemToBasketCommandValidator()
    {
        RuleFor(x => x.UserName).NotEmpty().WithMessage("UserName is required");
        RuleFor(x => x.ShoppingCartItem.ProductId).NotEmpty().WithMessage("ProductId is required");
        RuleFor(x => x.ShoppingCartItem.Quantity).GreaterThan(0).WithMessage("Quantity must be greater than 0");
        RuleFor(x => x.ShoppingCartItem.Price).GreaterThanOrEqualTo(0).WithMessage("Price cannot be negative");
        RuleFor(x => x.ShoppingCartItem.ProductName).NotEmpty().WithMessage("ProductName is required");
    }
}
public class AddItemToBasketCommandHandler(IBasketRepository repository, ISender sender)
    : ICommandHandler<AddItemToBasketCommand, AddItemToBasketResult>
{
    public async Task<AddItemToBasketResult> Handle(AddItemToBasketCommand command, CancellationToken cancellationToken)
    {
        var cart = await repository.GetBasketAsync(command.UserName, cancellationToken);

        if (cart is null)
        {
            cart = ShoppingCart.Create(Guid.NewGuid(), command.UserName);
        }

        var result = await sender.Send(new GetProductByIdQuery(command.ShoppingCartItem.ProductId));

        if (result?.Product == null)
        {
            throw new ProductNotFoundException(command.ShoppingCartItem.ProductId);
        }


        cart.AddItem(
            command.ShoppingCartItem.ProductId,
            command.ShoppingCartItem.Quantity,
            command.ShoppingCartItem.Color,
            result.Product.Price,
            result.Product.Name);
        //command.ShoppingCartItem.Price,
        //command.ShoppingCartItem.ProductName);

        // This stores the basket in EF Core AND updates Redis
        await repository.StoreBasketAsync(cart, cancellationToken);

        // This persists the database transaction
        await repository.SaveChangesAsync(command.UserName, cancellationToken);

        return new AddItemToBasketResult(cart.Id);
    }
}