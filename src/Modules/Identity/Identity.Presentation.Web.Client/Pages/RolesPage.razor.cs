namespace Modules.Identity.Presentation.Web.Client.Pages;

using System.Security.Claims;
using BridgingIT.DevKit.Common;
using Common.Presentation.Web.Client.Components;
using Microsoft.AspNetCore.Authorization;
using MudBlazor;
using Modules.Identity.Presentation.Web.Client.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

public partial class RolesPage
{
    private List<RoleResponseModel> roleList = new();
    private RoleResponseModel model = new();
    private string searchString = string.Empty;

    private ClaimsPrincipal user;
    private bool canCreateRoles;
    private bool canEditRoles;
    private bool canDeleteRoles;
    private bool canSearchRoles;
    private bool canViewRoleClaims;
    private bool loaded;

    [CascadingParameter]
    protected Task<AuthenticationState> AuthState { get; set; } = default!;

    [Inject]
    protected IAuthorizationService AuthService { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        this.user = (await this.AuthState).User;
        this.canCreateRoles = (await this.AuthService.AuthorizeAsync(this.user, IdentityPermissions.Roles.Create)).Succeeded;
        this.canEditRoles = (await this.AuthService.AuthorizeAsync(this.user, IdentityPermissions.Roles.Edit)).Succeeded;
        this.canDeleteRoles = (await this.AuthService.AuthorizeAsync(this.user, IdentityPermissions.Roles.Delete)).Succeeded;
        this.canSearchRoles = (await this.AuthService.AuthorizeAsync(this.user, IdentityPermissions.Roles.Search)).Succeeded;
        this.canViewRoleClaims = (await this.AuthService.AuthorizeAsync(this.user, IdentityPermissions.RoleClaims.View)).Succeeded;

        await this.GetRolesAsync();
        this.loaded = true;
    }

    private async Task GetRolesAsync()
    {
        var response = await this.apiClient.Identity_RoleGetAllAsync();
        if (response.IsSuccess)
        {
            this.roleList = response.Value.ToList();
        }
        else
        {
            foreach (var message in response.Messages.SafeNull())
            {
                this.snackbar.Add(message, Severity.Error);
            }
        }
    }

    private async Task Delete(string id)
    {
        var dialog = this.dialogService.Show<DeleteConfirmation>(
            this.localizer["Delete"],
            new DialogParameters
            {
                { nameof(DeleteConfirmation.ContentText), string.Format(this.localizer["Delete Content"], id) }
            },
            new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true });
        var result = await dialog.Result;

        if (!result.Canceled)
        {
            try
            {
                var response = await this.apiClient.Identity_RoleDeleteAsync(id);
                if (response.IsSuccess)
                {
                    await this.Reset();
                    this.snackbar.Add(response.Messages.FirstOrDefault(), Severity.Success);
                }
                else
                {
                    await this.Reset();
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
    }

    private async Task InvokeModal(string id = null)
    {
        var parameters = new DialogParameters();
        if (id != null)
        {
            this.model = this.roleList?.FirstOrDefault(c => c.Id == id);
            if (this.model != null)
            {
                parameters.Add(nameof(RoleModal.Model), new RoleRequestModel
                {
                    Id = this.model.Id,
                    Name = this.model.Name,
                    Description = this.model.Description
                });
            }
        }

        var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true };
        var dialog = this.dialogService.Show<RoleModal>(id == null ? this.localizer["Create"] : this.localizer["Edit"], parameters, options);
        var result = await dialog.Result;
        if (!result.Canceled)
        {
            await this.Reset();
        }
    }

    private async Task Reset()
    {
        this.model = new RoleResponseModel();
        await this.GetRolesAsync();
    }

    private bool Search(RoleResponseModel role)
    {
        if (string.IsNullOrWhiteSpace(this.searchString))
        {
            return true;
        }

        if (role.Name?.Contains(this.searchString, StringComparison.OrdinalIgnoreCase) == true)
        {
            return true;
        }

        if (role.Description?.Contains(this.searchString, StringComparison.OrdinalIgnoreCase) == true)
        {
            return true;
        }

        return false;
    }

    private void ManagePermissions(string roleId) => this.navManager.NavigateTo($"/identity/role-permissions/{roleId}");
}
