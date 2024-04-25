namespace Modules.Inventory.Application;

using BridgingIT.DevKit.Application.Commands;
using BridgingIT.DevKit.Application.Messaging;
using BridgingIT.DevKit.Common;
using Microsoft.Extensions.Logging;

public class InventoryImportCommandHandler : CommandHandlerBase<InventoryImportCommand>
{
    private readonly IMessageBroker messageBroker;

    public InventoryImportCommandHandler(
        ILoggerFactory loggerFactory,
        IMessageBroker messageBroker)
        : base(loggerFactory)
    {
        EnsureArg.IsNotNull(messageBroker, nameof(messageBroker));

        this.messageBroker = messageBroker;
    }

    public override async Task<CommandResponse> Process(InventoryImportCommand command, CancellationToken cancellationToken)
    {
        this.Logger.LogDebug("INV +++++++++++++++++++++++ importing inventory...........");
        await Task.Delay(5000, cancellationToken).AnyContext(); // simulate the inventory import duration
        this.Logger.LogDebug("INV +++++++++++++++++++++++ importing inventory........... DONE #{ProductCount}", command.ProductCount);

        await this.messageBroker.Publish(
            new InventoryImportedMessage { Source = "TODO", ProductCount = command.ProductCount }, cancellationToken).AnyContext();

        return new CommandResponse();
    }
}