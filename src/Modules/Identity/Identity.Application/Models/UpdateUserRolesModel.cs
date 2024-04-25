namespace Modules.Identity.Application;

public class UpdateUserRolesModel
{
    public string UserId { get; set; }

    public IList<UserRoleModel> UserRoles { get; set; }
}
