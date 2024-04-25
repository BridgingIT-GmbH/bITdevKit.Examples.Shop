namespace Modules.Identity.Presentation.Web.Client.Components;

using Common.SignalR;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor;

public partial class Logout
{
    [CascadingParameter]
    public MudDialogInstance MudDialog { get; set; }

    [Parameter]
    public HubConnection HubConnection { get; set; }

    [Parameter]
    public string ContentText { get; set; }

    [Parameter]
    public string ButtonText { get; set; }

    [Parameter]
    public Color Color { get; set; }

    [Parameter]
    public string CurrentUserId { get; set; }

    private async Task Submit()
    {
        if (this.HubConnection.State == HubConnectionState.Connected)
        {
            await this.HubConnection.SendAsync(SignalRHubConstants.OnDisconnect, this.CurrentUserId);
        }

        var result = await this.authManager.LogoutAsync();
        if (result.IsSuccess)
        {
            this.navManager.NavigateTo($"{IdentityPageConstants.Login}?redirectUrl={Uri.EscapeDataString(this.navManager.Uri)}");
            this.MudDialog.Close(DialogResult.Ok(true));
        }
    }

    private void Cancel() => this.MudDialog.Cancel();
}