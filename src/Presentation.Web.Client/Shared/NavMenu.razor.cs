namespace Presentation.Web.Client.Shared;

using System.Security.Claims;
using System.Threading.Tasks;
using Common.Presentation.Web.Client.Models;
using Modules.Catalog.Presentation.Web.Client;
using Modules.Identity.Presentation.Web.Client;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;

public partial class NavMenu
{
    private ClaimsPrincipal principal;
    private bool canViewDashboards = true;
    private bool canViewRoles = true;
    private bool canViewUsers = true;
    private bool canViewProducts = true;
    private bool canViewBrands = true;

    [CascadingParameter]
    protected Task<AuthenticationState> AuthState { get; set; } = default!;

    [Inject]
    protected IAuthorizationService AuthService { get; set; } = default!;

    protected override async Task OnParametersSetAsync()
    {
        this.principal = (await this.AuthState).User;
        this.canViewDashboards = (await this.AuthService.AuthorizeAsync(this.principal, Permissions.Dashboards.View)).Succeeded;
        this.canViewRoles = (await this.AuthService.AuthorizeAsync(this.principal, IdentityPermissions.Roles.View)).Succeeded;
        this.canViewUsers = (await this.AuthService.AuthorizeAsync(this.principal, IdentityPermissions.Users.View)).Succeeded;
        this.canViewProducts = (await this.AuthService.AuthorizeAsync(this.principal, CatalogPermissions.Products.View)).Succeeded;
        this.canViewBrands = (await this.AuthService.AuthorizeAsync(this.principal, CatalogPermissions.Brands.View)).Succeeded;
    }
}