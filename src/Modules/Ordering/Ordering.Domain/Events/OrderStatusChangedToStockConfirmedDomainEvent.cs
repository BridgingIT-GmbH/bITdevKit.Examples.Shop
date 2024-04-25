namespace Modules.Ordering.Domain;

using BridgingIT.DevKit.Domain;

public class OrderStatusChangedToStockConfirmedDomainEvent : DomainEventBase
{
    public OrderStatusChangedToStockConfirmedDomainEvent(Guid orderId)
        => this.OrderId = orderId;

    public Guid OrderId { get; }
}
