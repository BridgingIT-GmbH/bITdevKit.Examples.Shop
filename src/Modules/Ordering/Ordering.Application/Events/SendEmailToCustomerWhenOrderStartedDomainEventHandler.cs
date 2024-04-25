namespace Modules.Ordering.Application;

using BridgingIT.DevKit.Domain;
using Microsoft.Extensions.Logging;
using Modules.Ordering.Domain;

public class SendEmailToCustomerWhenOrderStartedDomainEventHandler : DomainEventHandlerBase<BuyerAndPaymentMethodVerifiedDomainEvent>
{
    public SendEmailToCustomerWhenOrderStartedDomainEventHandler(ILoggerFactory loggerFactory)
        : base(loggerFactory)
    {
    }

    public override bool CanHandle(BuyerAndPaymentMethodVerifiedDomainEvent notification)
    {
        throw new System.NotImplementedException();
    }

    public override Task Process(BuyerAndPaymentMethodVerifiedDomainEvent notification, CancellationToken cancellationToken)
    {
        throw new System.NotImplementedException();
        //TBD - Send email logic
    }
}
