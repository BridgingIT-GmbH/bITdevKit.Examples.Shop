namespace Modules.Catalog.Application.Queries;

using BridgingIT.DevKit.Application.Entities;
using BridgingIT.DevKit.Domain.Repositories;
using Microsoft.Extensions.Logging;
using Modules.Catalog.Domain;

public class ProductFindOneQueryHandler : EntityFindOneQueryHandlerBase<ProductFindOneQuery, Product>
{
    public ProductFindOneQueryHandler(ILoggerFactory loggerFactory, IGenericRepository<Product> repository)
        : base(loggerFactory, repository)
    {
        this.AddSpecification<EntityNotDeletedSpecification<Product>>(); // specification from domain
    }
}