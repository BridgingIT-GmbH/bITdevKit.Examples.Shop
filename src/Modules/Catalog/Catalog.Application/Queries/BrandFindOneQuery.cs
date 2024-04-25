namespace Modules.Catalog.Application.Queries;

using BridgingIT.DevKit.Application.Entities;
using Modules.Catalog.Domain;

[GeneratedControllerApi(Module = "Catalog", Policy = CatalogPermissionSet.Brands.View)]
public class BrandFindOneQuery : EntityFindOneQueryBase<Brand>
{
    public BrandFindOneQuery(string entityId)
        : base(entityId)
    {
    }
}