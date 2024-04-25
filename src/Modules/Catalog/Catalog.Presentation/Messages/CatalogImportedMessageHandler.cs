namespace Modules.Catalog.Presentation;

using BridgingIT.DevKit.Application.Messaging;
using Common.SignalR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Modules.Catalog.Application;

public class CatalogImportedMessageHandler : MessageHandlerBase<CatalogImportedMessage>
{
    private readonly IHubContext<SignalRHub> hub;

    public CatalogImportedMessageHandler(ILoggerFactory loggerFactory, IHubContext<SignalRHub> hub)
        : base(loggerFactory)
    {
        EnsureArg.IsNotNull(hub, nameof(hub));

        this.hub = hub;
    }

    /// <summary>
    /// Handles the specified message.
    /// </summary>
    /// <param name="message">The event.</param>
    public override async Task Handle(CatalogImportedMessage message, CancellationToken cancellationToken)
    {
        var loggerState = new Dictionary<string, object>
        {
            ["MessageId"] = message.Id,
        };

        using (this.Logger.BeginScope(loggerState))
        {
            await Task.Delay(300, cancellationToken);

            await this.hub.Clients.All.SendAsync(
                SignalRHubConstants.ReceiveInformationMessage,
                $"Catalog imported ({message.Source}): #{message.BrandCount} brands, #{message.ProductCount} products",
                cancellationToken);
        }
    }
}