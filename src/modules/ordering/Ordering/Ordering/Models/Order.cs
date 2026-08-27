namespace Ordering.Ordering.Models;

public class Order : Aggregate<Guid>
{
    private readonly List<OrderItem> _orderItems = new();
    public IReadOnlyList<OrderItem> OrderItems => _orderItems.AsReadOnly();

    public Guid CustomerId { get; private set; }
    public string OrderName { get; private set; } = default!;
    public Address ShippingAddress { get; private set; } = default!;
    public Address BillingAddress { get; private set; } = default!;
    public Payment Payment { get; private set; } = default!;
    public OrderStatus OrderStatus { get; private set; } // Added missing property

    public decimal TotalPrice
    {
        get => OrderItems.Sum(x => x.Price * x.Quantity);
        private set { }
    }

    protected Order() { }

    private Order(
        Guid id,
        Guid customerId,
        string orderName,
        Address shippingAddress,
        Address billingAddress,
        Payment payment,
        OrderStatus orderStatus)
    {
        Id = id;
        CustomerId = customerId;
        OrderName = orderName;
        ShippingAddress = shippingAddress;
        BillingAddress = billingAddress;
        Payment = payment;
        OrderStatus = orderStatus;
    }

    public static Order Create(
        Guid id,
        Guid customerId,
        string orderName,
        Address shippingAddress,
        Address billingAddress,
        Payment payment)
    {
        if (string.IsNullOrWhiteSpace(orderName)) throw new ArgumentException("Order Name cannot be null or empty.");
        if (shippingAddress == null) throw new ArgumentException("Shipping Address cannot be null.");
        if (billingAddress == null) throw new ArgumentException("Billing Address cannot be null.");
        if (payment == null) throw new ArgumentException("Payment details cannot be null.");

        var order = new Order(
            id,
            customerId,
            orderName,
            shippingAddress,
            billingAddress,
            payment,
            OrderStatus.Pending);

        order.AddDomainEvent(new OrderCreatedEvent(order));

        return order;
    }

    public void Update(
        string orderName,
        Address shippingAddress,
        Address billingAddress,
        Payment payment,
        OrderStatus orderStatus)
    {
        if (string.IsNullOrWhiteSpace(orderName)) throw new ArgumentException("Order Name cannot be null or empty.");
        if (shippingAddress == null) throw new ArgumentException("Shipping Address cannot be null.");
        if (billingAddress == null) throw new ArgumentException("Billing Address cannot be null.");
        if (payment == null) throw new ArgumentException("Payment details cannot be null.");

        OrderName = orderName;
        ShippingAddress = shippingAddress;
        BillingAddress = billingAddress;
        Payment = payment;
        OrderStatus = orderStatus;

        AddDomainEvent(new OrderUpdatedEvent(this));
    }

    public void Add(Guid productId, int quantity, decimal price)
    {
        var existingItem = _orderItems.FirstOrDefault(x => x.ProductId == productId);

        if (existingItem != null)
        {
            existingItem.AddQuantity(quantity);
        }
        else
        {
            var orderItem = OrderItem.Create(Id, productId, quantity, price);
            _orderItems.Add(orderItem);
        }
    }

    public void Remove(Guid productId)
    {
        var orderItem = _orderItems.FirstOrDefault(x => x.ProductId == productId);
        if (orderItem != null)
        {
            _orderItems.Remove(orderItem);
        }
    }

    public void ChangeStatus(OrderStatus newStatus)
    {
        if (OrderStatus == OrderStatus.Completed || OrderStatus == OrderStatus.Cancelled)
        {
            throw new ArgumentException($"Cannot change status from {OrderStatus} to {newStatus}. Order is already finalized.");
        }

        OrderStatus = newStatus;
        AddDomainEvent(new OrderStatusChangedEvent(Id, newStatus));
    }
}