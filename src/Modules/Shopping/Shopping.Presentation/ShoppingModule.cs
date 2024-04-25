namespace Modules.Shopping.Presentation;

using BridgingIT.DevKit.Application.Storage;
using BridgingIT.DevKit.Common;
using BridgingIT.DevKit.Domain.Repositories;
//using BridgingIT.DevKit.Infrastructure.Azure.Storage;
using BridgingIT.DevKit.Infrastructure.EntityFramework.Repositories;
using BridgingIT.DevKit.Infrastructure.EntityFramework.Storage;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Modules.Shopping.Application;
using Modules.Shopping.Application.Integration;
using Modules.Shopping.Domain;
using Modules.Shopping.Infrastructure.EntityFramework;

public class ShoppingModule : WebModuleBase
{
    public override IServiceCollection Register(IServiceCollection services, IConfiguration configuration = null, IWebHostEnvironment environment = null)
    {
        var moduleConfiguration = services.Configure<ShoppingModuleConfiguration, ShoppingModuleConfiguration.Validator>(configuration, this);

        services.AddMessaging()
            .WithSubscription<EchoMessage, EchoMessageHandler>() // handler for the EchoMessage sent by the EchoMessageJob
            .WithSubscription<ProductImportedMessage, ProductImportedMessageHandler>(); // handler for ProductImportedMessage sent by Catalog.CatalogImportCommandHandler

        services.AddSqlServerDbContext<ShoppingDbContext>(o => o
                .UseConnectionString(services.BuildServiceProvider().GetRequiredService<IOptions<ShoppingModuleConfiguration>>().Value.ConnectionStrings.GetValue("SqlServer"))
                .UseLogger())
            //.WithHealthChecks()
            .WithDatabaseMigratorService();

        //services.AddScoped<IDocumentStoreClient<ReferenceDataProduct>>(sp => // TODO: migrations need to be initiated
        //    new DocumentStoreClient<ReferenceDataProduct>(
        //    new InMemoryDocumentStoreProvider(
        //        sp.GetRequiredService<ILoggerFactory>(),
        //        new InMemoryDocumentStoreContext())));

        //services.AddScoped<IDocumentStoreClient<ReferenceDataProduct>>(sp => // TODO: migrations need to be initiated
        //    new DocumentStoreClient<ReferenceDataProduct>(
        //    new AzureBlobDocumentStoreProvider(
        //        sp.GetRequiredService<ILoggerFactory>(),
        //        services.BuildServiceProvider().GetRequiredService<IOptions<ShoppingModuleConfiguration>>().Value.ConnectionStrings.GetValue("StorageAccount"),
        //        this.Name)));

        //services.AddScoped<IDocumentStoreClient<ReferenceDataProduct>>(sp => // TODO: migrations need to be initiated
        //    new DocumentStoreClient<ReferenceDataProduct>(
        //    new AzureTableDocumentStoreProvider(
        //        sp.GetRequiredService<ILoggerFactory>(),
        //        services.BuildServiceProvider().GetRequiredService<IOptions<ShoppingModuleConfiguration>>().Value.ConnectionStrings.GetValue("StorageAccount"),
        //        this.Name)));

        services.AddScoped<IDocumentStoreClient<ReferenceDataProduct>>(sp =>
            new DocumentStoreClient<ReferenceDataProduct>(
            new EntityFrameworkDocumentStoreProvider<ShoppingDbContext>(sp.GetRequiredService<ShoppingDbContext>())))
            .Decorate<IDocumentStoreClient<ReferenceDataProduct>, LoggingDocumentStoreClientBehavior<ReferenceDataProduct>>()
            .Decorate<IDocumentStoreClient<ReferenceDataProduct>>((inner, sp) =>
                new TimeoutDocumentStoreClientBehavior<ReferenceDataProduct>(sp.GetRequiredService<ILoggerFactory>(), inner, new TimeoutDocumentStoreClientBehaviorOptions { Timeout = 30.Seconds() }));

        services.AddEntityFrameworkRepository<Cart, ShoppingDbContext>()
            .WithTransactions()
            // behaviors order: last..first
            .WithBehavior<RepositoryDomainEventPublisherBehavior<Cart>>()
            .WithBehavior<RepositoryDomainEventBehavior<Cart>>()
            .WithBehavior<RepositoryNoTrackingBehavior<Cart>>()
            .WithBehavior<RepositoryLoggingBehavior<Cart>>()
            .WithBehavior<RepositoryTracingBehavior<Cart>>()
            .WithBehavior((inner) => new RepositoryIncludeBehavior<Cart>(e => e.Items, inner));

        return services;
    }
}