namespace Modules.Shopping.Domain;

using System.Collections.Generic;
using BridgingIT.DevKit.Domain.Model;

public class CartProduct : ValueObject
{
    private CartProduct()
    {
    }

    private CartProduct(string sku, string name, Amount unitPrice)
    {
        this.SKU = sku;
        this.Name = name;
        this.UnitPrice = unitPrice;
    }

    public string SKU { get; private set; } // needed by EF to set the value when loaded from DB

    public string Name { get; private set; } // needed by EF to set the value when loaded from DB

    public Amount UnitPrice { get; private set; } // needed by EF to set the value when loaded from DB

    public static CartProduct For(string sku, string name, Amount unitPrice)
    {
        // TODO: check some rules before creating new product instance?
        return new CartProduct(sku, name, unitPrice);
    }

    protected override IEnumerable<object> GetAtomicValues()
    {
        throw new NotImplementedException();
    }
}