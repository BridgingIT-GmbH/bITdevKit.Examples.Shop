namespace Modules.Shopping.Presentation.Web.Client;

using Common;

public class ShoppingPermissions : IPermissions
{
    public static class Carts
    {
        public const string View = "Shopping.Carts.View";
        public const string Create = "Shopping.Carts.Create";
        public const string Edit = "Shopping.Carts.Edit";
        public const string Delete = "Shopping.Carts.Delete";
        public const string Export = "Shopping.Carts.Export";
        public const string Search = "Shopping.Carts.Search";
    }
}