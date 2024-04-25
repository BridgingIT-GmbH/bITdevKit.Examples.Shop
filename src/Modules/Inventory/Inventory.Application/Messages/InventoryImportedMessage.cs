namespace Modules.Inventory.Application;

using BridgingIT.DevKit.Application.Messaging;

public class InventoryImportedMessage : MessageBase
{
    public string Source { get; set; }

    public int ProductCount { get; set; }
}
