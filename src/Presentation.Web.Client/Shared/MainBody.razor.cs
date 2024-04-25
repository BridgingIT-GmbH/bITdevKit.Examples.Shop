namespace Presentation.Web.Client.Shared;

using System.Security.Claims;
using Common.SignalR;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.SignalR.Client;
using Modules.Identity.Presentation.Web.Client;
using MudBlazor;

public partial class MainBody
{
    private ILogger logger;
    private HubConnection hubConnection;
    private bool drawerOpen = true;
    private bool isDarkMode;
    private ErrorBoundary errorBoundary;

    [CascadingParameter]
    protected Task<AuthenticationState> AuthState { get; set; } = default!;

    [Inject]
    private IAccessTokenProvider TokenProvider { get; set; } = default!;

    [Parameter]
    public RenderFragment ChildContent { get; set; }

    [Parameter]
    public EventCallback OnDarkModeToggle { get; set; }

    private char FirstLetterOfName { get; set; }

    private string CurrentUserId { get; set; }

    private string ImageDataUrl { get; set; }

    private string FirstName { get; set; }

    private string SecondName { get; set; }

    private string Email { get; set; }

    private bool IsConnected { get; set; }

    protected override async Task OnInitializedAsync()
    {
        this.logger = this.loggerFactory.CreateLogger("Presentation.Web");
        this.isDarkMode = await this.clientPreferenceManager.IsDarkMode();

        this.hubConnection = this.hubConnection.TryInitialize(this.navManager, this.TokenProvider, this.loggerProvider); // TODO: inject this hubconnection?

        //hubConnection.On<string, string, string>(HubConstants.ReceiveChatNotification, (message, receiverUserId, senderUserId) =>
        //{
        //    if (CurrentUserId == receiverUserId)
        //    {
        //        jsRuntime.InvokeAsync<string>("PlayAudio", new[] { "notification" });
        //        snackbar.Add(message, Severity.Info, config =>
        //        {
        //            config.VisibleStateDuration = 10000;
        //            config.HideTransitionDuration = 500;
        //            config.ShowTransitionDuration = 500;
        //            config.Action = localizer["Chat?"];
        //            config.ActionColor = Color.Primary;
        //            config.Onclick = snackbar =>
        //            {
        //                navManager.NavigateTo($"chat/{senderUserId}");
        //                return Task.CompletedTask;
        //            };
        //        });
        //    }
        //});
        //hubConnection.On(HubConstants.ReceiveRegenerateTokens, async () => NOR NEEDED, APICLIENT+HANDLER DOES REFRESH
        //{
        //    try
        //    {
        //        var token = await authManager.TryForceRefreshTokenAsync();
        //        if (!string.IsNullOrEmpty(token))
        //        {
        //            snackbar.Add(localizer["Refreshed Token."], Severity.Success);
        //            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.Message);
        //        snackbar.Add(localizer["You are Logged Out."], Severity.Error);
        //        await authManager.LogoutAsync();
        //        navManager.NavigateTo("/");
        //    }
        //});
        //this.hubConnection.On<string>(SignalRHubConstants.ReceiveInformationMessage, async =>
        //{
        //    this.logger.LogInformation("signalr: receive {signlrMessage} no message", SignalRHubConstants.ConnectUser);
        //});
        this.hubConnection.On<string>(SignalRHubConstants.ReceiveInformationMessage, (message) =>
        {
            var jsResult = this.jsRuntime.InvokeAsync<string>("PlayAudio", new[] { "notification" });
            this.logger.LogInformation("signalr: receive {signlrMessage}", SignalRHubConstants.ReceiveInformationMessage);
            this.snackbar.Add(message, Severity.Normal, config =>
                {
                    config.VisibleStateDuration = 10000;
                    config.HideTransitionDuration = 500;
                    config.ShowTransitionDuration = 500;
                    config.ActionColor = Color.Primary;
                    //config.Onclick = snackbar =>
                    //{
                    //    this.navManager.NavigateTo($"catalog/products");
                    //    return Task.CompletedTask;
                    //};
                });
        });
        this.hubConnection.On<string>(SignalRHubConstants.ConnectUser, async (userId) =>
        {
            this.logger.LogInformation("signalr: receive {signlrMessage} (userId={userId})", SignalRHubConstants.ConnectUser, userId);
            await Task.Delay(0);
        });
        this.hubConnection.On<string>(SignalRHubConstants.DisconnectUser, async (userId) =>
        {
            this.logger.LogInformation("signalr: receive {signlrMessage} (userId={userId})", SignalRHubConstants.DisconnectUser, userId);
            await Task.Delay(0);
        });
        this.hubConnection.On<string>(SignalRHubConstants.LogoutDeactivatedUser, async (userId) =>
        {
            if (this.CurrentUserId == userId)
            {
                this.logger.LogInformation("signalr: receive {signlrMessage} (userId={userId})", SignalRHubConstants.LogoutDeactivatedUser, userId);
                if (this.CurrentUserId == userId)
                {
                    this.snackbar.Add(this.localizer["You are logged out because your account has been deactivated."], Severity.Error);
                    if (this.hubConnection.State == HubConnectionState.Connected)
                    {
                        await this.hubConnection.SendAsync(SignalRHubConstants.OnDisconnect, this.CurrentUserId);
                    }

                    await this.authManager.LogoutAsync();
                    this.navManager.NavigateTo($"{IdentityPageConstants.Login}?redirectUrl={Uri.EscapeDataString(this.navManager.Uri)}");
                }
            }
        });
        this.hubConnection.On<string, string>(SignalRHubConstants.LogoutUsersByRole, async (userId, roleId) =>
        {
            await Task.Delay(1);
            this.snackbar.Add($"On {SignalRHubConstants.LogoutUsersByRole} not available now", Severity.Warning);
            //if (this.CurrentUserId != userId) // TODO: move to Identity where the Identity APIClient is available
            //{
            //    this.logger.LogInformation("signalr: receive {signlrMessage} (userId={userId})", SignalRHubConstants.LogoutUsersByRole, userId);
            //    // following permissions are needed for this to work (roles): Permissions.Users.View + Permissions.Roles.View
            //    var response = await this.apiClient.Role_GetAllAsync();
            //    if (response.Result.IsSuccess)
            //    {
            //        var role = response.Result.Value.FirstOrDefault(x => x.Id == roleId);
            //        if (role != null)
            //        {
            //            var currentUserRolesResponse = await this.apiClient.User_GetRolesAsync(this.CurrentUserId);
            //            if (currentUserRolesResponse.Result.IsSuccess && currentUserRolesResponse.Result.Value.UserRoles.Any(x => x.RoleName == role.Name))
            //            {
            //                this.snackbar.Add(this.localizer["You are logged out because the Permissions of one of your Roles have been updated."], Severity.Error);
            //                if (this.hubConnection.State == HubConnectionState.Connected)
            //                {
            //                    await this.hubConnection.SendAsync(SignalRHubConstants.OnDisconnect, this.CurrentUserId);
            //                }

            //                await this.authManager.LogoutAsync();
            //                this.navManager.NavigateTo($"{IdentityPageConstants.Login}?redirectUrl={Uri.EscapeDataString(this.navManager.Uri)}");
            //            }
            //        }
            //    }
            //}
        });
        this.hubConnection.On<string>(SignalRHubConstants.PingRequest, async (userName) =>
        {
            if (this.IsConnected)
            {
                await this.hubConnection.SendAsync(SignalRHubConstants.PingResponse, this.CurrentUserId, userName);
            }
        });

        await this.hubConnection.Start();
        this.IsConnected = this.hubConnection.State == HubConnectionState.Connected;

        if (this.IsConnected)
        {
            await this.hubConnection.SendAsync(SignalRHubConstants.OnConnect, this.CurrentUserId);
            this.snackbar.Add("signalr connected", Severity.Normal);
        }
        else
        {
            this.snackbar.Add("signalr not connected", Severity.Error);
        }
    }

    protected override void OnParametersSet()
    {
        this.errorBoundary?.Recover();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await this.LoadDataAsync();
        }
    }

    private async Task ToggleDarkMode()
    {
        await this.OnDarkModeToggle.InvokeAsync();
        this.isDarkMode = await this.clientPreferenceManager.IsDarkMode();
    }

    private async Task LoadDataAsync()
    {
        var user = (await this.AuthState).User;
        if (user.Identity?.IsAuthenticated == true)
        {
            if (string.IsNullOrEmpty(this.CurrentUserId))
            {
                this.CurrentUserId = user.FindFirstValue(ClaimTypes.NameIdentifier);
                this.FirstName = user.FindFirstValue(ClaimTypes.Name);
                if (this.FirstName?.Length > 0)
                {
                    this.FirstLetterOfName = this.FirstName[0];
                }

                this.SecondName = user.FindFirstValue(ClaimTypes.Surname);
                this.Email = user.FindFirstValue(ClaimTypes.Email);
                //var profileResponse = await this.apiClient.Account_GetProfilePictureAsync(this.CurrentUserId);
                //if (profileResponse.Result.Succeeded)
                //{
                //    this.ImageDataUrl = profileResponse.Result.Data;
                //}

                //var userResponse = await this.apiClient.User_GetByIdAsync(this.CurrentUserId);
                //if (!userResponse.Result.Succeeded || userResponse.Result.Data == null)
                //{
                //    this.snackbar.Add(
                //        this.localizer["You are logged out because the user with your token has been deleted."],
                //        Severity.Error);
                //    this.CurrentUserId = string.Empty;
                //    this.ImageDataUrl = string.Empty;
                //    this.FirstName = string.Empty;
                //    this.SecondName = string.Empty;
                //    this.Email = string.Empty;
                //    this.FirstLetterOfName = char.MinValue;
                //    await this.authManager.LogoutAsync();
                //}
            }
        }
    }

    private void DrawerToggle()
    {
        this.drawerOpen = !this.drawerOpen;
    }

    private void Logout()
    {
        var parameters = new DialogParameters
        {
            {nameof(global::Modules.Identity.Presentation.Web.Client.Components.Logout.ContentText), $"{this.localizer["Are you sure?"]}"},
            {nameof(global::Modules.Identity.Presentation.Web.Client.Components.Logout.ButtonText), $"{this.localizer["Logout"]}"},
            {nameof(global::Modules.Identity.Presentation.Web.Client.Components.Logout.Color), Color.Error},
            {nameof(global::Modules.Identity.Presentation.Web.Client.Components.Logout.CurrentUserId), this.CurrentUserId},
            {nameof(global::Modules.Identity.Presentation.Web.Client.Components.Logout.HubConnection), this.hubConnection}
        };

        var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true };
        this.dialogService.Show<global::Modules.Identity.Presentation.Web.Client.Components.Logout>(this.localizer["Logout"], parameters, options);
    }
}
