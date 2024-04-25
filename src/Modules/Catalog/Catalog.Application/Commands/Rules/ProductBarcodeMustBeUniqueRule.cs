namespace Modules.Catalog.Application.Commands;

using BridgingIT.DevKit.Application.Entities;
using BridgingIT.DevKit.Common;
using BridgingIT.DevKit.Domain.Repositories;
using BridgingIT.DevKit.Domain.Specifications;
using Modules.Catalog.Domain;

public class ProductBarcodeMustBeUniqueRule : IEntityCreateCommandRule<Product>, IEntityUpdateCommandRule<Product>
{
    private readonly IGenericRepository<Product> repository;

    public ProductBarcodeMustBeUniqueRule(IGenericRepository<Product> repository)
    {
        this.repository = repository;
    }

    public string Message => "Product barcode must be unique";

    async Task<bool> IEntityCommandRule<Product>.IsSatisfiedAsync(Product entity)
    {
        return await this.repository.FindOneAsync(
            new Specification<Product>(e => e.Barcode == entity.Barcode)).AnyContext() == null;
    }
}