namespace Presentation.Web.Client.Pages;

using BridgingIT.DevKit.Presentation.Web.Client;
using Common.SignalR;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor;

public partial class IndexPage
{
    private HubConnection hubConnection;
    //private bool isTokenValid;
    //private DateTimeOffset tokenExpiration;
    private string authToken;

    private string permissions = "";

    private bool IsConnected { get; set; }

    [CascadingParameter]
    protected Task<AuthenticationState> AuthState { get; set; } = default!;

    [Inject]
    private IAccessTokenProvider TokenProvider { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        this.hubConnection = this.hubConnection.TryInitialize(this.navManager, this.TokenProvider, this.loggerProvider); // TODO: inject this hubconnection?
        await this.hubConnection.Start();

        this.IsConnected = this.hubConnection.State == HubConnectionState.Connected;
        //this.isTokenValid = await this.authManager.IsTokenValidAsync();
        //this.tokenExpiration = await this.authManager.GetTokenExpirationAsync();
        this.authToken = await this.TokenProvider.GetAccessTokenAsync();
        var user = (await this.AuthState).User;
        if (user.Identity?.IsAuthenticated == true)
        {
            foreach (var permission in user.Claims.Where(c => c.Type == "Permission").Select(c => c.Value))
            {
                this.permissions += permission + ", ";
            }
        }
    }

    private async Task CopyText(string text)
    {
        if (!string.IsNullOrEmpty(text))
        {
            await this.jsRuntime.CopyToClipboardAsync(text);
            this.snackbar.Add("Copied!", Severity.Info);
        }
    }
}