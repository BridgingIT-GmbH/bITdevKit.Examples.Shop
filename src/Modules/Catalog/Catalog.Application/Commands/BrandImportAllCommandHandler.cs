namespace Modules.Catalog.Application.Commands;

using System.Data;
using BridgingIT.DevKit.Application.Collaboration;
using BridgingIT.DevKit.Application.Commands;
using BridgingIT.DevKit.Common;
using MediatR;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Modules.Catalog.Domain;

public class BrandImportAllCommandHandler : CommandHandlerBase<BrandImportAllCommand, Result<int>>
{
    private readonly IStringLocalizer<CatalogResources> localizer;
    private readonly IMediator mediator;
    private readonly IExcelInterchangeService excelService;

    public BrandImportAllCommandHandler(
        ILoggerFactory loggerFactory,
        IStringLocalizer<CatalogResources> localizer,
        IMediator mediator,
        IExcelInterchangeService excelService)
        : base(loggerFactory)
    {
        EnsureArg.IsNotNull(mediator, nameof(mediator));
        EnsureArg.IsNotNull(excelService, nameof(excelService));
        EnsureArg.IsNotNull(localizer, nameof(localizer));

        this.mediator = mediator;
        this.excelService = excelService;
        this.localizer = localizer;
    }

    public override async Task<CommandResponse<Result<int>>> Process(BrandImportAllCommand request, CancellationToken cancellationToken)
    {
        var stream = new MemoryStream(request.Data); // TODO: dispose strean
        var result = await this.excelService.ImportAsync(stream, mappers: new Dictionary<string, Func<DataRow, Brand, object>>
        {
            { this.localizer["Id"], (row, item) => item.Id = row["Id"].ToString().IsNullOrEmpty() ? Guid.Empty : Guid.Parse(row["Id"].ToString()) },
            { this.localizer["Name"], (row, item) => item.Name = row[this.localizer["Name"]].ToString() },
            { this.localizer["Description"], (row, item) => item.Description = row[this.localizer["Description"]].ToString() },
        }, new ExcelInterchangeServiceOptions { SheetName = this.localizer["Brands"] });

        if (result.IsSuccess)
        {
            var createCommands = new List<BrandCreateCommand>();
            var updateCommands = new List<BrandUpdateCommand>();
            var errors = new List<string>();

            foreach (var entity in result.Value)
            {
                if (entity.Id == Guid.Empty)
                {
                    var command = new BrandCreateCommand(entity, request.Identity);
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
                    var command = new BrandUpdateCommand(entity, request.Identity);
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
                        this.localizer[CatalogResources.Brands_Imported, result.Value.Count()])
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
