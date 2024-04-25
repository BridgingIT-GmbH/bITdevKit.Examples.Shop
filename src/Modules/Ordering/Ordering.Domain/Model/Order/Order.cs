namespace Modules.Ordering.Domain;

using BridgingIT.DevKit.Domain;
using BridgingIT.DevKit.Domain.Model;

public class Order : AggregateRoot<Guid> // TODO: use strongly typed ids https://www.youtube.com/watch?v=z4SB5BkQX7M
{
    private readonly List<OrderItem> items = new();

    protected Order()
    {
    }

    public DateTimeOffset CreatedDate { get; private set; }

    public Guid? BuyerId { get; private set; } // TODO: use strongly typed ids https://www.youtube.com/watch?v=z4SB5BkQX7M

    public Guid? PaymentMethodId { get; private set; } // TODO: use strongly typed ids https://www.youtube.com/watch?v=z4SB5BkQX7M

    public string Description { get; private set; }

    public Address Address { get; private set; }

    public IReadOnlyCollection<OrderItem> Items => this.items;

    public OrderStatus OrderStatus { get; private set; }

    public static Order SubmitForBuyer(
            string userId, string userName, string email, Address address,
            int cardTypeId, string cardNumber, string cardSecurityNumber, string cardHolderName, DateTime cardExpiration,
            Guid? buyerId = null, Guid? paymentMethodId = null)
    {
        var entity = new Order()
        {
            BuyerId = buyerId,
            OrderStatus = OrderStatus.Submitted,
            CreatedDate = DateTime.UtcNow,
            Address = address, // store also on buyer?
        };

        entity.DomainEvents.Register(
            new OrderSubmittedDomainEvent(
                    entity, userId, userName, email, cardTypeId, cardNumber,
                    cardSecurityNumber, cardHolderName, cardExpiration));

        return entity;
    }

    public void SetPaymentId(Guid id)
    {
        this.PaymentMethodId = id;
    }

    public void SetBuyerId(Guid id)
    {
        this.BuyerId = id;
    }

    public void AddOrderItem(Guid productId, string productName, decimal unitPrice, decimal discount, string pictureUrl, int units = 1)
    {
        var item = this.items.SingleOrDefault(o => o.ProductId == productId);
        if (item != null)
        {
            //if previous line exist modify it with higher discount and units..
            if (discount > item.Discount)
            {
                item.SetNewDiscount(discount);
            }

            item.AddUnits(units);
        }
        else
        {
            //add validated new order item
            var orderItem = new OrderItem(productId, productName, unitPrice, discount, pictureUrl, units);
            this.items.Add(orderItem);
        }
    }

    public void SetAwaitingValidationStatus()
    {
        if (this.OrderStatus == OrderStatus.Submitted)
        {
            this.DomainEvents.Register(new OrderStatusChangedToAwaitingValidationDomainEvent(this.Id, this.items));
            this.OrderStatus = OrderStatus.AwaitingValidation;
        }
    }

    public void SetStockConfirmedStatus()
    {
        if (this.OrderStatus == OrderStatus.AwaitingValidation)
        {
            this.DomainEvents.Register(new OrderStatusChangedToStockConfirmedDomainEvent(this.Id));
            this.OrderStatus = OrderStatus.StockConfirmed;
            this.Description = "All the items were confirmed with available stock.";
        }
    }

    public void SetPaidStatus()
    {
        if (this.OrderStatus == OrderStatus.StockConfirmed)
        {
            this.DomainEvents.Register(new OrderStatusChangedToPaidDomainEvent(this.Id, this.items));

            this.OrderStatus = OrderStatus.Paid;
            this.Description = "The payment was performed at a simulated \"American Bank checking bank account ending on XX35071\"";
        }
    }

    public void SetShippedStatus()
    {
        if (this.OrderStatus != OrderStatus.Paid)
        {
            this.StatusChangeException(OrderStatus.Shipped);
        }

        this.OrderStatus = OrderStatus.Shipped;
        this.Description = "The order was shipped.";
        this.DomainEvents.Register(new OrderShippedDomainEvent(this));
    }

    public void SetCancelledStatus()
    {
        if (this.OrderStatus == OrderStatus.Paid ||
            this.OrderStatus == OrderStatus.Shipped)
        {
            this.StatusChangeException(OrderStatus.Cancelled);
        }

        this.OrderStatus = OrderStatus.Cancelled;
        this.Description = "The order was cancelled.";
        this.DomainEvents.Register(new OrderCancelledDomainEvent(this));
    }

    public void SetCancelledStatusWhenStockIsRejected(IEnumerable<Guid> orderStockRejectedItems)
    {
        if (this.OrderStatus == OrderStatus.AwaitingValidation)
        {
            this.OrderStatus = OrderStatus.Cancelled;

            var itemsStockRejectedProductNames = this.Items
                .Where(i => orderStockRejectedItems.Contains(i.ProductId))
                .Select(i => i.ProductName);

            var itemsStockRejectedDescription = string.Join(", ", itemsStockRejectedProductNames);
            this.Description = $"The product items don't have stock: ({itemsStockRejectedDescription}).";
        }
    }

    public decimal GetTotal()
    {
        return this.Items.Sum(o => o.Units * o.UnitPrice);
    }

    private void StatusChangeException(OrderStatus orderStatusToChange)
    {
        throw new BusinessRuleNotSatisfiedException($"Is not possible to change the order status from {this.OrderStatus.Name} to {orderStatusToChange.Name}.");
    }
}
