namespace Modules.Shopping.Domain;

using BridgingIT.DevKit.Domain;

public class CartItemAddedDomainEvent : DomainEventBase
{
    public CartItemAddedDomainEvent(CartId cartId, CartItem item)
    {
        this.CartId = cartId;
        this.Item = item;
    }

    public Guid CartId { get; }

    public CartItem Item { get; }
}