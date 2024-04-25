namespace Modules.Inventory.Domain;

using System;
using BridgingIT.DevKit.Domain;
using BridgingIT.DevKit.Domain.Model;

public class Stock : AggregateRoot<Guid> // TODO: use strongly typed ids https://www.youtube.com/watch?v=z4SB5BkQX7M
{
    public string SKU { get; set; }

    // Quantity in stock
    public int AvailableQuantity { get; set; }

    // Quantity ordered
    public int RestockQuantity { get; set; }

    // Available stock at which we should reorder
    public int RestockThreshold { get; set; }

    // Maximum number of units that can be in-stock at any time (due to physicial/logistical constraints in warehouses)
    public int MaxStockThreshold { get; set; }

    /// <summary>
    /// True if item is on reorder
    /// </summary>
    public bool OnRestock { get; set; }

    public State State { get; set; } = new State();

    /// <summary>
    /// <para>
    /// Decrements the quantity of a particular item in inventory and ensures the restockThreshold hasn't
    /// been breached. If so, a RestockRequest is generated in CheckThreshold.
    /// </para>
    /// <para>
    /// If there is sufficient stock of an item, then the integer returned at the end of this call should be the same as quantityDesired.
    /// In the event that there is not sufficient stock available, the method will remove whatever stock is available and return that quantity to the client.
    /// In this case, it is the responsibility of the client to determine if the amount that is returned is the same as quantityDesired.
    /// It is invalid to pass in a negative number.
    /// </para>
    /// </summary>
    /// <param name="quantity"></param>
    /// <returns>int: Returns the number actually removed from stock. </returns>
    ///
    public int RemoveStock(int quantity)
    {
        if (this.AvailableQuantity == 0)
        {
            throw new BusinessRuleNotSatisfiedException($"Empty stock, product {this.SKU} is sold out");
        }

        if (quantity <= 0)
        {
            throw new BusinessRuleNotSatisfiedException($"Product {this.SKU} quantity desired should be greater than zero");
        }

        var removed = Math.Min(quantity, this.AvailableQuantity);
        this.AvailableQuantity -= removed;

        return removed;
    }

    /// <summary>
    /// Increments the quantity of a particular item in inventory.
    /// <param name="quantity"></param>
    /// <returns>int: Returns the quantity that has been added to stock</returns>
    /// </summary>
    public int AddStock(int quantity)
    {
        var original = this.AvailableQuantity;

        // The quantity that the client is trying to add to stock is greater than what can be physically accommodated in the Warehouse
        if ((this.AvailableQuantity + quantity) > this.MaxStockThreshold)
        {
            // For now, this method only adds new units up maximum stock threshold. In an expanded version of this application, we
            //could include tracking for the remaining units and store information about overstock elsewhere.
            this.AvailableQuantity += this.MaxStockThreshold - this.AvailableQuantity;
        }
        else
        {
            this.AvailableQuantity += quantity;
        }

        this.OnRestock = false;
        this.RestockQuantity = 0;

        return this.AvailableQuantity - original;
    }
}
