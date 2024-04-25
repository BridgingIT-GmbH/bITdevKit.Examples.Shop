namespace Modules.Ordering.Domain;

using BridgingIT.DevKit.Domain;
using BridgingIT.DevKit.Domain.Model;

public class PaymentMethod : Entity<Guid> // TODO: use strongly typed ids https://www.youtube.com/watch?v=z4SB5BkQX7M
{
    protected PaymentMethod()
    {
    }

    public string Alias { get; private set; }

    public string CardNumber { get; private set; }

    public string SecurityNumber { get; private set; }

    public string CardHolderName { get; private set; }

    public DateTime Expiration { get; private set; }

    public CardType CardType { get; private set; }

    public static PaymentMethod ForCard(
        CardType cardType, string alias, string cardNumber, string securityNumber, string cardHolderName, DateTime expiration)
    {
        // TODO: businessrules/guards
        if (expiration < DateTime.UtcNow)
        {
            throw new BusinessRuleNotSatisfiedException(nameof(expiration));
        }

        var entity = new PaymentMethod
        {
            CardType = cardType,
            CardNumber = !string.IsNullOrWhiteSpace(cardNumber) ? cardNumber : throw new BusinessRuleNotSatisfiedException(nameof(cardNumber)),
            SecurityNumber = !string.IsNullOrWhiteSpace(securityNumber) ? securityNumber : throw new BusinessRuleNotSatisfiedException(nameof(securityNumber)),
            CardHolderName = !string.IsNullOrWhiteSpace(cardHolderName) ? cardHolderName : throw new BusinessRuleNotSatisfiedException(nameof(cardHolderName)),
            Alias = alias,
            Expiration = expiration
        };

        return entity;
    }

    public bool IsEqualTo(int cardTypeId, string cardNumber, DateTime expiration)
    {
        return this.CardType?.Id == cardTypeId
            && this.CardNumber == cardNumber
            && this.Expiration == expiration;
    }
}
