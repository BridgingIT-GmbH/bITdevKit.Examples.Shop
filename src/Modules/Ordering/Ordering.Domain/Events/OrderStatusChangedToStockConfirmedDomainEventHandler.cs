namespace Modules.Ordering.Domain;

using BridgingIT.DevKit.Domain;
using BridgingIT.DevKit.Domain.Repositories;
using Microsoft.Extensions.Logging;

public class OrderStatusChangedToStockConfirmedDomainEventHandler : DomainEventHandlerBase<OrderStatusChangedToStockConfirmedDomainEvent>
{
    private readonly IGenericRepository<Order> orderRepository;
    private readonly IGenericRepository<Buyer> buyerRepository;

    public OrderStatusChangedToStockConfirmedDomainEventHandler(
        ILoggerFactory loggerFactory,
        IGenericRepository<Order> orderRepository,
        IGenericRepository<Buyer> buyerRepository)
        : base(loggerFactory)
    {
        this.orderRepository = orderRepository;
        this.buyerRepository = buyerRepository;
    }

    public override bool CanHandle(OrderStatusChangedToStockConfirmedDomainEvent notification)
    {
        return notification != null;
    }

    public override async Task Process(OrderStatusChangedToStockConfirmedDomainEvent notification, CancellationToken cancellationToken)
    {
        this.Logger.LogInformation(
            "Order with Id: {OrderId} has been successfully updated to status {Status} ({Id})",
            notification.OrderId, nameof(OrderStatus.StockConfirmed), OrderStatus.StockConfirmed.Id);

        var order = await this.orderRepository.FindOneAsync(notification.OrderId, cancellationToken: cancellationToken);
        var buyer = await this.buyerRepository.FindOneAsync(order.BuyerId, cancellationToken: cancellationToken);

        // TODO: send OrderSubmittedDomainEvent instead of IntegrationEvent?
        //       or send command? src\Services\Ordering\Ordering.API\Application\IntegrationEvents\EventHandling\OrderStockConfirmedIntegrationEventHandler.cs
        //var orderStatusChangedToStockConfirmedIntegrationEvent = new OrderStatusChangedToStockConfirmedIntegrationEvent(order.Id, order.OrderStatus.Name, buyer.Name);
        //await _orderingIntegrationEventService.AddAndSaveEventAsync(orderStatusChangedToStockConfirmedIntegrationEvent);
    }
}
