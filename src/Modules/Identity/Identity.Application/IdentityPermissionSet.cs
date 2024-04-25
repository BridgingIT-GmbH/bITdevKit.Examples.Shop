namespace Modules.Identity.Application;

using Common;

public class IdentityPermissionSet : PermissionSetBase
{
    [DisplayName("Users")]
    [Description("Users Permissions")]
    public static class Users
    {
        public const string View = "Identity.Users.View";
        public const string Create = "Identity.Users.Create";
        public const string Edit = "Identity.Users.Edit";
        public const string Delete = "Identity.Users.Delete";
        public const string Export = "Identity.Users.Export";
        public const string Search = "Identity.Users.Search";
    }

    [DisplayName("Roles")]
    [Description("Roles Permissions")]
    public static class Roles
    {
        public const string View = "Identity.Roles.View";
        public const string Create = "Identity.Roles.Create";
        public const string Edit = "Identity.Roles.Edit";
        public const string Delete = "Identity.Roles.Delete";
        public const string Search = "Identity.Roles.Search";
    }

    [DisplayName("Role Claims")]
    [Description("Role Claims Permissions")]
    public static class RoleClaims
    {
        public const string View = "Identity.RoleClaims.View";
        public const string Create = "Identity.RoleClaims.Create";
        public const string Edit = "Identity.RoleClaims.Edit";
        public const string Delete = "Identity.RoleClaims.Delete";
        public const string Search = "Identity.RoleClaims.Search";
    }
}
