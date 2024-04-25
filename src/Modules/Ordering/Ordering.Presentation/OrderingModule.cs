namespace Modules.Ordering.Presentation;
using BridgingIT.DevKit.Common;
using BridgingIT.DevKit.Domain.Repositories;
using BridgingIT.DevKit.Infrastructure.EntityFramework.Repositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Modules.Ordering.Application;
using Modules.Ordering.Domain;
using Modules.Ordering.Infrastructure.EntityFramework;

public class OrderingModule : WebModuleBase
{
    public override IServiceCollection Register(IServiceCollection services, IConfiguration configuration = null, IWebHostEnvironment environment = null)
    {
        var moduleConfiguration = services.Configure<OrderingModuleConfiguration, OrderingModuleConfiguration.Validator>(configuration, this);

        services.AddMessaging()
            .WithSubscription<EchoMessage, EchoMessageHandler>(); // handler for the EchoMessage sent by the EchoMessageJob

        services.AddSqlServerDbContext<OrderingDbContext>(o => o
                .UseConnectionString(services.BuildServiceProvider().GetRequiredService<IOptions<OrderingModuleConfiguration>>().Value.ConnectionStrings.GetValue("SqlServer"))
                .UseLogger())
            //.WithHealthChecks()
            .WithDatabaseMigratorService();

        services.AddEntityFrameworkRepository<Order, OrderingDbContext>()
            .WithBehavior<RepositoryDomainEventPublisherBehavior<Order>>()
            .WithBehavior<RepositoryDomainEventBehavior<Order>>()
            .WithBehavior<RepositoryNoTrackingBehavior<Order>>()
            .WithBehavior<RepositoryLoggingBehavior<Order>>()
            .WithBehavior<RepositoryTracingBehavior<Order>>();

        services.AddEntityFrameworkRepository<Buyer, OrderingDbContext>()
            .WithBehavior<RepositoryDomainEventPublisherBehavior<Buyer>>()
            .WithBehavior<RepositoryDomainEventBehavior<Buyer>>()
            .WithBehavior<RepositoryNoTrackingBehavior<Buyer>>()
            .WithBehavior<RepositoryLoggingBehavior<Buyer>>()
            .WithBehavior<RepositoryTracingBehavior<Buyer>>();

        return services;
    }
}