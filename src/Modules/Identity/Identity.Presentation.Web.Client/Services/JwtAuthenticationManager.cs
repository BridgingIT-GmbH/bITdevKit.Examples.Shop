namespace Modules.Identity.Presentation.Web.Client;

using System.Net.Http.Json;
using BridgingIT.DevKit.Common;
using Blazored.LocalStorage;
using Common;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

public class JwtAuthenticationManager : IAuthenticationManager
{
    private readonly ILogger<JwtAuthenticationManager> logger;
    private readonly ILocalStorageService localStorage;
    private readonly AuthenticationStateProvider authStateProvider;
    private readonly NavigationManager navManager;
    private readonly IStringLocalizer<JwtAuthenticationManager> localizer;
    private readonly HttpClient httpClient;

    public JwtAuthenticationManager(
        ILogger<JwtAuthenticationManager> logger,
        IHttpClientFactory httpClientFactory,
        ILocalStorageService localStorage,
        AuthenticationStateProvider authStateProvider,
        NavigationManager navManager,
        IStringLocalizer<JwtAuthenticationManager> localizer)
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

    public async Task<Result> LoginAsync(string email, string password)
    {
        this.logger.LogInformation("auth: acquire token");
        var response = await this.httpClient.PostAsJsonAsync(ApiConstants.TokenAcquireRoute, new TokenRequestModel { Email = email, Password = password });
        var result = await response.Content.ReadFromJsonAsync<ResultOfTokenResponseModel>();
        if (result?.IsSuccess == true)
        {
            await this.localStorage.SetItemAsync(StorageConstants.Local.AuthToken, result.Value.Token);
            await this.localStorage.SetItemAsync(StorageConstants.Local.RefreshToken, result.Value.RefreshToken);
            if (!string.IsNullOrEmpty(result.Value.UserImageURL))
            {
                await this.localStorage.SetItemAsync(StorageConstants.Local.UserImageURL, result.Value.UserImageURL);
            }

            await ((JwtAuthenticationStateProvider)this.authStateProvider).StateChangedAsync();
            this.logger.LogInformation("auth: authenticated");

            return Result.Success();
        }
        else
        {
            return Result.Failure(result?.Messages?.ToList());
        }
    }

    public async Task<Result> LogoutAsync()
    {
        this.logger.LogInformation("auth: logout > remove tokens");
        await this.localStorage.RemoveItemAsync(StorageConstants.Local.AuthToken);
        await this.localStorage.RemoveItemAsync(StorageConstants.Local.RefreshToken);
        await this.localStorage.RemoveItemAsync(StorageConstants.Local.UserImageURL);

        ((JwtAuthenticationStateProvider)this.authStateProvider).SetNotAuthenticated();
        this.logger.LogInformation("auth: logout > not authenticated");

        return Result.Success();
    }

    public async Task<string> RefreshTokenAsync()
    {
        this.logger.LogInformation("auth: refresh token");
        var response = await this.httpClient.PostAsJsonAsync(
            ApiConstants.TokenRefreshRoute,
            new TokenRefreshRequestModel
            {
                Token = await this.localStorage.GetItemAsync<string>(StorageConstants.Local.AuthToken),
                RefreshToken = await this.localStorage.GetItemAsync<string>(StorageConstants.Local.RefreshToken)
            });
        var result = await response.Content.ReadFromJsonAsync<ResultOfTokenResponseModel>();
        if ((result?.IsSuccess) != false)
        {
            this.logger.LogInformation("auth: refresh token success");
            await this.localStorage.SetItemAsync(StorageConstants.Local.AuthToken, result.Value?.Token);
            await this.localStorage.SetItemAsync(StorageConstants.Local.RefreshToken, result.Value?.RefreshToken);
            return result.Value?.Token;
        }
        else
        {
            // token could not be refreshed (for example: refresh token expired)
            this.logger.LogWarning("auth: refresh token failed > {message}", result?.Messages?.FirstOrDefault());
            await this.LogoutAsync();
            this.navManager.NavigateTo($"{IdentityPageConstants.Login}?redirectUrl={Uri.EscapeDataString(this.navManager.Uri)}");
            //throw new SecurityException(this.localizer["Something went wrong during the refresh token action"]);
            return result.Value?.Token;
        }
    }

    public async Task<string> TryRefreshTokenAsync()
    {
        if (!await this.IsTokenValidAsync())
        {
            return await this.RefreshTokenAsync();
        }

        return string.Empty;
    }

    public async Task<string> TryForceRefreshTokenAsync() => await this.RefreshTokenAsync();

    public async Task<bool> IsTokenValidAsync()
    {
        var token = await this.localStorage.GetItemAsync<string>(StorageConstants.Local.RefreshToken);
        if (string.IsNullOrEmpty(token)) //check if token exists
        {
            return false;
        }

        var authState = await this.authStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;
        var exp = user.FindFirst(c => c.Type.Equals("exp"))?.Value;
        var expTime = DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(exp));
        var timeUTC = DateTime.UtcNow;
        var diff = expTime - timeUTC;
        return diff.TotalMinutes > 1;
    }

    public async Task<DateTimeOffset> GetTokenExpirationAsync()
    {
        var token = await this.localStorage.GetItemAsync<string>(StorageConstants.Local.RefreshToken);
        if (string.IsNullOrEmpty(token)) //check if token exists
        {
            return default;
        }

        var authState = await this.authStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;
        var exp = user.FindFirst(c => c.Type.Equals("exp"))?.Value;
        return DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(exp));
    }

    public async Task<string> GetToken() => await this.localStorage.GetItemAsync<string>(StorageConstants.Local.AuthToken);
}
