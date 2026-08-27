// Location: Ordering/Exceptions/OrderNotFoundException.cs
namespace Ordering.Exceptions;

public class OrderNotFoundException : NotFoundException
{
    public OrderNotFoundException(Guid id)
        : base("Order", id)
    {
    }
}