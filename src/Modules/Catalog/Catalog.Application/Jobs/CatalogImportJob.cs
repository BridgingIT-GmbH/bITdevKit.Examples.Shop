namespace Modules.Catalog.Application;

using BridgingIT.DevKit.Application.JobScheduling;
using BridgingIT.DevKit.Common;
using MediatR;
using Microsoft.Extensions.Logging;
using Modules.Catalog.Application.Commands;
using Quartz;

public class CatalogImportJob : JobBase
{
    private readonly IMediator mediator;

    public CatalogImportJob(ILoggerFactory loggerFactory, IMediator mediator)
        : base(loggerFactory)
    {
        EnsureArg.IsNotNull(mediator, nameof(mediator));

        this.mediator = mediator;
    }

    public override async Task Process(IJobExecutionContext context, CancellationToken cancellationToken = default)
    {
        await this.mediator.Send(
            new CatalogImportCommand(), cancellationToken).AnyContext();
    }
}
