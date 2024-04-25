namespace Modules.Catalog.Application;

using Modules.Catalog.Domain;

public interface ICatalogDataAdapter
{
    IAsyncEnumerable<Brand> GetBrands(CancellationToken cancellationToken = default);

    IAsyncEnumerable<Product> GetProducts(CancellationToken cancellationToken = default);
}