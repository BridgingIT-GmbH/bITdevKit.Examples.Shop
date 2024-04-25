namespace Modules.Catalog.Application.Commands;

using BridgingIT.DevKit.Application.Entities;
using BridgingIT.DevKit.Domain.Repositories;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Modules.Catalog.Domain;

public class BrandDeleteCommandHandler : EntityDeleteCommandHandlerBase<BrandDeleteCommand, Brand>
{
    public BrandDeleteCommandHandler(
    ILoggerFactory loggerFactory,
    IGenericRepository<Brand> repository,
    IEnumerable<IEntityDeleteCommandRule<Brand>> rules = null, // TODO: DI injects rule BrandWithProductsCannotBeDeleted (while needs a repo to check in db)
    IStringLocalizer<CatalogResources> localizer = null)
    : base(loggerFactory, repository, rules, localizer)
    {
        this.AddRule<EntityDeactivatedCannotBeDeletedRule<Brand>>(); // more complex rules (with dependencies) can be injected through the ctor (rule argument)
    }
}