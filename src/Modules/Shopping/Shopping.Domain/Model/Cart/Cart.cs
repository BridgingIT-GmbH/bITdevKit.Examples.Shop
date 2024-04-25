namespace Modules.Shopping.Domain;

using BridgingIT.DevKit.Common;
using BridgingIT.DevKit.Domain;
using BridgingIT.DevKit.Domain.Model;

public class Cart : AuditableAggregateRoot<CartId>
{
    private readonly List<CartItem> items = new();

    private Cart()
    {
    }

    public string Identity { get; init; } // userId

    public Amount TotalPrice { get; private set; } = Amount.Zero;

    public IReadOnlyList<CartItem> Items => this.items.AsReadOnly();

    public static Cart ForUser(string identity)
    {
        return new Cart
        {
            Id = new CartId(),
            Identity = identity
        };
    }

    public CartItem FindItem(CartItemId itemId)
    {
        EnsureArg.IsNotNull(itemId, nameof(itemId));

        return this.items.Find(i => i.Id == itemId);
    }

    public CartItem FindItem(CartProduct product)
    {
        EnsureArg.IsNotNullOrEmpty(product.SKU, nameof(product.SKU));

        return this.items.Find(i => i.Product.SKU.SafeEquals(product.SKU));
    }

    public CartItem AddItem(CartProduct product, int quantity)
    {
        Check.Throw(new IBusinessRule[]
        {
            new IsValidCartProduct(product)
        });

        var item = this.FindItem(product);
        if (item == null)
        {
            item = CartItem.For(product, quantity);
            this.items.Add(item);
        }
        else
        {
            item.Update(product, quantity);
        }

        this.RecalculatePrices();
        this.DomainEvents.Register(new CartItemAddedDomainEvent(this.Id, item));

        return item;
    }

    public void RemoveItems()
    {
        this.items.Clear();
        this.RecalculatePrices();
    }

    public void RemoveItem(CartItem item)
    {
        this.items.Remove(item);
        this.RecalculatePrices();
    }

    private void RecalculatePrices()
    {
        this.TotalPrice = this.items.Select(i => i.Quantity * i.Product.UnitPrice).Sum();
    }

    //RemoveItem(s)
}

public class CartId : GuidTypedId
{
    public CartId()
        : this(Guid.NewGuid())
    {
    }

    public CartId(Guid value)
        : base(value)
    {
    }

    public static implicit operator CartId(Guid id) => new(id);

    public override string ToString() => this.Value.ToString();
}