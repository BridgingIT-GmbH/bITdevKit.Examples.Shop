namespace Modules.Catalog.Application;

using System.Diagnostics;
using BridgingIT.DevKit.Application.Messaging;

[DebuggerDisplay("SKU={SKU}")]
public class ProductImportedMessage : MessageBase
{
    public string SKU { get; set; }

    public string Name { get; set; }

    public decimal Price { get; set; }
}