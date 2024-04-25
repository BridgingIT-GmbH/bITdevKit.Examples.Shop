namespace Modules.Catalog.Application.Commands;

using BridgingIT.DevKit.Application.Entities;
using Modules.Catalog.Domain;

[GeneratedControllerApi(Module = "Catalog", Policy = CatalogPermissionSet.Brands.Delete)]
public class BrandDeleteCommand : EntityDeleteCommandBase<Brand>
{
    public BrandDeleteCommand(string id, string identity = null)
        : base(id, identity)
    {
    }
}
