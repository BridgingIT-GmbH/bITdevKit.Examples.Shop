namespace Modules.Ordering.Domain;

using BridgingIT.DevKit.Domain;

public class OrderStatusChangedToPaidDomainEvent : DomainEventBase
{
    public OrderStatusChangedToPaidDomainEvent(Guid orderId,
        IEnumerable<OrderItem> orderItems)
    {
        this.OrderId = orderId;
        this.OrderItems = orderItems;
    }

    public Guid OrderId { get; }

    public IEnumerable<OrderItem> OrderItems { get; }
}
