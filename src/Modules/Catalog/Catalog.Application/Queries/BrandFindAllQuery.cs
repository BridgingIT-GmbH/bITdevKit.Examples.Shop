namespace Modules.Catalog.Application.Queries;

using BridgingIT.DevKit.Application.Entities;
using Modules.Catalog.Domain;

[GeneratedControllerApi(Module = "Catalog", Policy = CatalogPermissionSet.Brands.View, Parameters = new[] { nameof(IncludeDeleted) })]
public class BrandFindAllQuery : EntityFindAllQueryBase<Brand>
{
    public BrandFindAllQuery(int pageNumber = 1, int pageSize = int.MaxValue, string searchString = null, string orderBy = null, string include = null)
        : base(pageNumber, pageSize, searchString, orderBy, include)
    {
    }

    public bool IncludeDeleted { get; set; }
}