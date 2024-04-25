namespace Modules.Inventory.Application;

using BridgingIT.DevKit.Application.Commands;
using BridgingIT.DevKit.Common;
using BridgingIT.DevKit.Domain.Repositories;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Modules.Inventory.Domain;

public class StockCreateCommandHandler : CommandHandlerBase<StockCreateCommand, Result<EntityCreatedCommandResult>>
{
    private readonly IStringLocalizer<InventoryResources> localizer;
    private readonly IGenericRepository<Stock> repository;

    public StockCreateCommandHandler(
        ILoggerFactory loggerFactory,
        IStringLocalizer<InventoryResources> localizer,
        IGenericRepository<Stock> repository)
        : base(loggerFactory)
    {
        EnsureArg.IsNotNull(localizer, nameof(localizer));
        EnsureArg.IsNotNull(repository, nameof(repository));
        this.localizer = localizer;
        this.repository = repository;
    }

    public override async Task<CommandResponse<Result<EntityCreatedCommandResult>>> Process(
        StockCreateCommand command,
        CancellationToken cancellationToken)
    {
        await this.repository.InsertAsync(command.Entity, cancellationToken).AnyContext();

        return new CommandResponse<Result<EntityCreatedCommandResult>>
        {
            Result = Result<EntityCreatedCommandResult>.Success(
                new EntityCreatedCommandResult(command.Entity.Id.ToString()),
                this.localizer[InventoryResources.Stock_Saved])
        };
    }
}
