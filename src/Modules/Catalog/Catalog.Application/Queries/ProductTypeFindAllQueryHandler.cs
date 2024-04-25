namespace Modules.Catalog.Application.Queries;

using BridgingIT.DevKit.Application.Entities;
using BridgingIT.DevKit.Domain.Repositories;
using Microsoft.Extensions.Logging;
using Modules.Catalog.Domain;

public class ProductTypeFindAllQueryHandler : EntityFindAllQueryHandlerBase<ProductTypeFindAllQuery, ProductType>
{
    public ProductTypeFindAllQueryHandler(ILoggerFactory loggerFactory, IGenericRepository<ProductType> repository)
        : base(loggerFactory, repository)
    {
        this.AddSpecification(request => new ProductTypeFilterSpecification(request.SearchString));
    }
}