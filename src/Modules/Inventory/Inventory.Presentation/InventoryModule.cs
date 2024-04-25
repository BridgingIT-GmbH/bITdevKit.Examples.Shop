namespace Modules.Inventory.Presentation;

using BridgingIT.DevKit.Common;
using BridgingIT.DevKit.Domain.Repositories;
using BridgingIT.DevKit.Infrastructure.LiteDb.Repositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Modules.Inventory.Application;
using Modules.Inventory.Domain;
using Modules.Inventory.Infrastructure.LiteDb;

public class InventoryModule : WebModuleBase
{
    public override IServiceCollection Register(IServiceCollection services, IConfiguration configuration = null, IWebHostEnvironment environment = null)
    {
        var moduleConfiguration = services.Configure<InventoryModuleConfiguration, InventoryModuleConfiguration.Validator>(configuration, this);

        //services.AddJobScheduling()
        //    .WithScopedJob<InventoryImportJob>(CronExpressions.Every10Minutes);

        services.AddMessaging()
            .WithSubscription<EchoMessage, EchoMessageHandler>() // handler for the EchoMessage sent by the EchoMessageJob
            .WithSubscription<CatalogImportedMessage, CatalogImportedMessageHandler>()
            .WithSubscription<InventoryImportedMessage, InventoryImportedMessageHandler>();

        services.AddSingleton(sp =>
            new InventoryLiteDbContext(moduleConfiguration.ConnectionStrings.GetValue("LiteDb"))); // configuration[$"Modules:{this.Name}:ConnectionStrings:InventoryLiteDb"]
        services.AddScoped<IGenericRepository<Stock>>(sp =>
            new LiteDbGenericRepository<Stock>(o => o
                .LoggerFactory(sp.GetRequiredService<ILoggerFactory>())
                .DbContext(sp.GetRequiredService<InventoryLiteDbContext>())))
            .Decorate<IGenericRepository<Stock>, RepositoryTracingBehavior<Stock>>()
            .Decorate<IGenericRepository<Stock>, RepositoryLoggingBehavior<Stock>>()
            .Decorate<IGenericRepository<Stock>, RepositoryDomainEventBehavior<Stock>>()
            .Decorate<IGenericRepository<Stock>, RepositoryDomainEventPublisherBehavior<Stock>>();

        return services;
    }
}