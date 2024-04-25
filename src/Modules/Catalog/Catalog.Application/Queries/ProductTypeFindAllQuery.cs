namespace Modules.Catalog.Application.Queries;

using BridgingIT.DevKit.Application.Entities;
using Modules.Catalog.Domain;

public class ProductTypeFindAllQuery : EntityFindAllQueryBase<ProductType>
{
    public ProductTypeFindAllQuery(int pageNumber = 1, int pageSize = int.MaxValue, string searchString = null, string orderBy = null, string include = null)
        : base(pageNumber, pageSize, searchString, orderBy, include)
    {
    }
}