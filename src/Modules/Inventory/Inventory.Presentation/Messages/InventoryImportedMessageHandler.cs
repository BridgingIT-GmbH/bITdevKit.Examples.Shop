namespace Modules.Inventory.Presentation;

using BridgingIT.DevKit.Application.Messaging;
using Common.SignalR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Modules.Inventory.Application;

public class InventoryImportedMessageHandler : MessageHandlerBase<InventoryImportedMessage>
{
    private readonly IHubContext<SignalRHub> hub;

    public InventoryImportedMessageHandler(ILoggerFactory loggerFactory, IHubContext<SignalRHub> hub)
        : base(loggerFactory)
    {
        EnsureArg.IsNotNull(hub, nameof(hub));

        this.hub = hub;
    }

    /// <summary>
    /// Handles the specified message.
    /// </summary>
    /// <param name="message">The event.</param>
    public override async Task Handle(InventoryImportedMessage message, CancellationToken cancellationToken)
    {
        var loggerState = new Dictionary<string, object>
        {
            ["MessageId"] = message.Id,
        };

        using (this.Logger.BeginScope(loggerState))
        {
            await this.hub.Clients.All.SendAsync(
                SignalRHubConstants.ReceiveInformationMessage,
                $"Inventory imported ({message.Source}): # {message.ProductCount} products",
                cancellationToken);
        }
    }
}