namespace Modules.Ordering.Domain;

using BridgingIT.DevKit.Domain;
using BridgingIT.DevKit.Domain.Repositories;
using Microsoft.Extensions.Logging;

public class UpdateOrderWhenBuyerAndPaymentMethodVerifiedDomainEventHandler : DomainEventHandlerBase<BuyerAndPaymentMethodVerifiedDomainEvent>
{
    private readonly IGenericRepository<Order> orderRepository;

    public UpdateOrderWhenBuyerAndPaymentMethodVerifiedDomainEventHandler(ILoggerFactory loggerFactory, IGenericRepository<Order> orderRepository)
        : base(loggerFactory)
    {
        this.orderRepository = orderRepository;
    }

    public override bool CanHandle(BuyerAndPaymentMethodVerifiedDomainEvent notification)
    {
        return notification != null;
    }

    public override async Task Process(BuyerAndPaymentMethodVerifiedDomainEvent notification, CancellationToken cancellationToken)
    {
        var order = await this.orderRepository.FindOneAsync(notification.OrderId, cancellationToken: cancellationToken);

        if (order != null)
        {
            order.SetBuyerId(notification.Buyer.Id);
            order.SetPaymentId(notification.Payment.Id);

            this.Logger.LogInformation(
                "Order with Id: {OrderId} has been successfully updated with a payment method {PaymentMethod} ({Id})",
                notification.OrderId, nameof(notification.Payment), notification.Payment.Id);
        }
        else
        {
            this.Logger.LogWarning(
                "Order with Id: {OrderId} could not be found and has not been updated with a payment method {PaymentMethod} ({Id})",
                notification.OrderId, nameof(notification.Payment), notification.Payment.Id);
        }
    }
}