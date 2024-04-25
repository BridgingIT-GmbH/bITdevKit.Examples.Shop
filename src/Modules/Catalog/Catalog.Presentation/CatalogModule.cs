namespace Modules.Catalog.Presentation;

using BridgingIT.DevKit.Application;
using BridgingIT.DevKit.Common;
using BridgingIT.DevKit.Domain.Repositories;
using BridgingIT.DevKit.Infrastructure.EntityFramework.Repositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Modules.Catalog.Application;
using Modules.Catalog.Application.Integration;
using Modules.Catalog.Domain;
using Modules.Catalog.Infrastructure;
using Modules.Catalog.Infrastructure.EntityFramework;
using Modules.Catalog.Infrastructure.OData;

public class CatalogModule : WebModuleBase
{
    public override IServiceCollection Register(IServiceCollection services, IConfiguration configuration = null, IWebHostEnvironment environment = null)
    {
        var moduleConfiguration = services.Configure<CatalogModuleConfiguration, CatalogModuleConfiguration.Validator>(configuration, this);

        services.AddJobScheduling()
            .WithScopedJob<CatalogImportJob>(CronExpressions.Every30Minutes);

        services.AddStartupTasks()
            .WithTask<CatalogDomainSeederTask>(o => o.Enabled().StartupDelay("00:00:05"));

        services.AddMessaging()
            .WithSubscription<EchoMessage, EchoMessageHandler>() // handler for the EchoMessage sent by the EchoMessageJob
            .WithSubscription<CatalogImportedMessage, CatalogImportedMessageHandler>();

        services.AddSqlServerDbContext<CatalogDbContext>(o => o
                .UseConnectionString(services.BuildServiceProvider().GetRequiredService<IOptions<CatalogModuleConfiguration>>().Value.ConnectionStrings.GetValue("SqlServer"))
                .UseLogger())
            //.WithHealthChecks()
            .WithDatabaseMigratorService();

        services.AddEntityFrameworkRepository<Brand, CatalogDbContext>()
            // behaviors order: last..first
            .WithBehavior<RepositoryDomainEventPublisherBehavior<Brand>>()
            .WithBehavior<RepositoryDomainEventBehavior<Brand>>()
            .WithBehavior<RepositoryNoTrackingBehavior<Brand>>()
            .WithBehavior<RepositoryLoggingBehavior<Brand>>()
            .WithBehavior<RepositoryTracingBehavior<Brand>>();

        services.AddEntityFrameworkRepository<Product, CatalogDbContext>()
            // behaviors order: last..first
            .WithBehavior<RepositoryDomainEventPublisherBehavior<Product>>()
            .WithBehavior<RepositoryDomainEventBehavior<Product>>()
            .WithBehavior<RepositoryNoTrackingBehavior<Product>>()
            .WithBehavior<RepositoryLoggingBehavior<Product>>()
            .WithBehavior<RepositoryTracingBehavior<Product>>()
            .WithBehavior((inner) => new RepositoryIncludeBehavior<Product>(e => e.Brand, inner))
            .WithBehavior((inner) => new RepositoryIncludeBehavior<Product>(e => e.Type, inner));

        services.AddEntityFrameworkRepository<ProductType, CatalogDbContext>()
            // behaviors order: last..first
            .WithBehavior<RepositoryNoTrackingBehavior<ProductType>>()
            .WithBehavior<RepositoryLoggingBehavior<ProductType>>()
            .WithBehavior<RepositoryTracingBehavior<ProductType>>();

        // Catalog data adapter (= anti corruption)
        //services.AddScoped<ICatalogDataAdapter>(sp =>
        //    new BrocadeCatalogDataAdapter(
        //        sp.GetRequiredService<ILoggerFactory>(),
        //        sp.GetRequiredService<IHttpClientFactory>(),
        //        moduleConfiguration.BrocadeApiKey))
        //    .Decorate<ICatalogDataAdapter>((d) => TraceActivityBehavior<ICatalogDataAdapter>.Create(d))
        //    .AddHttpClient("BrocadeClient", c =>
        //    {
        //        c.BaseAddress = new Uri(moduleConfiguration.BrocadeUrl);
        //        c.DefaultRequestHeaders.Add("Accept", System.Net.Mime.MediaTypeNames.Application.Json);
        //    });

        services.AddScoped<ICatalogDataAdapter>(sp =>
            new DummyJsonCatalogDataAdapter(
                sp.GetRequiredService<ILoggerFactory>(),
                sp.GetRequiredService<IHttpClientFactory>()))
            .Decorate<ICatalogDataAdapter>((d) => TraceActivityDecorator<ICatalogDataAdapter>.Create(d))
            .AddHttpClient("DummyJsonClient", c =>
            {
                c.BaseAddress = new Uri(moduleConfiguration.DummyJsonProductsUrl);
                c.DefaultRequestHeaders.Add("Accept", System.Net.Mime.MediaTypeNames.Application.Json);
            });

        services.AddScoped<IODataAdapter, ODataAdapter>();

        return services;
    }
}