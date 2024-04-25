namespace Modules.Identity.Presentation.Web.Client;

using Common;

public class IdentityPermissions : IPermissions
{
    public static class Users
    {
        public const string View = "Identity.Users.View";
        public const string Create = "Identity.Users.Create";
        public const string Edit = "Identity.Users.Edit";
        public const string Delete = "Identity.Users.Delete";
        public const string Export = "Identity.Users.Export";
        public const string Search = "Identity.Users.Search";
    }

    public static class Roles
    {
        public const string View = "Identity.Roles.View";
        public const string Create = "Identity.Roles.Create";
        public const string Edit = "Identity.Roles.Edit";
        public const string Delete = "Identity.Roles.Delete";
        public const string Search = "Identity.Roles.Search";
    }

    public static class RoleClaims
    {
        public const string View = "Identity.RoleClaims.View";
        public const string Create = "Identity.RoleClaims.Create";
        public const string Edit = "Identity.RoleClaims.Edit";
        public const string Delete = "Identity.RoleClaims.Delete";
        public const string Search = "Identity.RoleClaims.Search";
    }
}