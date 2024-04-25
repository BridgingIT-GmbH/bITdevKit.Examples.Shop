namespace Modules.Catalog.Application.Commands;

using BridgingIT.DevKit.Application.Entities;
using BridgingIT.DevKit.Common;
using BridgingIT.DevKit.Domain.Repositories;
using BridgingIT.DevKit.Domain.Specifications;
using Modules.Catalog.Domain;

public class BrandNameMustBeUniqueRule : IEntityCreateCommandRule<Brand>, IEntityUpdateCommandRule<Brand>
{
    private readonly IGenericRepository<Brand> repository;

    public BrandNameMustBeUniqueRule(IGenericRepository<Brand> repository)
    {
        this.repository = repository;
    }

    public string Message => "Brand name must be unique";

    async Task<bool> IEntityCommandRule<Brand>.IsSatisfiedAsync(Brand entity)
    {
        return await this.repository.FindOneAsync(
            new Specification<Brand>(e => e.Name == entity.Name)).AnyContext() == null;
    }
}