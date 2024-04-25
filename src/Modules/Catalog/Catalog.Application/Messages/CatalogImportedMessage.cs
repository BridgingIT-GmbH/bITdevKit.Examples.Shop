namespace Modules.Catalog.Application;

using BridgingIT.DevKit.Application.Messaging;

public class CatalogImportedMessage : MessageBase
{
    public string Source { get; set; }

    public int BrandCount { get; set; }

    public int ProductCount { get; set; }
}