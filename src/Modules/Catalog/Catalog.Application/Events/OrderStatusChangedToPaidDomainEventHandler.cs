//// src\Services\Catalog\Catalog.API\IntegrationEvents\EventHandling\OrderStatusChangedToPaidIntegrationEventHandler.cs
//namespace Catalog.Application
//{
//    using BridgingIT.DevKit.Domain;
//    using BridgingIT.DevKit.Domain.Repositories;
//    using Catalog.Domain;
//    using Domain.Ordering;
//    using Domain.Ordering.Model;
//    using Microsoft.Extensions.Logging;

//    public class OrderStatusChangedToPaidDomainEventHandler : DomainEventHandlerBase<OrderStatusChangedToPaidDomainEvent>
//    {
//        private readonly IGenericRepository<Product> catalogRepository;

//        public OrderStatusChangedToPaidDomainEventHandler(
//            ILoggerFactory loggerFactory,
//            IGenericRepository<Product> catalogRepository)
//            : base(loggerFactory)
//        {
//            this.catalogRepository = catalogRepository;
//        }

//        public override bool CanHandle(OrderStatusChangedToPaidDomainEvent notification)
//        {
//            return notification != null;
//        }

//        public override async Task Process(OrderStatusChangedToPaidDomainEvent notification, CancellationToken cancellationToken)
//        {
//            this.Logger.LogInformation(
//                "Order with Id: {OrderId} has been successfully updated to status {Status} ({Id})",
//                notification.OrderId, nameof(OrderStatus.Paid), OrderStatus.Paid.Id);

//            await Task.Delay(5);

//            foreach (var item in notification.OrderItems)
//            {
//                //var catalogItem = await this.catalogRepository.FindOneAsync(item.ProductId, cancellationToken: cancellationToken);

//                //catalogItem.RemoveStock(item.Units);

//                //await this.catalogRepository.UpsertAsync(catalogItem, cancellationToken);
//            }
//        }
//    }
//}
