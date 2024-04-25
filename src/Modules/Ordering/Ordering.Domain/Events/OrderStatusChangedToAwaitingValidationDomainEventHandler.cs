namespace Modules.Ordering.Domain;

using BridgingIT.DevKit.Domain;
using BridgingIT.DevKit.Domain.Repositories;
using Microsoft.Extensions.Logging;

public class OrderStatusChangedToAwaitingValidationDomainEventHandler : DomainEventHandlerBase<OrderStatusChangedToAwaitingValidationDomainEvent>
{
    private readonly IGenericRepository<Order> orderRepository;
    private readonly IGenericRepository<Buyer> buyerRepository;

    public OrderStatusChangedToAwaitingValidationDomainEventHandler(
        ILoggerFactory loggerFactory,
        IGenericRepository<Order> orderRepository,
        IGenericRepository<Buyer> buyerRepository)
        : base(loggerFactory)
    {
        this.orderRepository = orderRepository;
        this.buyerRepository = buyerRepository;
    }

    public override bool CanHandle(OrderStatusChangedToAwaitingValidationDomainEvent notification)
    {
        return notification != null;
    }

    public override async Task Process(OrderStatusChangedToAwaitingValidationDomainEvent notification, CancellationToken cancellationToken)
    {
        this.Logger.LogInformation(
            "Order with Id: {OrderId} has been successfully updated to status {Status} ({Id})",
            notification.OrderId, nameof(OrderStatus.AwaitingValidation), OrderStatus.AwaitingValidation.Id);

        var order = await this.orderRepository.FindOneAsync(notification.OrderId, cancellationToken: cancellationToken);
        var buyer = await this.buyerRepository.FindOneAsync(order.BuyerId, cancellationToken: cancellationToken);

        // TODO: send OrderSubmittedDomainEvent instead of IntegrationEvent?
        //var orderStockList = notification.OrderItems
        //    .Select(orderItem => new OrderStockItem(orderItem.ProductId, orderItem.Units));

        //var orderStatusChangedToAwaitingValidationIntegrationEvent = new OrderStatusChangedToAwaitingValidationIntegrationEvent(
        //    order.Id, order.OrderStatus.Name, buyer.Name, orderStockList);
        //await _orderingIntegrationEventService.AddAndSaveEventAsync(orderStatusChangedToAwaitingValidationIntegrationEvent);
    }
}
