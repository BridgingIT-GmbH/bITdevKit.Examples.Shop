namespace Modules.Shopping.Application;

using BridgingIT.DevKit.Application.Commands;
using BridgingIT.DevKit.Application.Storage;
using BridgingIT.DevKit.Common;
using BridgingIT.DevKit.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Modules.Shopping.Application.Integration;
using Modules.Shopping.Application.Queries;
using Modules.Shopping.Domain;

public class CartAddItemCommandHandler : CommandHandlerBase<CartAddItemCommand, Result<Cart>>
{
    private readonly IMediator mediator;
    private readonly IStringLocalizer<ShoppingResources> localizer;
    private readonly IGenericRepository<Cart> repository;
    private readonly IDocumentStoreClient<ReferenceDataProduct> documentStoreClient;

    public CartAddItemCommandHandler(
        ILoggerFactory loggerFactory,
        IMediator mediator,
        IStringLocalizer<ShoppingResources> localizer,
        IGenericRepository<Cart> repository,
        IDocumentStoreClient<ReferenceDataProduct> documentStoreClient)
        : base(loggerFactory)
    {
        EnsureArg.IsNotNull(mediator, nameof(mediator));
        EnsureArg.IsNotNull(localizer, nameof(localizer));
        EnsureArg.IsNotNull(repository, nameof(repository));
        EnsureArg.IsNotNull(documentStoreClient, nameof(documentStoreClient));

        this.mediator = mediator;
        this.localizer = localizer;
        this.repository = repository;
        this.documentStoreClient = documentStoreClient;
    }

    public override async Task<CommandResponse<Result<Cart>>> Process(
        CartAddItemCommand command,
        CancellationToken cancellationToken)
    {
        var cart = (await this.mediator.Send(new CartFindOneQuery(command.Identity), cancellationToken))?.Result?.Value;
        if (cart == null)
        {
            cart = Cart.ForUser(command.Identity); // Or send CartCreateCommand?
        }

        var product = (await this.documentStoreClient.FindAsync(
            new(nameof(ReferenceDataProduct), command.SKU), cancellationToken)).FirstOrDefault();
        if (product == null)
        {
            throw new Exception("Reference Product not found");
        }

        cart.AddItem(CartProduct.For(product.SKU, product.Name, product.Price), command.Quantity);
        await this.repository.UpsertAsync(cart, cancellationToken);

        return new CommandResponse<Result<Cart>>
        {
            Result = Result<Cart>.Success(
                cart,
                this.localizer[ShoppingResources.CartItem_Added])
        };
    }
}