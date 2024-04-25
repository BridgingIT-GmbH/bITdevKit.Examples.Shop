namespace Modules.Catalog.Application.Queries;

using BridgingIT.DevKit.Application.Entities;
using BridgingIT.DevKit.Domain.Repositories;
using Microsoft.Extensions.Logging;
using Modules.Catalog.Domain;

public class ProductFindAllQueryHandler : EntityFindAllQueryHandlerBase<ProductFindAllQuery, Product>
{
    public ProductFindAllQueryHandler(ILoggerFactory loggerFactory, IGenericRepository<Product> repository)
        : base(loggerFactory, repository)
    {
        this.AddSpecification<EntityNotDeletedSpecification<Product>>();
        this.AddSpecification(query => new ProductFilterSpecification(query.SearchString)); // specification from domain
    }
}