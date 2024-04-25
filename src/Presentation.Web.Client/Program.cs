using System.Globalization;
using System.Net.Http.Headers;
using BlazorApplicationInsights;
using Blazored.LocalStorage;
using Common;
using Common.Presentation.Web.Client;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Modules.Catalog.Presentation.Web.Client;
using Modules.Identity.Presentation.Web.Client;
using Modules.Inventory.Presentation.Web.Client;
using Modules.Ordering.Presentation.Web.Client;
using Modules.Shopping.Presentation.Web.Client;
using MudBlazor;
using MudBlazor.Services;
using Polly;
using Presentation.Web.Client;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Logging.AddConfiguration( // https://learn.microsoft.com/en-us/aspnet/core/blazor/fundamentals/configuration?view=aspnetcore-7.0#logging-configuration
    builder.Configuration.GetSection("Logging"));

builder.Services
    .AddConfiguration()
    .AddBlazoredLocalStorage()
    .AddLocalization(options => options.ResourcesPath = "Resources")
    //.AddBlazorApplicationInsights(async applicationInsights =>
    //{
    //    var telemetryItem = new TelemetryItem()
    //    {
    //        Tags = new Dictionary<string, object>()
    //        {
    //            { "ai.cloud.role", "SPA" },
    //            { "ai.cloud.roleInstance", "Blazor Wasm" },
    //        }
    //    };

    //    //var key = builder.Configuration.GetSection("ApplicationInsights").GetSection("InstrumentationKey").Value; // https://github.com/IvanJosipovic/BlazorApplicationInsights/issues/109
    //    await applicationInsights.SetConnectionString("InstrumentationKey=2b3e0b23-188d-4cc3-9a38-8f0ef15e2126;");
    //    await applicationInsights.AddTelemetryInitializer(telemetryItem);
    //})
    .AddMudServices(configuration =>
    {
        configuration.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomRight;
        configuration.SnackbarConfiguration.HideTransitionDuration = 100;
        configuration.SnackbarConfiguration.ShowTransitionDuration = 100;
        configuration.SnackbarConfiguration.VisibleStateDuration = 3000;
        configuration.SnackbarConfiguration.ShowCloseIcon = false;
    })
    .AddAuthentication(builder.Configuration)
    .AddScoped<IClientPreferenceManager, ClientPreferenceManager>(); // TODO: need interface?

builder.Services.AddHttpClient(HttpClientConstants.ApiAuthName)
    .ConfigureHttpClient(client =>
    {
        client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(System.Net.Mime.MediaTypeNames.Application.Json));
        client.DefaultRequestHeaders.AcceptLanguage.Clear();
        client.DefaultRequestHeaders.AcceptLanguage.ParseAdd(CultureInfo.DefaultThreadCurrentCulture?.TwoLetterISOLanguageName);
    })
    .AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromSeconds(30)));

if (builder.Configuration["App:Modules:Identity:Provider"] == "AzureAd")
{
    Console.WriteLine("builder ::::::::::::: AzureAd");
    builder.Services.AddHttpClient(HttpClientConstants.ApiClientName)
    .ConfigureHttpClient(client =>
    {
        client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(System.Net.Mime.MediaTypeNames.Application.Json));
        client.DefaultRequestHeaders.AcceptLanguage.Clear();
        client.DefaultRequestHeaders.AcceptLanguage.ParseAdd(CultureInfo.DefaultThreadCurrentCulture?.TwoLetterISOLanguageName);
    })
    //.AddTransientHttpErrorPolicy(builder => builder.WaitAndRetryAsync(new[]
    //{
    //    TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(5)
    //}))
    //.AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromSeconds(15)))
    //    sp.GetRequiredService<ILocalStorageService>()));
    // TODO: add auth exception handler (snackbar) + redirect on 401 (refreshtoken expired)

    .AddHttpMessageHandler<AzureAdAuthorizationMessageHandler>();
}
else
{
    Console.WriteLine("builder ::::::::::::: Jwt");
    builder.Services.AddHttpClient(HttpClientConstants.ApiClientName)
    .ConfigureHttpClient(client =>
    {
        client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(System.Net.Mime.MediaTypeNames.Application.Json));
        client.DefaultRequestHeaders.AcceptLanguage.Clear();
        client.DefaultRequestHeaders.AcceptLanguage.ParseAdd(CultureInfo.DefaultThreadCurrentCulture?.TwoLetterISOLanguageName);
    })
    //.AddTransientHttpErrorPolicy(builder => builder.WaitAndRetryAsync(new[]
    //{
    //    TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(5)
    //}))
    //.AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromSeconds(15)))
    //.AddHttpMessageHandler(sp => new JwtAuthenticationRefreshHandler(
    //    sp.GetRequiredService<ILogger<JwtAuthenticationRefreshHandler>>(),
    //    sp.GetRequiredService<IAuthenticationManager>(),
    //    sp.GetRequiredService<NavigationManager>()))
    //.AddHttpMessageHandler(sp => new JwtAuthenticationHeaderHandler(
    //    sp.GetRequiredService<ILogger<JwtAuthenticationHeaderHandler>>(),
    //    sp.GetRequiredService<ILocalStorageService>()));
    // TODO: add auth exception handler (snackbar) + redirect on 401 (refreshtoken expired)

    .AddHttpMessageHandler<JwtAuthenticationRefreshHandler>()
    .AddHttpMessageHandler<JwtAuthenticationHeaderHandler>();
}

builder.Services
    .AddScoped<ICatalogApiClient>(sp => new CatalogApiClient(sp.GetRequiredService<IHttpClientFactory>().CreateClient(HttpClientConstants.ApiClientName)))
    .AddScoped<IIdentityApiClient>(sp => new IdentityApiClient(sp.GetRequiredService<IHttpClientFactory>().CreateClient(HttpClientConstants.ApiClientName)))
    .AddScoped<IInventoryApiClient>(sp => new InventoryApiClient(sp.GetRequiredService<IHttpClientFactory>().CreateClient(HttpClientConstants.ApiClientName)))
    .AddScoped<IOrderingApiClient>(sp => new OrderingApiClient(sp.GetRequiredService<IHttpClientFactory>().CreateClient(HttpClientConstants.ApiClientName)))
    .AddScoped<IShoppingApiClient>(sp => new ShoppingApiClient(sp.GetRequiredService<IHttpClientFactory>().CreateClient(HttpClientConstants.ApiClientName)));

var host = builder.Build();
await host.UseLocalization(LocalizationConstants.SupportedLanguages.FirstOrDefault()?.Code);
await host.RunAsync();