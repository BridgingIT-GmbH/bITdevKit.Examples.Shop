namespace Modules.Identity.Presentation.Web.Client.Pages;

using System.Security.Claims;
using BridgingIT.DevKit.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;

public partial class UserRolesPage
{
    private UserRoleModel model = new();
    private string searchString = string.Empty;
    private ClaimsPrincipal user;
    private bool canEditUsers;
    private bool canSearchRoles;
    private bool loaded;

    [CascadingParameter]
    protected Task<AuthenticationState> AuthState { get; set; } = default!;

    [Inject]
    protected IAuthorizationService AuthService { get; set; } = default!;

    [Parameter]
    public string Id { get; set; }

    [Parameter]
    public string Title { get; set; }

    [Parameter]
    public string Description { get; set; }

    public List<UserRoleModel> UserRolesList { get; set; } = new();

    protected override async Task OnInitializedAsync()
    {
        this.user = (await this.AuthState).User;
        this.canEditUsers = (await this.AuthService.AuthorizeAsync(this.user, IdentityPermissions.Users.Edit)).Succeeded;
        this.canSearchRoles = (await this.AuthService.AuthorizeAsync(this.user, IdentityPermissions.Roles.Search)).Succeeded;

        await this.LoadData();
        this.loaded = true;
    }

    private async Task LoadData()
    {
        var userId = this.Id;
        var response = await this.apiClient.Identity_UserGetByIdAsync(userId);
        if (response.IsSuccess)
        {
            var user = response.Value;
            if (user != null)
            {
                this.Title = this.localizer["User roles"];
                this.Description = string.Format(this.localizer["Manage {0} {1}'s Roles"], user.FirstName, user.LastName);
                var response2 = await this.apiClient.Identity_UserGetRolesAsync(user.Id);
                this.UserRolesList = response2.Value.UserRoles?.ToList();
            }
        }
    }

    private async Task SaveAsync()
    {
        var request = new UpdateUserRolesRequestModel
        {
            UserId = this.Id,
            UserRoles = this.UserRolesList
        };
        try
        {
            var response = await this.apiClient.Identity_UserUpdateRolesAsync(request.UserId, request);
            if (response.IsSuccess)
            {
                this.snackbar.Add(response.Messages?.FirstOrDefault(), Severity.Success);
                this.navManager.NavigateTo("/identity/users");
            }
            else
            {
                foreach (var message in response.Messages.SafeNull())
                {
                    this.snackbar.Add(message, Severity.Error);
                }
            }
        }
        catch (ApiException<ValidationProblemDetailsModel> ex)
        {
            this.snackbar.Add(ex.Result?.Detail, Severity.Error);
        }
    }

    private bool Search(UserRoleModel userRole)
    {
        if (string.IsNullOrWhiteSpace(this.searchString))
        {
            return true;
        }

        if (userRole.RoleName?.Contains(this.searchString, StringComparison.OrdinalIgnoreCase) == true)
        {
            return true;
        }

        if (userRole.RoleDescription?.Contains(this.searchString, StringComparison.OrdinalIgnoreCase) == true)
        {
            return true;
        }

        return false;
    }
}
