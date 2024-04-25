namespace Modules.Shopping.Application.Integration;

using BridgingIT.DevKit.Application.Messaging;
using BridgingIT.DevKit.Application.Storage;
using Microsoft.Extensions.Logging;

public class ProductImportedMessageHandler : MessageHandlerBase<ProductImportedMessage>
//IRetryMessageHandler,
//IChaosExceptionMessageHandler
{
    private readonly IDocumentStoreClient<ReferenceDataProduct> documentStoreClient;

    public ProductImportedMessageHandler(
        ILoggerFactory loggerFactory,
        IDocumentStoreClient<ReferenceDataProduct> documentStoreClient)
        : base(loggerFactory)
    {
        EnsureArg.IsNotNull(documentStoreClient, nameof(documentStoreClient));

        this.documentStoreClient = documentStoreClient;
    }

    //RetryMessageHandlerOptions IRetryMessageHandler.Options => new() { Attempts = 3, Backoff = new TimeSpan(0, 0, 0, 1) };

    //ChaosExceptionMessageHandlerOptions IChaosExceptionMessageHandler.Options => new() { InjectionRate = 0.0 };

    /// <summary>
    /// Handles the specified message.
    /// </summary>
    /// <param name="message">The event.</param>
    public override async Task Handle(ProductImportedMessage message, CancellationToken cancellationToken)
    {
        await Task.Delay(3000, cancellationToken);

        this.Logger.LogDebug("Store shopping product reference data (sku={ProductSKU})", message.SKU);

        var entity = new ReferenceDataProduct(message.SKU) { Name = message.Name, Price = message.Price };
        var existingEntity = (await this.documentStoreClient.FindAsync(
            new(nameof(ReferenceDataProduct), entity.SKU), cancellationToken)).FirstOrDefault();

        if (existingEntity != null) // preserve some existing values if not part of inbound entity
        {
            //entity.Id = existingEntity.Id;
            if (entity.Name == null && existingEntity.Name != null)
            {
                entity.Name = existingEntity.Name;
            }

            if (entity.Price == null && existingEntity.Price != null)
            {
                entity.Price = existingEntity.Price;
            }

            if (entity.Stock == null && existingEntity.Stock != null)
            {
                entity.Stock = existingEntity.Stock;
            }
        }

        await this.documentStoreClient.UpsertAsync(
            new(nameof(ReferenceDataProduct), entity.SKU), entity, cancellationToken);
    }
}