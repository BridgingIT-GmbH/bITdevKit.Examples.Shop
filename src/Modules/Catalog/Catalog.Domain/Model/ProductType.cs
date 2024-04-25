namespace Modules.Catalog.Domain;

using BridgingIT.DevKit.Domain.Model;

public class ProductType : Entity<Guid>
{
    //private readonly List<Product> products = new();

    public string Name { get; set; }

    public string Description { get; set; }

    //public IReadOnlyCollection<Product> Products => this.products;
}