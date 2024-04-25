namespace Modules.Shopping.Domain;

using BridgingIT.DevKit.Domain;
using BridgingIT.DevKit.Domain.Model;

public class CartItem : Entity<CartItemId>
{
    private CartItem()
    {
    }

    public CartProduct Product { get; private set; }

    public int Quantity { get; private set; }

    public Amount TotalPrice { get; private set; }

    public static CartItem For(CartProduct product, int quantity)
    {
        Check.Throw(new IBusinessRule[]
        {
            new IsValidCartProduct(product)
        });

        // TODO: check quantity < 1 ?

        return new()
        {
            Id = new CartItemId(),
            Product = product,
            Quantity = quantity,
            TotalPrice = quantity * product.UnitPrice.Amount
        };
    }

    public void Update(CartProduct product, int quantity)
    {
        Check.Throw(new IBusinessRule[]
        {
            new IsValidCartProduct(product)
        });

        // TODO: check quantity < 1 ?

        this.Product = product;
        this.Quantity += quantity;
        this.TotalPrice = this.Quantity * this.Product.UnitPrice;
    }
}

public class CartItemId : GuidTypedId
{
    public CartItemId()
        : this(Guid.NewGuid())
    {
    }

    public CartItemId(Guid value)
        : base(value)
    {
    }

    public static implicit operator CartItemId(Guid id) => new(id);

    public override string ToString() => this.Value.ToString();
}