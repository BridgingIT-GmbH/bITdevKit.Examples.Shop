namespace Modules.Catalog.Application.Commands;

using BridgingIT.DevKit.Application.Entities;
using BridgingIT.DevKit.Domain.Repositories;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Modules.Catalog.Domain;

public class ProductCreateCommandHandler : EntityCreateCommandHandlerBase<ProductCreateCommand, Product>
{
    public ProductCreateCommandHandler(
        ILoggerFactory loggerFactory,
        IGenericRepository<Product> repository,
        IEnumerable<IEntityCreateCommandRule<Product>> rules = null,
        IStringLocalizer<CatalogResources> localizer = null)
        : base(loggerFactory, repository, rules, localizer)
    {
        this.AddRule(new ProductNameMustBeUniqueRule(repository)); // more complex rules (with dependencies) can be injected through the ctor (rule argument)
        this.AddRule(new ProductSkuMustBeUniqueRule(repository));
        this.AddRule(new ProductBarcodeMustBeUniqueRule(repository));
    }
}