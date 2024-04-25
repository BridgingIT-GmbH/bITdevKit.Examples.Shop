namespace Modules.Catalog.Domain;

using System.Linq.Expressions;
using BridgingIT.DevKit.Domain.Specifications;

public class ProductTypeFilterSpecification : Specification<ProductType>
{
    private readonly string searchString;

    public ProductTypeFilterSpecification(string searchString)
    {
        this.searchString = searchString;
    }

    public override Expression<Func<ProductType, bool>> ToExpression()
    {
        return p => p.Name.Contains(this.searchString) || p.Description.Contains(this.searchString);
    }
}
