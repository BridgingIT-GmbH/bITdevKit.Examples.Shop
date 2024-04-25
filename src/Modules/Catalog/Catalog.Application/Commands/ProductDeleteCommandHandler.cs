namespace Modules.Catalog.Application.Commands;

using BridgingIT.DevKit.Application.Entities;
using BridgingIT.DevKit.Domain.Repositories;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Modules.Catalog.Domain;

public class ProductDeleteCommandHandler : EntityDeleteCommandHandlerBase<ProductDeleteCommand, Product>
{
    public ProductDeleteCommandHandler(
    ILoggerFactory loggerFactory,
    IGenericRepository<Product> repository,
    IEnumerable<IEntityDeleteCommandRule<Product>> rules = null,
    IStringLocalizer<CatalogResources> localizer = null)
    : base(loggerFactory, repository, rules, localizer)
    {
        this.AddRule<EntityDeactivatedCannotBeDeletedRule<Product>>(); // more complex rules (with dependencies) can be injected through the ctor (rule argument)
    }
}