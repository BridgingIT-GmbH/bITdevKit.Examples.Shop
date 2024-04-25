namespace Modules.Identity.Presentation.Web.Client;

using BridgingIT.DevKit.Common;
using Blazored.LocalStorage;
using Common;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

public class AzureAdAuthenticationManager : IAuthenticationManager
{
    private readonly ILogger<AzureAdAuthenticationManager> logger;
    private readonly ILocalStorageService localStorage;
    private readonly AuthenticationStateProvider authStateProvider;
    private readonly NavigationManager navManager;
    private readonly IStringLocalizer<AzureAdAuthenticationManager> localizer;
    private readonly HttpClient httpClient;

    public AzureAdAuthenticationManager(
        ILogger<AzureAdAuthenticationManager> logger,
        IHttpClientFactory httpClientFactory,
        ILocalStorageService localStorage,
        AuthenticationStateProvider authStateProvider,
        NavigationManager navManager,
        IStringLocalizer<AzureAdAuthenticationManager> localizer)
    {
        this.logger = logger;
        this.httpClient = httpClientFactory.CreateClient(HttpClientConstants.ApiAuthName);
        this.localStorage = localStorage;
        this.authStateProvider = authStateProvider;
        this.navManager = navManager;
        this.localizer = localizer;
    }

    public void NavigateToExternalLogin(string returnUrl) =>
        this.navManager.NavigateTo($"authentication/login?returnUrl={Uri.EscapeDataString(returnUrl)}");

    public Task<Result> LoginAsync(string email, string password)
    {
        throw new NotImplementedException();
    }

    public Task<Result> LogoutAsync()
    {
        this.navManager.NavigateToLogout("authentication/logout");

        return Task.FromResult(Result.Success());
    }

    public Task<string> RefreshTokenAsync()
    {
        //NavigateToExternalLogin(returnUrl);
        throw new NotImplementedException();
    }

    public Task<string> GetToken()
    {
        throw new NotImplementedException();
    }

    public Task<string> TryRefreshTokenAsync()
    {
        throw new NotImplementedException();
    }

    public Task<string> TryForceRefreshTokenAsync()
    {
        throw new NotImplementedException();
    }

    public Task<bool> IsTokenValidAsync()
    {
        throw new NotImplementedException();
    }

    public Task<DateTimeOffset> GetTokenExpirationAsync()
    {
        throw new NotImplementedException();
    }
}