namespace Modules.Ordering.Domain;

using BridgingIT.DevKit.Domain;

public class OrderCancelledDomainEvent : DomainEventBase
{
    public OrderCancelledDomainEvent(Order order)
    {
        this.Order = order;
    }

    public Order Order { get; }
}
