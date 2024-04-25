namespace Modules.Ordering.Domain;

using BridgingIT.DevKit.Domain;
using BridgingIT.DevKit.Domain.Repositories;
using Microsoft.Extensions.Logging;

public class OrderStatusChangedToPaidDomainEventHandler : DomainEventHandlerBase<OrderStatusChangedToPaidDomainEvent>
{
    private readonly IGenericRepository<Order> orderRepository;
    private readonly IGenericRepository<Buyer> buyerRepository;

    public OrderStatusChangedToPaidDomainEventHandler(
        ILoggerFactory loggerFactory,
        IGenericRepository<Order> orderRepository,
        IGenericRepository<Buyer> buyerRepository)
        : base(loggerFactory)
    {
        this.orderRepository = orderRepository;
        this.buyerRepository = buyerRepository;
    }

    public override bool CanHandle(OrderStatusChangedToPaidDomainEvent notification)
    {
        return notification != null;
    }

    public override async Task Process(OrderStatusChangedToPaidDomainEvent notification, CancellationToken cancellationToken)
    {
        this.Logger.LogInformation(
            "Order with Id: {OrderId} has been successfully updated to status {Status} ({Id})",
            notification.OrderId, nameof(OrderStatus.Paid), OrderStatus.Paid.Id);

        var order = await this.orderRepository.FindOneAsync(notification.OrderId, cancellationToken: cancellationToken);
        var buyer = await this.buyerRepository.FindOneAsync(order.BuyerId, cancellationToken: cancellationToken);

        // TODO: send OrderSubmittedDomainEvent instead of IntegrationEvent?
        //       or send command?
        //var orderStockList = notification.OrderItems
        //    .Select(i => new OrderStockItem(i.ProductId, i.Units));

        //var orderStatusChangedToPaidIntegrationEvent = new OrderStatusChangedToPaidIntegrationEvent(
        //    orderStatusChangedToPaidDomainEvent.OrderId,
        //    order.OrderStatus.Name,
        //    buyer.Name,
        //    orderStockList);

        //await _orderingIntegrationEventService.AddAndSaveEventAsync(orderStatusChangedToPaidIntegrationEvent);
    }
}
