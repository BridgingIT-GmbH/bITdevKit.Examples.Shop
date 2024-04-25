namespace Modules.Ordering.Domain;

using BridgingIT.DevKit.Domain;

public class OrderSubmittedDomainEvent : DomainEventBase
{
    public OrderSubmittedDomainEvent(
        Order order, string userId, string userName, string email,
        int cardTypeId, string cardNumber, string cardSecurityNumber, string cardHolderName, DateTime cardExpiration)
    {
        this.Order = order;
        this.UserId = userId;
        this.UserName = userName;
        this.Email = email;
        this.CardTypeId = cardTypeId;
        this.CardNumber = cardNumber;
        this.CardSecurityNumber = cardSecurityNumber;
        this.CardHolderName = cardHolderName;
        this.CardExpiration = cardExpiration;
    }

    public Order Order { get; }

    public string UserId { get; }

    public string UserName { get; }

    public string Email { get; }

    public int CardTypeId { get; }

    public string CardNumber { get; }

    public string CardSecurityNumber { get; }

    public string CardHolderName { get; }

    public DateTime CardExpiration { get; }
}
