namespace Common.SignalR;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;

public static class SignalRHubExtensions
{
    public static HubConnection TryInitialize(
        this HubConnection source,
        NavigationManager navManager,
        IAccessTokenProvider tokenProvider,
        //IAuthenticationManager authManager,
        //ILocalStorageService localStorage,
        ILoggerProvider loggerProvider)
    {
        source ??= new HubConnectionBuilder()
                .WithUrl(
                    navManager.ToAbsoluteUri(SignalRHubConstants.HubUrl), // TODO: from args, remove dependency
                                                                          //options => options.AccessTokenProvider = async () =>
                                                                          //{
                                                                          //    await authManager.TryRefreshTokenAsync(); // TODO: redirect to login if refresh token not valid anymore
                                                                          //    return await localStorage.GetItemAsync<string>(StorageConstants.Local.AuthToken);
                                                                          //}
                    options =>
                        options.AccessTokenProvider =
                            () => tokenProvider.GetAccessTokenAsync())
                .ConfigureLogging(logging => logging.AddProvider(loggerProvider))
                .WithAutomaticReconnect().Build();

        return source;
    }

    public static HubConnection TryInitialize(
        this HubConnection source,
        NavigationManager navManager)
    {
        source ??= new HubConnectionBuilder()
                .WithUrl(navManager.ToAbsoluteUri(SignalRHubConstants.HubUrl)).Build();

        return source;
    }

    public static async Task<HubConnection> Start(
        this HubConnection source)
    {
        if (source?.State == HubConnectionState.Disconnected)
        {
            try
            {
                await source.StartAsync();
            }
            catch (HttpRequestException)
            {
                // token issue?
            }
        }

        return source;
    }
}
