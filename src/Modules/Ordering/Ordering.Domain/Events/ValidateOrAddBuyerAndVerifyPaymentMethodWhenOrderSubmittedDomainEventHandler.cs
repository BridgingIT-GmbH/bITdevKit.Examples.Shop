namespace Modules.Ordering.Domain;

using BridgingIT.DevKit.Domain;
using BridgingIT.DevKit.Domain.Repositories;
using BridgingIT.DevKit.Domain.Specifications;
using Microsoft.Extensions.Logging;

public class ValidateOrAddBuyerAndVerifyPaymentMethodWhenOrderSubmittedDomainEventHandler : DomainEventHandlerBase<OrderSubmittedDomainEvent>
{
    private readonly IGenericRepository<Buyer> buyerRepository;

    public ValidateOrAddBuyerAndVerifyPaymentMethodWhenOrderSubmittedDomainEventHandler(
        ILoggerFactory loggerFactory,
        IGenericRepository<Buyer> buyerRepository)
        : base(loggerFactory)
    {
        this.buyerRepository = buyerRepository;
    }

    public override bool CanHandle(OrderSubmittedDomainEvent notification)
    {
        return notification.Order != null;
    }

    public override async Task Process(OrderSubmittedDomainEvent notification, CancellationToken cancellationToken)
    {
        var cardTypeId = (notification.CardTypeId != 0) ? notification.CardTypeId : 1;
        var buyer = (await this.buyerRepository.FindAllAsync(
            new Specification<Buyer>(b => b.Identity == notification.UserId), cancellationToken: cancellationToken)).FirstOrDefault();
        if (buyer != null)
        {
            buyer = Buyer.ForUser(notification.UserId, notification.UserName, notification.Email);
        }

        buyer.VerifyOrAddPaymentMethod(cardTypeId,
                                       $"Payment Method on {DateTime.UtcNow}",
                                       notification.CardNumber,
                                       notification.CardSecurityNumber,
                                       notification.CardHolderName,
                                       notification.CardExpiration,
                                       notification.Order.Id);

        await this.buyerRepository.UpsertAsync(buyer, cancellationToken);

        // TODO: send OrderSubmittedDomainEvent instead of IntegrationEvent?
        //var orderStatusChangedTosubmittedIntegrationEvent = new OrderStatusChangedToSubmittedIntegrationEvent(orderStartedEvent.Order.Id, orderStartedEvent.Order.OrderStatus.Name, buyer.Name);
        //await _orderingIntegrationEventService.AddAndSaveEventAsync(orderStatusChangedTosubmittedIntegrationEvent);

        this.Logger.LogInformation(
            "Buyer {BuyerId} and related payment method were validated or updated for orderId: {OrderId}.",
            buyer.Id, notification.Order.Id);
    }
}
