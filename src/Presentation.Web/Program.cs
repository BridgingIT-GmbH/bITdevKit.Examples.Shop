using System.Net;
using Azure.Monitor.OpenTelemetry.Exporter;
using BridgingIT.DevKit.Application.Commands;
using BridgingIT.DevKit.Application.Entities;
using BridgingIT.DevKit.Application.JobScheduling;
using BridgingIT.DevKit.Application.Messaging;
using BridgingIT.DevKit.Application.Queries;
using BridgingIT.DevKit.Application.Utilities;
using BridgingIT.DevKit.Common;
using BridgingIT.DevKit.Presentation;
using BridgingIT.DevKit.Presentation.Web;
using Common.SignalR;
using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using NSwag;
using NSwag.AspNetCore;
using NSwag.Generation.AspNetCore;
using NSwag.Generation.Processors.Security;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Presentation.Web;
using Serilog;

// ===============================================================================================
// Create the webhost
var builder = WebApplication.CreateBuilder(args);
builder.Host.ConfigureLogging();
builder.Host.ConfigureAppConfiguration();

// ===============================================================================================
// Configure the modules
builder.Services.AddModules(builder.Configuration, builder.Environment)
    .WithModule<Modules.Shared.Presentation.SharedModule>()
    .WithModule<Modules.Catalog.Presentation.CatalogModule>()
    .WithModule<Modules.Identity.Presentation.IdentityModule>()
    .WithModule<Modules.Inventory.Presentation.InventoryModule>()
    .WithModule<Modules.Ordering.Presentation.OrderingModule>()
    .WithModule<Modules.Shopping.Presentation.ShoppingModule>()
    .WithModuleContextAccessors()
    .WithRequestModuleContextAccessors()
    .WithModuleControllers(c => c // alternative: WithModuleFeatureProvider(c => ...)
        .AddJsonOptions(ConfigureJsonOptions)
        .AddOData(o => o.Select().Filter().Count().OrderBy().Expand()));

// ===============================================================================================
// Configure the services
builder.Services.AddMediatR(); // or AddDomainEvents()?
builder.Services.AddMapping().WithMapster();
builder.Services.AddCaching(builder.Configuration)
    //.WithEntityFrameworkDocumentStoreProvider<CoreDbContext>()
    //.WithAzureBlobDocumentStoreProvider()
    //.WithAzureTableDocumentStoreProvider()
    //.WithCosmosDbDocumentStoreProvider()
    .WithInMemoryProvider();

builder.Services.AddCommands()
    .WithBehavior(typeof(ModuleScopeCommandBehavior<,>))
    //.WithBehavior(typeof(ChaosExceptionCommandBehavior<,>))
    .WithBehavior(typeof(RetryCommandBehavior<,>))
    .WithBehavior(typeof(CircuitBreakerCommandBehavior<,>))
    .WithBehavior(typeof(TimeoutCommandBehavior<,>))
    .WithBehavior(typeof(CacheInvalidateCommandBehavior<,>)); ;
builder.Services.AddQueries()
    .WithBehavior(typeof(ModuleScopeQueryBehavior<,>))
    //.WithBehavior(typeof(ChaosExceptionQueryBehavior<,>))
    .WithBehavior(typeof(CacheQueryBehavior<,>))
    .WithBehavior(typeof(RetryQueryBehavior<,>))
    .WithBehavior(typeof(CircuitBreakerQueryBehavior<,>))
    .WithBehavior(typeof(TimeoutQueryBehavior<,>));

builder.Services.AddJobScheduling(o => o.StartupDelay("00:00:10"))
    .WithBehavior<ModuleScopeJobSchedulingBehavior>()
    //.WithBehavior<ChaosExceptionJobSchedulingBehavior>()
    .WithBehavior<RetryJobSchedulingBehavior>()
    .WithBehavior<TimeoutJobSchedulingBehavior>();

builder.Services.AddStartupTasks(o => o.Enabled().StartupDelay("00:00:05"))
    .WithTask<EchoStartupTask>(o => o.Enabled().StartupDelay("00:00:03"))
    .WithBehavior<ModuleScopeStartupTaskBehavior>()
    //.WithBehavior<ChaosExceptionStartupTaskBehavior>()
    .WithBehavior<RetryStartupTaskBehavior>()
    .WithBehavior<TimeoutStartupTaskBehavior>();

builder.Services.AddMessaging(builder.Configuration, o => o
        .StartupDelay("00:00:10"))
    .WithBehavior<ModuleScopeMessagePublisherBehavior>()
    .WithBehavior<ModuleScopeMessageHandlerBehavior>()
    .WithBehavior<TimeoutMessageHandlerBehavior>()
    .WithBehavior<RetryMessageHandlerBehavior>()
    //.WithRabbitMQBroker(services.BuildServiceProvider().GetRequiredService<IOptions<SharedModuleConfiguration>>().Value.MessagingRabbitMQ)
    //    .Services.AddHealthChecks()
    //             .AddRabbitMQ(services.BuildServiceProvider().GetRequiredService<IOptions<SharedModuleConfiguration>>().Value.MessagingRabbitMQ.ConnectionString, name: $"rabbitmq ({this.Name})", tags: new[] { "ready" });
    //.WithPulsarBroker(services.BuildServiceProvider().GetRequiredService<IOptions<SharedModuleConfiguration>>().Value.MessagingPulsar);
    .WithInProcessBroker(); // 2.3.33: WithInProcessBroker()

ConfigureHealth(builder.Services); // NET 7.0 runtime issue

builder.Services.AddApplicationInsightsTelemetry(); // https://docs.microsoft.com/en-us/azure/azure-monitor/app/asp-net-core
builder.Services.AddOpenTelemetry()
    //.WithMetrics(...)
    .WithTracing(ConfigureTracing);

builder.Services.Configure<ApiBehaviorOptions>(ConfigureApiBehavior);
builder.Services.AddSingleton<IConfigurationRoot>(builder.Configuration);
builder.Services.AddProblemDetails(options =>
{
    //options.MapToStatusCode<EntityCommandRuleNotSatisfied>(StatusCodes.Status400BadRequest);
    options.Map<EntityCommandRuleNotSatisfied>(ex => // TODO: move to Configure.ProblemDetails
    {
        return new ValidationProblemDetails
        {
            Title = "Bad Request",
            Status = (int)HttpStatusCode.BadRequest,
            Detail = $"[{ex.GetType().Name}] {ex.Message}",
            Type = "https://httpstatuses.com/400"
        };
    });

    Configure.ProblemDetails(options, true);
});

builder.Services.AddRazorPages();
builder.Services.AddSignalR();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApiDocument(ConfigureOpenApiDocument); // TODO: still needed when all OpenAPI specifications are available in swagger UI?

builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

// ===============================================================================================
// Configure the HTTP request pipeline
var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseProblemDetails();
app.UseRouting();

app.UseRequestCorrelation();
app.UseRequestModuleContext();
app.UseRequestLogging();
app.UseRequestCultureLocalization();

app.UseOpenApi();
app.UseSwaggerUi(ConfigureSwaggerUi);

//app.UseResponseCompression();
app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();

var provider = new FileExtensionContentTypeProvider();
provider.Mappings.Add(".yaml", ContentType.YAML.MimeType());
app.UseStaticFiles(new StaticFileOptions
{
    ContentTypeProvider = provider,
    OnPrepareResponse = context =>
    {
        if (context.Context.Response.ContentType == ContentType.YAML.MimeType()) // Disable caching for yaml (OpenAPI) files
        {
            context.Context.Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
            context.Context.Response.Headers["Expires"] = "-1";
            context.Context.Response.Headers["Pragma"] = "no-cache";
        }
    }
});

app.UseModules();

app.UseAuthentication(); // TODO: move to IdentityModule
app.UseAuthorization(); // TODO: move to IdentityModule

app.MapModules();
app.MapRazorPages();
app.MapControllers();
app.MapHealthChecks();
app.MapFallbackToFile("index.html");
app.MapHub<SignalRHub>(SignalRHubConstants.HubUrl);

//#if DEBUG
//app.MapGet("/api/_system/configuration", async context =>
//{
//    await context.Response.WriteAsJsonAsync(
//        context.RequestServices.GetRequiredService<IConfiguration>().AsEnumerable(),
//        new JsonSerializerOptions { WriteIndented = true });
//});
//#endif

app.Run();

void ConfigureApiBehavior(ApiBehaviorOptions options)
{
    options.SuppressModelStateInvalidFilter = true;
}

void ConfigureJsonOptions(JsonOptions options)
{
    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
}

void ConfigureHealth(IServiceCollection services)
{
    services.AddHealthChecks()
        .AddCheck("self", () => HealthCheckResult.Healthy(), tags: new[] { "live" });
        //.AddCheck<RandomHealthCheck>("random")
        //.AddSeqPublisher(s => s.Endpoint = builder.Configuration["Serilog:SeqServerUrl"]);
        // ^^ NET 7.0 runtime issue
        //.AddApplicationInsightsPublisher()

    // TODO: .NET8 issue with HealthChecks name conflic https://github.com/dotnet/aspnetcore/issues/50836
    services.AddHealthChecksUI() // https://github.com/Xabaril/AspNetCore.Diagnostics.HealthChecks/blob/master/README.md
      .AddInMemoryStorage();
    //   //.AddSqliteStorage($"Data Source=data_health.db");
}

void ConfigureTracing(TracerProviderBuilder c)
{
    // TODO: multiple per module tracer needed? https://github.com/open-telemetry/opentelemetry-dotnet/issues/2040
    // https://opentelemetry.io/docs/instrumentation/net/getting-started/
    c.SetSampler(new AlwaysOnSampler())
      //.AddSource(ModuleExtensions.Modules.Select(m => m.Name).Insert(serviceName).ToArray()) // TODO: provide a nice (module) extension for this -> .AddModuleSources() // NOT NEEDED, * will add all activitysources
      .AddSource("*")
      .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(ModuleExtensions.ServiceName))
      .SetErrorStatusOnException(true)
      .AddAspNetCoreInstrumentation(options =>
      {
          options.RecordException = true;
          options.Filter = context => !context.Request.Path.ToString().EqualsPatternAny(new RequestLoggingOptions().PathBlackListPatterns);
      })
      .AddHttpClientInstrumentation(options =>
      {
          options.RecordException = true;
          options.FilterHttpRequestMessage = request => !request.RequestUri.PathAndQuery.EqualsPatternAny(new RequestLoggingOptions().PathBlackListPatterns.Insert("*api/events/raw*"));
      })
      .AddSqlClientInstrumentation(options =>
      {
          options.EnableConnectionLevelAttributes = true;
          options.RecordException = true;
          options.SetDbStatementForText = true;
      });

    if (builder.Configuration["Tracing:Jaeger:Enabled"].IsNullOrEmpty() || builder.Configuration["Tracing:Jaeger:Enabled"].SafeEquals("True"))
    {
        Log.Logger.Information("{LogKey} jaeger exporter enabled (host={JaegerHost})", "TRC", builder.Configuration["Tracing:Jaeger:AgentHost"]);
        c.AddJaegerExporter(opts =>
        {
            opts.AgentHost = builder.Configuration["Tracing:Jaeger:AgentHost"];
            opts.AgentPort = Convert.ToInt32(builder.Configuration["Tracing:Jaeger:AgentPort"]);
            opts.ExportProcessorType = ExportProcessorType.Simple;
        });
    }

    if (builder.Configuration["Tracing:Console:Enabled"].IsNullOrEmpty() || builder.Configuration["Tracing:Console:Enabled"].SafeEquals("True"))
    {
        Log.Logger.Information("{LogKey} console exporter enabled", "TRC");
        c.AddConsoleExporter();
    }

    if (builder.Configuration["Tracing:AzureMonitor:Enabled"].IsNullOrEmpty() || builder.Configuration["Tracing:AzureMonitor:Enabled"].SafeEquals("True"))
    {
        Log.Logger.Information("{LogKey} azuremonitor exporter enabled", "TRC");
        c.AddAzureMonitorTraceExporter(o => o
            .ConnectionString = builder.Configuration["Tracing:AzureMonitor:ConnectionString"].EmptyToNull() ?? Environment.GetEnvironmentVariable("APPLICATIONINSIGHTS_CONNECTION_STRING"));
    }
}

void ConfigureOpenApiDocument(AspNetCoreOpenApiDocumentGeneratorSettings settings)
{
    settings.AddSecurity("JWT", Enumerable.Empty<string>(), new OpenApiSecurityScheme
    {
        Name = "Authorization",
        In = OpenApiSecurityApiKeyLocation.Header,
        Type = OpenApiSecuritySchemeType.ApiKey,
        BearerFormat = "JWT",
        Description = "Input your JWT token in this format - Bearer {your token here} to access this API",
    });

    settings.DocumentName = "generated";
    settings.Version = "v1";
    settings.Title = "Backend API";
    //settings.OperationProcessors.Add(new AuthorizeRolesSummaryOperationProcessor());
    settings.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor("JWT"));
    //settings.OperationProcessors.Add(new OperationSecurityScopeProcessor("bearer"));
    //settings.OperationProcessors.Add(new AuthorizationOperationProcessor("bearer"));
}

void ConfigureSwaggerUi(SwaggerUiSettings settings)
{
    settings.CustomStylesheetPath = "css/swagger.css";
    settings.SwaggerRoutes.Add(new SwaggerUiRoute("All (generated)", "/swagger/generated/swagger.json")); // TODO: still needed when all OpenAPI specifications are available in swagger UI?

    foreach (var module in ModuleExtensions.Modules.SafeNull().Where(m => m.Enabled))
    {
        settings.SwaggerRoutes.Add(
            new SwaggerUiRoute(module.Name, $"/openapi/{module.Name}-OpenAPI.yaml"));
    }
}

public partial class Program
{
    // this partial class is needed to set the accessibilty for the Program class to public
    // needed for testing with a test fixture https://learn.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-7.0#basic-tests-with-the-default-webapplicationfactory
}