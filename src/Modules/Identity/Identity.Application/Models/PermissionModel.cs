namespace Modules.Identity.Application;

public class PermissionModel
{
    public string RoleId { get; set; }

    public string RoleName { get; set; }

    public List<RoleClaimModel> RoleClaims { get; set; }
}
