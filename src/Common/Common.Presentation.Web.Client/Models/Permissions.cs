namespace Common.Presentation.Web.Client.Models;

using Common;

public class Permissions : IPermissions
{
    public static class Preferences
    {
        public const string ChangeLanguage = "Permissions.Preferences.ChangeLanguage";
        //TODO: add permissions
    }

    public static class Dashboards
    {
        public const string View = "Permissions.Dashboards.View";
    }
}