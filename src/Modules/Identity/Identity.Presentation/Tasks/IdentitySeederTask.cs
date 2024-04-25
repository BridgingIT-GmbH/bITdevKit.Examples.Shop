namespace Modules.Identity.Presentation;

using System;
using System.Threading.Tasks;
using BridgingIT.DevKit.Common;
using Common;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Modules.Identity.Application;
using Modules.Identity.Infrastructure;
using Modules.Identity.Infrastructure.EntityFramework;

public class IdentitySeederTask : IStartupTask
{
    private readonly ILogger<IdentitySeederTask> logger;
    private readonly UserManager<AppIdentityUser> userManager;
    private readonly RoleManager<AppIdentityRole> roleManager;
    private readonly IdentityDbContext context;
    private readonly IEnumerable<IPermissionSet> permissionSets;

    public IdentitySeederTask(
        ILoggerFactory loggerFactory,
        UserManager<AppIdentityUser> userManager,
        RoleManager<AppIdentityRole> roleManager,
        IdentityDbContext context,
        IEnumerable<IPermissionSet> permissionSets = null)
    {
        this.logger = loggerFactory?.CreateLogger<IdentitySeederTask>() ?? NullLoggerFactory.Instance.CreateLogger<IdentitySeederTask>();
        this.userManager = userManager;
        this.roleManager = roleManager;
        this.context = context;
        this.permissionSets = permissionSets;
    }

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        // add administrator ****************************
        //Check if Role Exists
        var adminRole = new AppIdentityRole(RoleConstants.AdministratorRole, "Administrator role with full permissions");
        var adminRoleInDb = await this.roleManager.FindByNameAsync(RoleConstants.AdministratorRole);
        if (adminRoleInDb == null)
        {
            await this.roleManager.CreateAsync(adminRole);
            adminRoleInDb = await this.roleManager.FindByNameAsync(RoleConstants.AdministratorRole);
            this.logger.LogDebug("Seeded Administrator Role.");
        }

        //Check if User Exists
        var superUser = new AppIdentityUser
        {
            FirstName = "Administrator",
            LastName = "Administrator",
            Email = "admin@acmeshop.com",
            UserName = "admin",
            EmailConfirmed = true,
            PhoneNumberConfirmed = true,
            CreatedOn = DateTime.UtcNow,
            IsActive = true
        };
        var superUserInDb = await this.userManager.FindByEmailAsync(superUser.Email);
        if (superUserInDb == null)
        {
            await this.userManager.CreateAsync(superUser, "Fidespic032");
            var result = await this.userManager.AddToRoleAsync(superUser, RoleConstants.AdministratorRole);
            if (result.Succeeded)
            {
                this.logger.LogDebug("Seeded Default SuperAdmin User.");
            }
            else
            {
                foreach (var error in result.Errors.SafeNull())
                {
                    this.logger.LogError(error.Description);
                }
            }
        }

        // add all permissions to the admin role
        foreach (var permissionSet in this.permissionSets.SafeNull())
        {
            this.logger.LogDebug("identity: set role permissions (role={roleName}, set={permissionSet})", adminRoleInDb.Name, permissionSet.GetType().FullName);
            foreach (var permission in permissionSet.GetPermissions())
            {
                await this.roleManager.AddPermissionClaim(adminRoleInDb, permission);
            }
        }

        // add basic user *********************
        //Check if Role Exists
        var basicRole = new AppIdentityRole(RoleConstants.BasicRole, "Basic role with default permissions");
        var basicRoleInDb = await this.roleManager.FindByNameAsync(RoleConstants.BasicRole);
        if (basicRoleInDb == null)
        {
            await this.roleManager.CreateAsync(basicRole);
            this.logger.LogDebug("Seeded Basic Role.");
        }

        //Check if User Exists
        var basicUser = new AppIdentityUser
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@acmeshop.com",
            UserName = "johndoe",
            EmailConfirmed = true,
            PhoneNumberConfirmed = true,
            CreatedOn = DateTime.UtcNow,
            IsActive = true
        };
        var basicUserInDb = await this.userManager.FindByEmailAsync(basicUser.Email);
        if (basicUserInDb == null)
        {
            await this.userManager.CreateAsync(basicUser, "Fidespic032");
            await this.userManager.AddToRoleAsync(basicUser, RoleConstants.BasicRole);
            this.logger.LogDebug("Seeded User with Basic Role.");
        }

        this.context.SaveChanges(); // TODO: really needed to save the dbcontext? everything is done by using the identity managers
    }
}