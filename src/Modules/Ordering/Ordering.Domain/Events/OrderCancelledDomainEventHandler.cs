namespace Modules.Ordering.Domain;

using BridgingIT.DevKit.Domain;
using BridgingIT.DevKit.Domain.Repositories;
using Microsoft.Extensions.Logging;

public class OrderCancelledDomainEventHandler : DomainEventHandlerBase<OrderCancelledDomainEvent>
{
    private readonly IGenericRepository<Order> orderRepository;
    private readonly IGenericRepository<Buyer> buyerRepository;

    public OrderCancelledDomainEventHandler(
        ILoggerFactory loggerFactory,
        IGenericRepository<Order> orderRepository,
        IGenericRepository<Buyer> buyerRepository)
        : base(loggerFactory)
    {
        this.orderRepository = orderRepository;
        this.buyerRepository = buyerRepository;
    }

    public override bool CanHandle(OrderCancelledDomainEvent notification)
    {
        return notification != null;
    }

    public override async Task Process(OrderCancelledDomainEvent notification, CancellationToken cancellationToken)
    {
        this.Logger.LogInformation(
            "Order with Id: {OrderId} has been successfully updated to status {Status} ({Id})",
            notification.Order.Id, nameof(OrderStatus.AwaitingValidation), OrderStatus.AwaitingValidation.Id);

        var order = await this.orderRepository.FindOneAsync(notification.Order.Id, cancellationToken: cancellationToken);
        var buyer = await this.buyerRepository.FindOneAsync(order.BuyerId, cancellationToken: cancellationToken);

        // TODO: send OrderSubmittedDomainEvent instead of IntegrationEvent?
        //var orderStatusChangedToCancelledIntegrationEvent = new OrderStatusChangedToCancelledIntegrationEvent(order.Id, order.OrderStatus.Name, buyer.Name);
        //await _orderingIntegrationEventService.AddAndSaveEventAsync(orderStatusChangedToCancelledIntegrationEvent);
    }
}
