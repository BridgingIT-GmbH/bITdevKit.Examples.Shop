namespace Modules.Catalog.Domain;

using System.Linq.Expressions;
using BridgingIT.DevKit.Domain.Specifications;

public class ProductFilterSpecification : Specification<Product>
{
    private readonly string searchString;

    public ProductFilterSpecification(string searchString)
    {
        this.searchString = searchString;
    }

    public override Expression<Func<Product, bool>> ToExpression()
    {
        return p => p.Name.Contains(this.searchString) || p.Description.Contains(this.searchString) || p.Barcode.Contains(this.searchString) || p.Brand.Name.Contains(this.searchString);
    }
}
