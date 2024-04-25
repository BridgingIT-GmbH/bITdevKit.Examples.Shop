namespace Modules.Ordering.Domain;

using BridgingIT.DevKit.Domain;

public class OrderShippedDomainEvent : DomainEventBase
{
    public OrderShippedDomainEvent(Order order)
    {
        this.Order = order;
    }

    public Order Order { get; }
}
