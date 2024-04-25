namespace Modules.Ordering.Domain;

using BridgingIT.DevKit.Domain;

public class BuyerAndPaymentMethodVerifiedDomainEvent : DomainEventBase
{
    public BuyerAndPaymentMethodVerifiedDomainEvent(Buyer buyer, PaymentMethod payment, Guid orderId)
    {
        this.Buyer = buyer;
        this.Payment = payment;
        this.OrderId = orderId;
    }

    public Buyer Buyer { get; }

    public PaymentMethod Payment { get; }

    public Guid OrderId { get; }
}