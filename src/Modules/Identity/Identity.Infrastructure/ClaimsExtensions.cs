namespace Modules.Identity.Infrastructure;

using System.Reflection;
using System.Security.Claims;
using BridgingIT.DevKit.Common;
using Common;
using Microsoft.AspNetCore.Identity;
using Modules.Identity.Application;

public static class ClaimsExtensions
{
    public static void GetAllPermissions(
        this List<RoleClaimModel> allPermissions,
        IPermissionSet permissionSet,
        IEnumerable<IModuleContextAccessor> moduleAccessors = null)
    {
        if (permissionSet == null)
        {
            return;
        }

        foreach (var group in permissionSet.GetType().GetNestedTypes())
        {
            var groupName = string.Empty;
            var groupDescription = string.Empty;

            if (group.GetCustomAttributes(typeof(DisplayNameAttribute), true)
                .FirstOrDefault() is DisplayNameAttribute displayNameAttribute)
            {
                groupName = displayNameAttribute.DisplayName;
            }

            if (group.GetCustomAttributes(typeof(DescriptionAttribute), true)
                .FirstOrDefault() is DescriptionAttribute descriptionAttribute)
            {
                groupDescription = descriptionAttribute.Description;
            }

            var moduleName = moduleAccessors.Find(group)?.Name;

            foreach (var permissionField in group.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy))
            {
                var permission = permissionField.GetValue(null);
                if (permission is not null)
                {
                    allPermissions.Add(new RoleClaimModel
                    {
                        Value = permission.ToString(),
                        Type = "Permission",
                        Module = moduleName,
                        Group = groupName,
                        Description = groupDescription
                    });
                }
            }
        }
    }

    public static async Task<IdentityResult> AddPermissionClaim(this RoleManager<AppIdentityRole> roleManager, AppIdentityRole role, string permission)
    {
        var claims = await roleManager.GetClaimsAsync(role);
        if (!claims.Any(a => a.Type == "Permission" && a.Value == permission))
        {
            return await roleManager.AddClaimAsync(role, new Claim("Permission", permission));
        }

        return IdentityResult.Failed();
    }
}
