namespace Modules.Catalog.Application.Queries;

using BridgingIT.DevKit.Application.Entities;
using BridgingIT.DevKit.Domain.Repositories;
using BridgingIT.DevKit.Domain.Specifications;
using Microsoft.Extensions.Logging;
using Modules.Catalog.Domain;
using System.Collections.Generic;

public class BrandFindAllQueryHandler : EntityFindAllQueryHandlerBase<BrandFindAllQuery, Brand>
{
    public BrandFindAllQueryHandler(ILoggerFactory loggerFactory, IGenericRepository<Brand> repository)
        : base(loggerFactory, repository)
    {
        //this.AddSpecification<EntityNotDeletedSpecification<Brand>>(); // never include deleted entities
        this.AddSpecification(r => new BrandFilterSpecification(r.SearchString));
    }

    public override IEnumerable<ISpecification<Brand>> AddSpecifications(BrandFindAllQuery request)
    {
        if (!request.IncludeDeleted) // optionally include deleted entities
        {
            return new[]
            {
                new EntityNotDeletedSpecification<Brand>()
            };
        }

        return Enumerable.Empty<ISpecification<Brand>>();
    }
}