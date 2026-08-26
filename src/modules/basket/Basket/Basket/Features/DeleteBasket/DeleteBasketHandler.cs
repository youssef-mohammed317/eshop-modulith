using Microsoft.EntityFrameworkCore;

namespace Basket.Features.DeleteBasket;

public record DeleteBasketCommand(string UserName) : ICommand<DeleteBasketResult>;
public record DeleteBasketResult(bool IsSuccess);

public class DeleteBasketCommandValidator : AbstractValidator<DeleteBasketCommand>
{
    public DeleteBasketCommandValidator()
    {
        RuleFor(x => x.UserName).NotEmpty().WithMessage("UserName is required");
    }
}

public class DeleteBasketCommandHandler(IBasketRepository repository)
    : ICommandHandler<DeleteBasketCommand, DeleteBasketResult>
{
    public async Task<DeleteBasketResult> Handle(DeleteBasketCommand command, CancellationToken cancellationToken)
    {
        var success = await repository.DeleteBasketAsync(command.UserName, cancellationToken);

        if (!success)
        {
            throw new BasketNotFoundException(command.UserName);
        }

        await repository.SaveChangesAsync(command.UserName, cancellationToken);

        return new DeleteBasketResult(true);
    }
}