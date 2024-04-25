namespace Modules.Catalog.Application.Integration;

using System.Linq;
using Modules.Catalog.Domain;

public interface IODataAdapter
{
    IQueryable<Product> Products { get; }

    IQueryable<Brand> Brands { get; }
}