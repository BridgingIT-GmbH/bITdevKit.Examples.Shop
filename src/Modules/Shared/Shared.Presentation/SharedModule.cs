namespace Modules.Shared.Presentation;

using BridgingIT.DevKit.Application;
using BridgingIT.DevKit.Application.Collaboration;
using BridgingIT.DevKit.Application.JobScheduling;
using BridgingIT.DevKit.Common;
using Common.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Modules.Shared.Application;

public class SharedModule : WebModuleBase
{
    public SharedModule()
        : base(priority: 0)
    {
    }

    public override IServiceCollection Register(IServiceCollection services, IConfiguration configuration = null, IWebHostEnvironment environment = null)
    {
        var moduleConfiguration = services.Configure<SharedModuleConfiguration, SharedModuleConfiguration.Validator>(configuration, this);

        services.AddMailService(services.BuildServiceProvider().GetRequiredService<IOptions<SharedModuleConfiguration>>().Value.Mail);
        services.AddScoped<IExcelInterchangeService, EPPlusExcelInterchangeService>();

        services.AddJobScheduling()
            .WithScopedJob<EchoJob>(CronExpressions.Every15Minutes)
            .WithScopedJob<EchoMessageJob>(CronExpressions.Every30Minutes) // publishes the EchoMessage to the messagebroker
            .WithBehavior<ModuleScopeJobSchedulingBehavior>()
            .WithBehavior<RetryJobSchedulingBehavior>();

        services.AddMessaging(configuration)
            .WithSubscription<EchoMessage, EchoMessageHandler>(); // handler for the EchoMessage sent by the EchoMessageJob

        return services;
    }

    public override IApplicationBuilder Use(IApplicationBuilder app, IConfiguration configuration = null, IWebHostEnvironment environment = null)
    {
        return app;
    }

    public override IEndpointRouteBuilder Map(IEndpointRouteBuilder app, IConfiguration configuration = null, IWebHostEnvironment environment = null)
    {
        //TODO: currently nswag does not generate swagger for minimal endpoints when versioning is also used https://github.com/RicoSuter/NSwag/pull/3814
        //      solution https://github.com/RicoSuter/NSwag/issues/3945 >> + https://github.com/JKamsker/Versioned-Minimal-and-Controller-AspNetCore
        //app.MapGet("/_system/correlationid", (HttpContext ctx) => ctx.TryGetCorrelationId());

        app.MapGet("/_system/hello", (Func<string>)(() => "Hello World!"))
            .WithTags("General");

        app.MapGet("/_system/sum/{a}/{b}", (Func<int, int, int>)((a, b) => a + b))
            .WithName("CalculateSum")
            .WithTags("Calculator");

        return app;
    }
}