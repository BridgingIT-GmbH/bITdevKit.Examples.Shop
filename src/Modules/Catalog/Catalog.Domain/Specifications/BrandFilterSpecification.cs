namespace Modules.Catalog.Domain;

using System.Linq.Expressions;
using BridgingIT.DevKit.Domain.Specifications;

public class BrandFilterSpecification : Specification<Brand>
{
    private readonly string searchString;

    public BrandFilterSpecification(string searchString)
    {
        this.searchString = searchString;
    }

    public override Expression<Func<Brand, bool>> ToExpression()
    {
        return p => p.Name.Contains(this.searchString) || p.Description.Contains(this.searchString);
    }
}