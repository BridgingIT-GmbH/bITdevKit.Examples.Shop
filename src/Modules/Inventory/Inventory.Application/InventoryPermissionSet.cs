namespace Modules.Inventory.Application;

using Common;

public class InventoryPermissionSet : PermissionSetBase
{
    [DisplayName("Stocks")]
    [Description("Stocks Permissions")]
    public static class Stocks
    {
        public const string View = "Inventory.Stocks.View";
        public const string Create = "Inventory.Stocks.Create";
        public const string Edit = "Inventory.Stocks.Edit";
        public const string Delete = "Inventory.Stocks.Delete";
        public const string Export = "Inventory.Stocks.Export";
        public const string Search = "Inventory.Stocks.Search";
        public const string Import = "Inventory.Stocks.Import";
    }
}
