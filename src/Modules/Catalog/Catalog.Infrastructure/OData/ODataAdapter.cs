namespace Modules.Catalog.Infrastructure.OData;

using System.Linq;
using Modules.Catalog.Application.Integration;
using Modules.Catalog.Domain;
using Modules.Catalog.Infrastructure.EntityFramework;

public class ODataAdapter : IODataAdapter
{
    private readonly CatalogDbContext context;

    public ODataAdapter(CatalogDbContext context)
    {
        this.context = context;
    }

    public IQueryable<Product> Products =>
        this.context.Products.AsQueryable();

    public IQueryable<Brand> Brands =>
        this.context.Brands.AsQueryable();
}
