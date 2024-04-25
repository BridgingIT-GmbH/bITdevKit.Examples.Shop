namespace Modules.Ordering.Application;

using Common;

public class OrderingPermissionSet : PermissionSetBase
{
    [DisplayName("Orders")]
    [Description("Orders Permissions")]
    public static class Orders
    {
        public const string View = "Ordering.Orders.View";
        public const string Create = "Ordering.Orders.Create";
        public const string Edit = "Ordering.Orders.Edit";
        public const string Delete = "Ordering.Orders.Delete";
        public const string Export = "Ordering.Orders.Export";
        public const string Search = "Ordering.Orders.Search";
        public const string Import = "Ordering.Orders.Import";
    }
}
