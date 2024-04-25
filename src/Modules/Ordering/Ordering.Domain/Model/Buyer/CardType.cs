namespace Modules.Ordering.Domain;

using BridgingIT.DevKit.Domain.Model;

public class CardType : Enumeration
{
    public static CardType Amex = new(1, "Amex");
    public static CardType Visa = new(2, "Visa");
    public static CardType MasterCard = new(3, "MasterCard");

    public CardType(int id, string name)
        : base(id, name)
    {
    }

    public static IEnumerable<CardType> GetAll() => GetAll<CardType>();
}
