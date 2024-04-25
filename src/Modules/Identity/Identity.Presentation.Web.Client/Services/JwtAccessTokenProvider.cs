namespace Modules.Identity.Presentation.Web.Client;
using System.Threading;
using Blazored.LocalStorage;
using Common;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

public class JwtAccessTokenProvider : IAccessTokenProvider
{
    private readonly SemaphoreSlim semaphore = new(1, 1);
    private readonly AuthenticationStateProvider authStateProvider;
    private readonly IAuthenticationManager authenticationManager;
    private readonly ILocalStorageService localStorage;

    public JwtAccessTokenProvider(
        AuthenticationStateProvider authStateProvider,
        IAuthenticationManager authenticationManager,
        ILocalStorageService localStorage)
    {
        this.authStateProvider = authStateProvider;
        this.authenticationManager = authenticationManager;
        this.localStorage = localStorage;
    }

    public async ValueTask<AccessTokenResult> RequestAccessToken()
    {
        var authState = await this.authStateProvider.GetAuthenticationStateAsync();
        if (authState.User.Identity?.IsAuthenticated is not true)
        {
            return new AccessTokenResult(AccessTokenResultStatus.RequiresRedirect, null, "/login", null);
        }

        await this.semaphore.WaitAsync();
        try
        {
            var token = await this.authenticationManager.RefreshTokenAsync();

            //if (!await this.IsTokenValidAsync(token))
            //{
            //    return new AccessTokenResult(AccessTokenResultStatus.RequiresRedirect, null, "/login", null);
            //}

            return new AccessTokenResult(AccessTokenResultStatus.Success, new AccessToken() { Value = token }, string.Empty, null);
        }
        finally
        {
            this.semaphore.Release();
        }
    }

    public ValueTask<AccessTokenResult> RequestAccessToken(AccessTokenRequestOptions options) =>
        this.RequestAccessToken();
}