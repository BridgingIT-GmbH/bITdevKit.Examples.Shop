namespace Modules.Catalog.Application.Commands;

using BridgingIT.DevKit.Application.Entities;
using BridgingIT.DevKit.Domain.Repositories;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Modules.Catalog.Domain;

public class BrandUpdateCommandHandler : EntityUpdateCommandHandlerBase<BrandUpdateCommand, Brand>
{
    public BrandUpdateCommandHandler(
        ILoggerFactory loggerFactory,
        IGenericRepository<Brand> repository,
        IEnumerable<IEntityUpdateCommandRule<Brand>> rules = null,
        IStringLocalizer<CatalogResources> localizer = null)
        : base(loggerFactory, repository, rules, localizer)
    {
        this.AddRule<EntityDeactivatedCannotBeUpdatedRule<Brand>>();
        //this.AddRule(new BrandNameMustBeUniqueRule(repository)); // more complex rules (with dependencies) can be injected through the ctor (rule argument)
    }
}