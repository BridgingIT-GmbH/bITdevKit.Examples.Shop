namespace Modules.Catalog.Application.Queries;

using BridgingIT.DevKit.Application.Entities;
using Modules.Catalog.Domain;

[GeneratedControllerApi]
public class ProductFindOneQuery : EntityFindOneQueryBase<Product>
{
    public ProductFindOneQuery(string entityId)
        : base(entityId)
    {
    }
}