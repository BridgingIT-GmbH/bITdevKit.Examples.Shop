namespace Modules.Ordering.Domain;

using BridgingIT.DevKit.Domain;
using BridgingIT.DevKit.Domain.Model;

public class OrderItem : Entity<Guid> // TODO: use strongly typed ids https://www.youtube.com/watch?v=z4SB5BkQX7M
{
    public OrderItem(Guid productId, string productName, decimal unitPrice, decimal discount = 0, string pictureUrl = null, int units = 1)
    {
        if (units <= 0) // TODO: use rules here
        {
            throw new BusinessRuleNotSatisfiedException("Invalid number of units");
        }

        if ((unitPrice * units) < discount)
        {
            throw new BusinessRuleNotSatisfiedException("The total of order item is lower than applied discount");
        }

        this.ProductId = productId;
        this.ProductName = productName;
        this.UnitPrice = unitPrice;
        this.Discount = discount;
        this.Units = units;
        this.PictureUrl = pictureUrl;
    }

    protected OrderItem()
    {
    }

    public Guid ProductId { get; private set; } // TODO: use strongly typed ids https://www.youtube.com/watch?v=z4SB5BkQX7M

    public string ProductName { get; private set; }

    public decimal UnitPrice { get; private set; }

    public decimal Discount { get; private set; }

    public int Units { get; private set; }

    public string PictureUrl { get; private set; }

    public void SetNewDiscount(decimal discount)
    {
        if (discount < 0) // TODO: use rules here
        {
            throw new BusinessRuleNotSatisfiedException("Discount is not valid");
        }

        this.Discount = discount;
    }

    public void AddUnits(int units)
    {
        if (units < 0) // TODO: use rules here
        {
            throw new BusinessRuleNotSatisfiedException("Invalid units");
        }

        this.Units += units;
    }
}
