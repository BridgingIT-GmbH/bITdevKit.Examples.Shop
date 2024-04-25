namespace Modules.Ordering.Domain;

using BridgingIT.DevKit.Domain.Model;

public class OrderStatus : Enumeration
{
    public static OrderStatus Submitted = new(1, nameof(Submitted).ToLowerInvariant());
    public static OrderStatus AwaitingValidation = new(2, nameof(AwaitingValidation).ToLowerInvariant());
    public static OrderStatus StockConfirmed = new(3, nameof(StockConfirmed).ToLowerInvariant());
    public static OrderStatus Paid = new(4, nameof(Paid).ToLowerInvariant());
    public static OrderStatus Shipped = new(5, nameof(Shipped).ToLowerInvariant());
    public static OrderStatus Cancelled = new(6, nameof(Cancelled).ToLowerInvariant());

    public OrderStatus(int id, string name)
        : base(id, name)
    {
    }

    public static IEnumerable<OrderStatus> GetAll() => GetAll<OrderStatus>();
}
