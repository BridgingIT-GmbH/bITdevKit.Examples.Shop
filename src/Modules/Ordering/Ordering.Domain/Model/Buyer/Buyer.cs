namespace Modules.Ordering.Domain;

using BridgingIT.DevKit.Domain.Model;

public class Buyer : AggregateRoot<Guid> // TODO: use strongly typed ids https://www.youtube.com/watch?v=z4SB5BkQX7M
{
    private readonly List<PaymentMethod> paymentMethods = new();

    protected Buyer()
    {
    }

    public string Identity { get; init; } // userId

    public string Name { get; init; }

    public EmailAddress Email { get; init; } // TODO: try to query this (.Endswith @domain.com) with some integration test + specs. See https://khalidabuhakmeh.com/entity-framework-core-conversions-for-logical-domain-types

    public IEnumerable<PaymentMethod> PaymentMethods => this.paymentMethods;

    public static Buyer ForUser(string identity, string name, string email)
    {
        return new Buyer
        {
            Identity = identity,
            Name = name,
            Email = EmailAddress.For(email)
        };
    }

    public PaymentMethod VerifyOrAddPaymentMethod(
        int cardTypeId, string alias, string cardNumber,
        string securityNumber, string cardHolderName, DateTime expiration, Guid orderId)
    {
        var existingPaymentMethod = this.paymentMethods
            .SingleOrDefault(p => p.IsEqualTo(cardTypeId, cardNumber, expiration));

        if (existingPaymentMethod != null)
        {
            this.DomainEvents?.Register(new BuyerAndPaymentMethodVerifiedDomainEvent(this, existingPaymentMethod, orderId));

            return existingPaymentMethod;
        }

        var paymentMethod = PaymentMethod.ForCard(
            Enumeration.From<CardType>(cardTypeId), alias, cardNumber, securityNumber, cardHolderName, expiration);
        this.paymentMethods.Add(paymentMethod);
        this.DomainEvents?.Register(
            new BuyerAndPaymentMethodVerifiedDomainEvent(this, paymentMethod, orderId));

        return paymentMethod;
    }
}