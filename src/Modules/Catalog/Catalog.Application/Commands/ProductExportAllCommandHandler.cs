namespace Modules.Catalog.Application.Commands;

using BridgingIT.DevKit.Application.Collaboration;
using BridgingIT.DevKit.Application.Commands;
using BridgingIT.DevKit.Common;
using BridgingIT.DevKit.Domain.Repositories;
using BridgingIT.DevKit.Domain.Specifications;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Modules.Catalog.Domain;

public class ProductExportAllCommandHandler : CommandHandlerBase<ProductExportAllCommand, Result<string>>
{
    private readonly IStringLocalizer<CatalogResources> localizer;
    private readonly IGenericRepository<Product> repository;
    private readonly IExcelInterchangeService excelService;

    public ProductExportAllCommandHandler(
        ILoggerFactory loggerFactory,
        IStringLocalizer<CatalogResources> localizer,
        IGenericRepository<Product> repository,
        IExcelInterchangeService excelService)
        : base(loggerFactory)
    {
        EnsureArg.IsNotNull(localizer, nameof(localizer));
        EnsureArg.IsNotNull(repository, nameof(repository));
        EnsureArg.IsNotNull(excelService, nameof(excelService));
        this.localizer = localizer;
        this.repository = repository;
        this.excelService = excelService;
    }

    public override async Task<CommandResponse<Result<string>>> Process(ProductExportAllCommand request, CancellationToken cancellationToken)
    {
        var searchSpecification = new ProductFilterSpecification(request.SearchString);
        var notDeletedSpecification = new Specification<Product>(e => e.AuditState.Deleted == null || !(bool)e.AuditState.Deleted);

        var entities = (await this.repository.FindAllAsync(
            specifications: new[] { searchSpecification, notDeletedSpecification },
            cancellationToken: cancellationToken).AnyContext()).SafeNull();

        return new CommandResponse<Result<string>>()
        {
            Result = Result<string>.Success(
                value: await this.excelService.ExportAsync(
                    entities,
                    mappers: new Dictionary<string, Func<Product, object>>
                    {
                        { this.localizer["Id"], item => item.Id },
                        { this.localizer["Name"], item => item.Name },
                        { this.localizer["Barcode"], item => item.Barcode },
                        { this.localizer["Description"], item => item.Description },
                        { this.localizer["Rating"], item => item.Rating },
                        { this.localizer["Price"], item => item.Price },
                        { this.localizer["Size"], item => item.Size },
                        { this.localizer["Sku"], item => item.Sku },
                        { this.localizer["BrandId"], item => item.BrandId},
                        { this.localizer["TypeId"], item => item.TypeId}
                    },
                    new ExcelInterchangeServiceOptions { SheetName = this.localizer["Products"] }),
                this.localizer[CatalogResources.Products_Exported, entities.Count()])
        };
    }
}
