namespace Modules.Ordering.Presentation.Web.Client;

using Common;

public class OrderingPermissions : IPermissions
{
    public static class Orders
    {
        public const string View = "Ordering.Oders.View";
        public const string Create = "Ordering.Oders.Create";
        public const string Edit = "Ordering.Oders.Edit";
        public const string Delete = "Ordering.Oders.Delete";
        public const string Export = "Ordering.Oders.Export";
        public const string Search = "Ordering.Oders.Search";
    }
}
