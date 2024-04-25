namespace Modules.Inventory.Application;

using BridgingIT.DevKit.Application.Commands;

public class InventoryImportCommand : CommandRequestBase
{
    public int BrandCount { get; set; }

    public int ProductCount { get; set; }
}
