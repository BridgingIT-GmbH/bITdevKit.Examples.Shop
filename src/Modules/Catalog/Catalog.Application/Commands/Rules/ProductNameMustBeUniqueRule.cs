namespace Modules.Catalog.Application.Commands;

using BridgingIT.DevKit.Application.Entities;
using BridgingIT.DevKit.Common;
using BridgingIT.DevKit.Domain.Repositories;
using BridgingIT.DevKit.Domain.Specifications;
using Modules.Catalog.Domain;

public class ProductNameMustBeUniqueRule : IEntityCreateCommandRule<Product>, IEntityUpdateCommandRule<Product>
{
    private readonly IGenericRepository<Product> repository;

    public ProductNameMustBeUniqueRule(IGenericRepository<Product> repository)
    {
        this.repository = repository;
    }

    public string Message => "Product name must be unique";

    async Task<bool> IEntityCommandRule<Product>.IsSatisfiedAsync(Product entity)
    {
        return await this.repository.FindOneAsync(
            new Specification<Product>(e => e.Name == entity.Name)).AnyContext() == null;
    }
}