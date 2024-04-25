namespace Modules.Identity.Presentation.Web.Client.Pages;

using System.Security.Claims;
using BridgingIT.DevKit.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.JSInterop;
using MudBlazor;
using Modules.Identity.Presentation.Web.Client.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

public partial class UsersPage
{
    private List<UserResponseModel> userList = new();
    private UserResponseModel model = new();
    private string searchString = string.Empty;

    private ClaimsPrincipal user;
    private bool canCreateUsers;
    private bool canSearchUsers;
    private bool canExportUsers;
    private bool canViewRoles;
    private bool loaded;

    [CascadingParameter]
    protected Task<AuthenticationState> AuthState { get; set; } = default!;

    [Inject]
    protected IAuthorizationService AuthService { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        this.user = (await this.AuthState).User;
        this.canCreateUsers = (await this.AuthService.AuthorizeAsync(this.user, IdentityPermissions.Users.Create)).Succeeded;
        this.canSearchUsers = (await this.AuthService.AuthorizeAsync(this.user, IdentityPermissions.Users.Search)).Succeeded;
        this.canExportUsers = (await this.AuthService.AuthorizeAsync(this.user, IdentityPermissions.Users.Export)).Succeeded;
        this.canViewRoles = (await this.AuthService.AuthorizeAsync(this.user, IdentityPermissions.Roles.View)).Succeeded;

        await this.LoadData();
        this.loaded = true;
    }

    private async Task LoadData()
    {
        var response = await this.apiClient.Identity_UserGetAllAsync();
        if (response.IsSuccess)
        {
            this.userList = response.Value.ToList();
        }
        else
        {
            foreach (var message in response.Messages.SafeNull())
            {
                this.snackbar.Add(message, Severity.Error);
            }
        }
    }

    private bool Search(UserResponseModel user)
    {
        if (string.IsNullOrWhiteSpace(this.searchString))
        {
            return true;
        }

        if (user.FirstName?.Contains(this.searchString, StringComparison.OrdinalIgnoreCase) == true)
        {
            return true;
        }

        if (user.LastName?.Contains(this.searchString, StringComparison.OrdinalIgnoreCase) == true)
        {
            return true;
        }

        if (user.Email?.Contains(this.searchString, StringComparison.OrdinalIgnoreCase) == true)
        {
            return true;
        }

        if (user.PhoneNumber?.Contains(this.searchString, StringComparison.OrdinalIgnoreCase) == true)
        {
            return true;
        }

        if (user.UserName?.Contains(this.searchString, StringComparison.OrdinalIgnoreCase) == true)
        {
            return true;
        }

        return false;
    }

    private async Task ExportToExcel()
    {
        var value = (await this.apiClient.Identity_UserExportAsync(this.searchString)).Value;
        await this.jsRuntime.InvokeVoidAsync("Download", new
        {
            ByteArray = value,
            FileName = $"users_{DateTime.Now:ddMMyyyyHHmmss}.xlsx",
            MimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
        });
        this.snackbar.Add(this.localizer["Users exported"], Severity.Success);
    }

    private async Task InvokeModal()
    {
        var parameters = new DialogParameters();
        var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true };
        var dialog = this.dialogService.Show<RegisterModal>(this.localizer["Register New User"], parameters, options);
        var result = await dialog.Result;
        if (!result.Canceled)
        {
            await this.LoadData();
        }
    }

    private void ViewProfile(string userId) => this.navManager.NavigateTo($"/user-profile/{userId}");

    private void ManageRoles(string userId) => this.navManager.NavigateTo($"/identity/user-roles/{userId}");
}
