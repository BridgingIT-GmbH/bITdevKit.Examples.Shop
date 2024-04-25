namespace Modules.Shared.Application;

using BridgingIT.DevKit.Application.JobScheduling;
using BridgingIT.DevKit.Application.Messaging;
using BridgingIT.DevKit.Common;
using Microsoft.Extensions.Logging;
using Quartz;

public class EchoMessageJob : JobBase,
    IRetryJobScheduling,
    IChaosExceptionJobScheduling
{
    private readonly IMessageBroker messageBroker;

    public EchoMessageJob(ILoggerFactory loggerFactory, IMessageBroker messageBroker)
        : base(loggerFactory)
    {
        EnsureArg.IsNotNull(messageBroker, nameof(messageBroker));
        this.messageBroker = messageBroker;
    }

    RetryJobSchedulingOptions IRetryJobScheduling.Options => new() { Attempts = 3, Backoff = new TimeSpan(0, 0, 0, 1) };

    ChaosExceptionJobSchedulingOptions IChaosExceptionJobScheduling.Options => new() { InjectionRate = 0.10 };

    public override async Task Process(IJobExecutionContext context, CancellationToken cancellationToken = default)
    {
        //await Task.Delay(2000, cancellationToken);

        await this.messageBroker.Publish(
            new EchoMessage("from job"), cancellationToken).AnyContext();
    }
}