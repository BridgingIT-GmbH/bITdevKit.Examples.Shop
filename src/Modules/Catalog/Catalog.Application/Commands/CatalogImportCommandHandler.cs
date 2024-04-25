namespace Modules.Catalog.Application.Commands;

using BridgingIT.DevKit.Application.Commands;
using BridgingIT.DevKit.Application.Messaging;
using BridgingIT.DevKit.Common;
using BridgingIT.DevKit.Domain.Repositories;
using Microsoft.Extensions.Logging;
using Modules.Catalog.Domain;

public class CatalogImportCommandHandler : CommandHandlerBase<CatalogImportCommand>
{
    private readonly ICatalogDataAdapter dataAdapter;
    private readonly IGenericRepository<Product> productRepository;
    private readonly IGenericRepository<Brand> brandRepository;
    private readonly IMessageBroker messageBroker;

    public CatalogImportCommandHandler(
        ILoggerFactory loggerFactory,
        ICatalogDataAdapter dataAdapter,
        IGenericRepository<Product> productRepository,
        IGenericRepository<Brand> brandRepository,
        IMessageBroker messageBroker)
        : base(loggerFactory)
    {
        EnsureArg.IsNotNull(dataAdapter, nameof(dataAdapter));
        EnsureArg.IsNotNull(productRepository, nameof(productRepository));
        EnsureArg.IsNotNull(brandRepository, nameof(brandRepository));
        EnsureArg.IsNotNull(messageBroker, nameof(messageBroker));

        this.dataAdapter = dataAdapter;
        this.productRepository = productRepository;
        this.brandRepository = brandRepository;
        this.messageBroker = messageBroker;
    }

    public override async Task<CommandResponse> Process(CatalogImportCommand request, CancellationToken cancellationToken)
    {
        //var existingProducts = await this.productRepository.FindAllAsync(cancellationToken: cancellationToken).AnyContext();
        var brandCount = 0;
        var productCount = 0;

        await foreach (var brand in this.dataAdapter.GetBrands(cancellationToken))
        {
            this.Logger.LogDebug($"Catalog brand: {brand.Name}");
            await this.brandRepository.UpsertAsync(brand, cancellationToken);
            brandCount++;
        }

        await foreach (var product in this.dataAdapter.GetProducts(cancellationToken))
        {
            this.Logger.LogDebug($"Catalog product: {product.Name} (sku={product.Sku}, price={product.Price})");
            var result = await this.productRepository.UpsertAsync(product, cancellationToken);
            productCount++;

            await this.messageBroker.Publish( // TODO: add created/updated/deleted information to message?
            new ProductImportedMessage { SKU = product.Sku, Name = product.Name, Price = product.Price }, cancellationToken).AnyContext();
        }

        await this.messageBroker.Publish(
            new CatalogImportedMessage { Source = this.dataAdapter.GetType().Name, BrandCount = brandCount, ProductCount = productCount }, cancellationToken).AnyContext();

        return new CommandResponse();
    }
}