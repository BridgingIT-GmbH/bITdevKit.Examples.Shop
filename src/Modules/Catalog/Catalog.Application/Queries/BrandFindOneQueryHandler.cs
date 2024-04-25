namespace Modules.Catalog.Application.Queries;

using BridgingIT.DevKit.Application.Entities;
using BridgingIT.DevKit.Domain.Repositories;
using Microsoft.Extensions.Logging;
using Modules.Catalog.Domain;

public class BrandFindOneQueryHandler : EntityFindOneQueryHandlerBase<BrandFindOneQuery, Brand>
{
    public BrandFindOneQueryHandler(ILoggerFactory loggerFactory, IGenericRepository<Brand> repository)
        : base(loggerFactory, repository)
    {
        this.AddSpecification<EntityNotDeletedSpecification<Brand>>();
    }
}