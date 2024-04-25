namespace Modules.Shopping.Application;

using Common;

public class ShoppingPermissionSet : PermissionSetBase
{
    [DisplayName("Carts")]
    [Description("Carts Permissions")]
    public static class Carts
    {
        public const string View = "Shopping.Carts.View";
        public const string Create = "Shopping.Carts.Create";
        public const string Edit = "Shopping.Carts.Edit";
        public const string Delete = "Shopping.Carts.Delete";
        public const string Export = "Shopping.Carts.Export";
        public const string Search = "Shopping.Carts.Search";
        public const string Import = "Shopping.Carts.Import";
    }
}
