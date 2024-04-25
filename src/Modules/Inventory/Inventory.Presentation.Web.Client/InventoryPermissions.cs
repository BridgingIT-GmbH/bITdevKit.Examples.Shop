namespace Modules.Inventory.Presentation.Web.Client;

using Common;

public class InventoryPermissions : IPermissions
{
    public static class Stocks
    {
        public const string View = "Inventory.Stocks.View";
        public const string Create = "Inventory.Stocks.Create";
        public const string Edit = "Inventory.Stocks.Edit";
        public const string Delete = "Inventory.Stocks.Delete";
        public const string Export = "Inventory.Stocks.Export";
        public const string Search = "Inventory.Stocks.Search";
    }
}
