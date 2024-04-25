namespace Modules.Identity.Presentation.Web.Client.Pages;

using System.Security.Claims;
using BridgingIT.DevKit.Common;
using Common.SignalR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.SignalR.Client;
using Modules.Identity.Presentation.Web.Client;
using MudBlazor;

public partial class RolePermissionsPage
{
    private ClaimsPrincipal user;
    private PermissionResponseModel model;
    private RoleClaimResponseModel roleClaims = new();
    private RoleClaimResponseModel selectedItem = new();
    private string searchString = string.Empty;
    private bool canEditRolePermissions;
    private bool canSearchRolePermissions;
    private bool loaded;

    [CascadingParameter]
    public HubConnection HubConnection { get; set; }

    [CascadingParameter]
    protected Task<AuthenticationState> AuthState { get; set; } = default!;

    [Inject]
    protected IAuthorizationService AuthService { get; set; } = default!;

    [Inject]
    private IAccessTokenProvider TokenProvider { get; set; } = default!;

    [Parameter]
    public string Id { get; set; }

    [Parameter]
    public string Title { get; set; }

    [Parameter]
    public string Description { get; set; }

    private Dictionary<string, List<RoleClaimResponseModel>> Models { get; } = new();

    protected override async Task OnInitializedAsync()
    {
        this.user = (await this.AuthState).User;
        this.canEditRolePermissions = (await this.AuthService.AuthorizeAsync(this.user, IdentityPermissions.RoleClaims.Edit)).Succeeded;
        this.canSearchRolePermissions = (await this.AuthService.AuthorizeAsync(this.user, IdentityPermissions.RoleClaims.Search)).Succeeded;

        this.HubConnection = this.HubConnection.TryInitialize(this.navManager, this.TokenProvider, this.loggerProvider); // TODO: really needed?, as it is already injected as a parameter
        await this.HubConnection.Start();

        await this.LoadData();
        this.loaded = true;
    }

    private async Task LoadData()
    {
        var roleId = this.Id;
        var response = await this.apiClient.Identity_RoleGetPermissionsByRoleIdAsync(roleId);
        if (response.IsSuccess)
        {
            this.model = response.Value;
            this.Models.Add(this.localizer["All Permissions"], this.model.RoleClaims?.ToList());

            foreach (var claim in this.model.RoleClaims.SafeNull())
            {
                if (this.Models.TryGetValue(claim.Group, out var value))
                {
                    value.Add(claim);
                }
                else
                {
                    this.Models.Add(claim.Group, new List<RoleClaimResponseModel> { claim });
                }
            }

            if (this.model != null)
            {
                this.Description = string.Format(this.localizer["Manage {0} Permissions"], this.model.RoleName);
            }
        }
        else
        {
            foreach (var message in response.Messages.SafeNull())
            {
                this.snackbar.Add(message, Severity.Error);
            }

            this.navManager.NavigateTo("/identity/roles");
        }
    }

    private async Task SaveAsync()
    {
        try
        {
            var requestModel = this.Map(this.model);
            var response = await this.apiClient.Identity_RoleUpdateAsync(requestModel);
            if (response.IsSuccess)
            {
                this.snackbar.Add(response.Messages.FirstOrDefault(), Severity.Success);
                if (this.HubConnection.State == HubConnectionState.Connected)
                {
                    await this.HubConnection.SendAsync(SignalRHubConstants.OnChangeRolePermissions, this.user.FindFirst(ClaimTypes.NameIdentifier)?.Value, requestModel.RoleId);
                }

                this.navManager.NavigateTo("/identity/roles");
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

    private bool Search(RoleClaimResponseModel roleClaims)
    {
        if (string.IsNullOrWhiteSpace(this.searchString))
        {
            return true;
        }

        if (roleClaims.Type?.Contains(this.searchString, StringComparison.OrdinalIgnoreCase) == true)
        {
            return true;
        }

        if (roleClaims.Module?.Contains(this.searchString, StringComparison.OrdinalIgnoreCase) == true)
        {
            return true;
        }

        if (roleClaims.Value?.Contains(this.searchString, StringComparison.OrdinalIgnoreCase) == true)
        {
            return true;
        }

        if (roleClaims.Description?.Contains(this.searchString, StringComparison.OrdinalIgnoreCase) == true)
        {
            return true;
        }

        return false;
    }

    private Color GetGroupBadgeColor(int selected, int all)
    {
        if (selected == 0)
        {
            return Color.Error;
        }

        if (selected == all)
        {
            return Color.Success;
        }

        return Color.Info;
    }

    private PermissionRequestModel Map(PermissionResponseModel source) =>
        new()
        {
            RoleId = source.RoleId,
            RoleClaims = source.RoleClaims.Select(r =>
            new RoleClaimRequestModel
            {
                RoleId = source.RoleId,
                Description = r.Description,
                Group = r.Group,
                Id = r.Id,
                Selected = r.Selected,
                Type = r.Type,
                Value = r.Value,
            }).ToList()
        };
}