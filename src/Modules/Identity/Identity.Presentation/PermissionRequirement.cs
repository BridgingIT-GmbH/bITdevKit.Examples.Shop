namespace Modules.Identity.Presentation;

using Microsoft.AspNetCore.Authorization;

public class PermissionRequirement : IAuthorizationRequirement
{
    public string Permission { get; private set; }

    public PermissionRequirement(string permission)
    {
        this.Permission = permission;
    }
}