//// src\Services\Catalog\Catalog.API\IntegrationEvents\EventHandling\OrderStatusChangedToAwaitingValidationIntegrationEventHandler.cs
//namespace Catalog.Application
//{
//    using BridgingIT.DevKit.Domain;
//    using BridgingIT.DevKit.Domain.Repositories;
//    using Catalog.Domain;
//    using Domain.Ordering;
//    using Domain.Ordering.Model;
//    using Microsoft.Extensions.Logging;

//    public class OrderStatusChangedToAwaitingValidationDomainEventHandler : DomainEventHandlerBase<OrderStatusChangedToAwaitingValidationDomainEvent>
//    {
//        private readonly IGenericRepository<Product> catalogRepository;

//        public OrderStatusChangedToAwaitingValidationDomainEventHandler(
//            ILoggerFactory loggerFactory,
//            IGenericRepository<Product> catalogRepository)
//            : base(loggerFactory)
//        {
//            this.catalogRepository = catalogRepository;
//        }

//        public override bool CanHandle(OrderStatusChangedToAwaitingValidationDomainEvent notification)
//        {
//            return notification != null;
//        }

//        public override async Task Process(OrderStatusChangedToAwaitingValidationDomainEvent notification, CancellationToken cancellationToken)
//        {
//            this.Logger.LogInformation(
//                "Order with Id: {OrderId} has been successfully updated to status {Status} ({Id})",
//                notification.OrderId, nameof(OrderStatus.Paid), OrderStatus.Paid.Id);

//            await Task.Delay(5);
//            //var confirmedOrderStockItems = new List<ConfirmedOrderStockItem>();

//            //foreach (var orderStockItem in notification.OrderItems)
//            //{
//            //    var catalogItem = await this.catalogRepository.FindOneAsync(orderStockItem.ProductId, cancellationToken: cancellationToken);
//            //    var hasStock = catalogItem.AvailableStock >= orderStockItem.Units;

//            //    //confirmedOrderStockItems.Add(new ConfirmedOrderStockItem(catalogItem.Id, hasStock));
//            //}

//            // TODO: send OrderSubmittedDomainEvent instead of IntegrationEvent?
//            //       send another domain event?
//            //var confirmedIntegrationEvent = confirmedOrderStockItems.Any(c => !c.HasStock)
//            //    ? (IntegrationEvent)new OrderStockRejectedIntegrationEvent(@event.OrderId, confirmedOrderStockItems)
//            //    : new OrderStockConfirmedIntegrationEvent(@event.OrderId);

//            //await _catalogIntegrationEventService.SaveEventAndCatalogContextChangesAsync(confirmedIntegrationEvent);
//            //await _catalogIntegrationEventService.PublishThroughEventBusAsync(confirmedIntegrationEvent);
//        }
//    }
//}
