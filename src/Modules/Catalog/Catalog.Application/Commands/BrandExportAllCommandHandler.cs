namespace Modules.Catalog.Application.Commands;

using BridgingIT.DevKit.Application.Collaboration;
using BridgingIT.DevKit.Application.Commands;
using BridgingIT.DevKit.Common;
using BridgingIT.DevKit.Domain.Repositories;
using BridgingIT.DevKit.Domain.Specifications;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Modules.Catalog.Domain;

public class BrandExportAllCommandHandler : CommandHandlerBase<BrandExportAllCommand, Result<string>>
{
    private readonly IGenericRepository<Brand> repository;
    private readonly IExcelInterchangeService excelService;
    private readonly IStringLocalizer<CatalogResources> localizer;

    public BrandExportAllCommandHandler(
        ILoggerFactory loggerFactory,
        IStringLocalizer<CatalogResources> localizer,
        IGenericRepository<Brand> repository,
        IExcelInterchangeService excelService)
        : base(loggerFactory)
    {
        EnsureArg.IsNotNull(repository, nameof(repository));
        EnsureArg.IsNotNull(excelService, nameof(excelService));
        EnsureArg.IsNotNull(localizer, nameof(localizer));

        this.repository = repository;
        this.excelService = excelService;
        this.localizer = localizer;
    }

    public override async Task<CommandResponse<Result<string>>> Process(BrandExportAllCommand request, CancellationToken cancellationToken)
    {
        var searchSpecification = new BrandFilterSpecification(request.SearchString);
        var notDeletedSpecification = new Specification<Brand>(e => e.AuditState.Deleted == null || !(bool)e.AuditState.Deleted);

        var entities = (await this.repository.FindAllAsync(
            specifications: new[] { searchSpecification, notDeletedSpecification },
            cancellationToken: cancellationToken).AnyContext()).SafeNull();

        return new CommandResponse<Result<string>>()
        {
            Result = Result<string>.Success(
                value: await this.excelService.ExportAsync(
                        entities,
                        mappers: new Dictionary<string, Func<Brand, object>>
                        {
                            { this.localizer["Id"], item => item.Id },
                            { this.localizer["Name"], item => item.Name },
                            { this.localizer["Description"], item => item.Description },
                        },
                        new ExcelInterchangeServiceOptions { SheetName = this.localizer["Brands"] }),
                this.localizer[CatalogResources.Brands_Exported, entities.Count()])
        };
    }
}
