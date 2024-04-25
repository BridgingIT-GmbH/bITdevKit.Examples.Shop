namespace Modules.Catalog.Application.Commands;

using System.Data;
using BridgingIT.DevKit.Application.Collaboration;
using BridgingIT.DevKit.Application.Commands;
using BridgingIT.DevKit.Common;
using MediatR;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Modules.Catalog.Domain;

public class ProductImportAllCommandHandler : CommandHandlerBase<ProductImportAllCommand, Result<int>>
{
    private readonly IMediator mediator;
    private readonly IExcelInterchangeService excelService;
    private readonly IStringLocalizer<CatalogResources> localizer;

    public ProductImportAllCommandHandler(
        ILoggerFactory loggerFactory,
        IStringLocalizer<CatalogResources> localizer,
        IMediator mediator,
        IExcelInterchangeService excelService)
        : base(loggerFactory)
    {
        EnsureArg.IsNotNull(localizer, nameof(localizer));
        EnsureArg.IsNotNull(mediator, nameof(mediator));
        EnsureArg.IsNotNull(excelService, nameof(excelService));
        this.localizer = localizer;
        this.mediator = mediator;
        this.excelService = excelService;
    }

    public override async Task<CommandResponse<Result<int>>> Process(ProductImportAllCommand request, CancellationToken cancellationToken)
    {
        var stream = new MemoryStream(request.Data); // TODO: dispose strean
        var result = await this.excelService.ImportAsync(stream, mappers: new Dictionary<string, Func<DataRow, Product, object>>
        {
            { this.localizer["Id"], (row, item) => item.Id = row["Id"].ToString().IsNullOrEmpty() ? Guid.Empty : Guid.Parse(row["Id"].ToString()) },
            { this.localizer["Name"], (row, item) => item.Name = row[this.localizer["Name"]].ToString() },
            { this.localizer["Barcode"], (row, item) => item.Barcode = row[this.localizer["Barcode"]].ToString() },
            { this.localizer["Description"], (row, item) => item.Description = row[this.localizer["Description"]].ToString() },
            { this.localizer["Rating"], (row, item) => item.Rating = row[this.localizer["Rating"]].To<int>() },
            { this.localizer["Price"], (row, item) => item.Price = row[this.localizer["Price"]].To<decimal>() },
            { this.localizer["Size"], (row, item) => item.Size = row[this.localizer["Size"]].ToString() },
            { this.localizer["Sku"], (row, item) => item.Sku = row[this.localizer["Sku"]].ToString() },
            { this.localizer["BrandId"], (row, item) => item.BrandId = row["BrandId"].ToString().IsNullOrEmpty() ? Guid.Empty : Guid.Parse(row["BrandId"].ToString()) },
            { this.localizer["TypeId"], (row, item) => item.TypeId = row["TypeId"].ToString().IsNullOrEmpty() ? Guid.Empty : Guid.Parse(row["TypeId"].ToString()) },
        }, new ExcelInterchangeServiceOptions { SheetName = this.localizer["Products"] });

        if (result.IsSuccess)
        {
            var createCommands = new List<ProductCreateCommand>();
            var updateCommands = new List<ProductUpdateCommand>();
            var errors = new List<string>();

            foreach (var entity in result.Value)
            {
                if (entity.Id == Guid.Empty)
                {
                    var command = new ProductCreateCommand(entity, request.Identity);
                    var validationResult = command.Validate();
                    if (validationResult.IsValid)
                    {
                        createCommands.Add(command);
                    }
                    else
                    {
                        errors.AddRange(validationResult.Errors.Select(e => $"{(!string.IsNullOrWhiteSpace(entity.Name) ? $"{entity.Name} - " : string.Empty)}{e.ErrorMessage}"));
                    }
                }
                else
                {
                    var command = new ProductUpdateCommand(entity, request.Identity);
                    var validationResult = command.Validate();
                    if (validationResult.IsValid)
                    {
                        updateCommands.Add(command);
                    }
                    else
                    {
                        errors.AddRange(validationResult.Errors.Select(e => $"{(!string.IsNullOrWhiteSpace(entity.Name) ? $"{entity.Name} - " : string.Empty)}{e.ErrorMessage}"));
                    }
                }
            }

            if (errors.Count == 0)
            {
                foreach (var command in createCommands)
                {
                    await this.mediator.Send(command, cancellationToken);
                }

                foreach (var command in updateCommands)
                {
                    await this.mediator.Send(command, cancellationToken);
                }

                return new CommandResponse<Result<int>>()
                {
                    Result = Result<int>.Success(
                        result.Value.Count(),
                        this.localizer[CatalogResources.Products_Imported, result.Value.Count()])
                };
            }
            else
            {
                return new CommandResponse<Result<int>>()
                {
                    Result = Result<int>.Failure(errors)
                };
            }
        }
        else
        {
            return new CommandResponse<Result<int>>()
            {
                Result = Result<int>.Failure(result.Messages.ToList())
            };
        }
    }
}