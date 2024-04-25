namespace Modules.Inventory.Application;

using BridgingIT.DevKit.Application.Messaging;
using MediatR;
using Microsoft.Extensions.Logging;

public class CatalogImportedMessageHandler : MessageHandlerBase<CatalogImportedMessage>
{
    private readonly IMediator mediator;

    public CatalogImportedMessageHandler(ILoggerFactory loggerFactory, IMediator mediator)
        : base(loggerFactory)
    {
        EnsureArg.IsNotNull(mediator, nameof(mediator));

        this.mediator = mediator;
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
            await this.mediator.Send(
                new InventoryImportCommand() { BrandCount = message.BrandCount, ProductCount = message.ProductCount }, cancellationToken);
        }
    }
}
