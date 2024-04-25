namespace Modules.Catalog.Application.Commands;

using BridgingIT.DevKit.Application.Entities;
using BridgingIT.DevKit.Domain.Repositories;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Modules.Catalog.Domain;

public class BrandCreateCommandHandler : EntityCreateCommandHandlerBase<BrandCreateCommand, Brand>
{
    public BrandCreateCommandHandler(
        ILoggerFactory loggerFactory,
        IGenericRepository<Brand> repository,
        IEnumerable<IEntityCreateCommandRule<Brand>> rules = null,
        IStringLocalizer<CatalogResources> localizer = null)
        : base(loggerFactory, repository, rules, localizer)
    {
        this.AddRule(new BrandNameMustBeUniqueRule(repository)); // more complex rules (with dependencies) can be injected through the ctor (rule argument)
    }
}