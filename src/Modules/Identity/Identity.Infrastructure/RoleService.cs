namespace Modules.Identity.Infrastructure;

using BridgingIT.DevKit.Common;
using Common;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Modules.Identity.Application;

public class RoleService : IRoleService // TODO: add logging statements
{
    private readonly ILogger<RoleService> logger;
    private readonly RoleManager<AppIdentityRole> roleManager;
    private readonly UserManager<AppIdentityUser> userManager;
    private readonly IStringLocalizer<RoleService> localizer;
    private readonly IRoleClaimService roleClaimService;
    private readonly ICurrentUserService currentUserService;
    private readonly IEnumerable<IModuleContextAccessor> moduleAccessors;
    private readonly IEnumerable<IPermissionSet> permissionSets;
    private readonly ICacheProvider cacheProvider;

    public RoleService(
        ILogger<RoleService> logger,
        RoleManager<AppIdentityRole> roleManager,
        UserManager<AppIdentityUser> userManager,
        IStringLocalizer<RoleService> localizer,
        IRoleClaimService roleClaimService,
        ICurrentUserService currentUserService,
        IEnumerable<IModuleContextAccessor> moduleAccessors = null,
        IEnumerable<IPermissionSet> permissionSets = null,
        ICacheProvider cacheProvider = null)
    {
        this.logger = logger;
        this.roleManager = roleManager;
        this.userManager = userManager;
        this.localizer = localizer;
        this.roleClaimService = roleClaimService;
        this.currentUserService = currentUserService;
        this.moduleAccessors = moduleAccessors;
        this.permissionSets = permissionSets;
        this.cacheProvider = cacheProvider;
    }

    public async Task<Result> DeleteAsync(string id)
    {
        var existingRole = await this.roleManager.FindByIdAsync(id);
        if (existingRole.Name != RoleConstants.AdministratorRole && existingRole.Name != RoleConstants.BasicRole)
        {
            var roleIsNotUsed = true;
            var allUsers = await this.userManager.Users.ToListAsync();
            foreach (var user in allUsers)
            {
                if (await this.userManager.IsInRoleAsync(user, existingRole.Name))
                {
                    roleIsNotUsed = false;
                }
            }

            if (roleIsNotUsed)
            {
                await this.roleManager.DeleteAsync(existingRole);
                await this.InvalidatePermissionsCacheAsync();

                return Result.Success(string.Format("Role {0} Deleted.", existingRole.Name));
            }
            else
            {
                return Result.Failure(string.Format("Not allowed to delete {0} Role as it is being used.", existingRole.Name));
            }
        }
        else
        {
            return Result.Failure(string.Format("Not allowed to delete {0} Role.", existingRole.Name));
        }
    }

    public async Task<Result<IEnumerable<RoleModel>>> GetAllAsync()
    {
        var roles = await this.roleManager.Roles.ToListAsync();

        return Result<IEnumerable<RoleModel>>.Success(
            roles.ConvertAll(r => new RoleModel { Id = r.Id.ToString(), Name = r.Name, Description = r.Description }));
    }

    public async Task<Result<PermissionModel>> GetAllPermissionsAsync(string roleId)
    {
        var response = new PermissionModel();
        var allPermissions = this.GetAllPermissions();
        var role = await this.roleManager.FindByIdAsync(roleId);

        if (role != null)
        {
            response.RoleId = role.Id.ToString();
            response.RoleName = role.Name;
            var roleClaimsResult = await this.roleClaimService.GetAllByRoleIdAsync(role.Id.ToString());

            if (roleClaimsResult.IsSuccess)
            {
                var roleClaims = roleClaimsResult.Value;
                var allClaimValues = allPermissions.ConvertAll(a => a.Value);
                var roleClaimValues = roleClaims.Select(a => a.Value).ToList();
                var authorizedClaims = allClaimValues.Intersect(roleClaimValues).ToList();

                foreach (var permission in allPermissions)
                {
                    if (authorizedClaims.Any(a => a == permission.Value))
                    {
                        permission.Selected = true;
                        var roleClaim = roleClaims.SingleOrDefault(a => a.Value == permission.Value);

                        if (roleClaim?.Description != null)
                        {
                            permission.Description = roleClaim.Description;
                        }

                        if (roleClaim?.Group != null)
                        {
                            permission.Group = roleClaim.Group;
                        }
                    }
                }
            }
            else
            {
                response.RoleClaims = new List<RoleClaimModel>();

                return Result<PermissionModel>.Failure().WithMessages(roleClaimsResult.Messages);
            }
        }

        response.RoleClaims = allPermissions;
        return Result<PermissionModel>.Success(response);
    }

    public async Task<Result<RoleModel>> GetByIdAsync(string id)
    {
        var roleId = Guid.Parse(id);
        var roles = await this.roleManager.Roles.SingleOrDefaultAsync(x => x.Id == roleId);

        return Result<RoleModel>.Success(
            new RoleModel { Id = roles.Id.ToString(), Name = roles.Name, Description = roles.Description });
    }

    public async Task<Result> SaveAsync(RoleModel role)
    {
        if (string.IsNullOrEmpty(role.Id))
        {
            var existingRole = await this.roleManager.FindByNameAsync(role.Name);
            if (existingRole != null)
            {
                return Result.Failure("Similar Role already exists.");
            }

            var response = await this.roleManager.CreateAsync(new AppIdentityRole(role.Name, role.Description));
            if (response.Succeeded)
            {
                await this.InvalidatePermissionsCacheAsync();

                return Result.Success(string.Format("Role {0} Created.", role.Name));
            }
            else
            {
                return Result.Failure(response.Errors.Select(e => e.Description).ToList());
            }
        }
        else
        {
            var existingRole = await this.roleManager.FindByIdAsync(role.Id);
            if (existingRole.Name == RoleConstants.AdministratorRole || existingRole.Name == RoleConstants.BasicRole)
            {
                return Result.Failure(string.Format("Not allowed to modify {0} Role.", existingRole.Name));
            }

            existingRole.Name = role.Name;
            existingRole.NormalizedName = role.Name.ToUpper();
            existingRole.Description = role.Description;
            await this.roleManager.UpdateAsync(existingRole);
            await this.InvalidatePermissionsCacheAsync();

            return Result.Success(string.Format("Role {0} Updated.", existingRole.Name));
        }
    }

    public async Task<Result> UpdatePermissionsAsync(PermissionModel permission)
    {
        try
        {
            var errors = new List<string>();
            var role = await this.roleManager.FindByIdAsync(permission.RoleId);
            var userId = Guid.Parse(this.currentUserService.UserId);
            if (role.Name == RoleConstants.AdministratorRole)
            {
                var currentUser = await this.userManager.Users.SingleAsync(x => x.Id == userId);
                if (await this.userManager.IsInRoleAsync(currentUser, RoleConstants.AdministratorRole))
                {
                    return Result.Failure("Not allowed to modify Permissions for this Role.");
                }
            }

            var selectedClaims = permission.RoleClaims.Where(a => a.Selected).ToList();
            if (role.Name == RoleConstants.AdministratorRole)
            {
                if (!selectedClaims.Any(x => x.Value == IdentityPermissionSet.Roles.View)
                   || !selectedClaims.Any(x => x.Value == IdentityPermissionSet.RoleClaims.View)
                   || !selectedClaims.Any(x => x.Value == IdentityPermissionSet.RoleClaims.Edit))
                {
                    return Result.Failure(string.Format(
                        "Not allowed to deselect {0} or {1} or {2} for this Role.",
                        IdentityPermissionSet.Roles.View, IdentityPermissionSet.RoleClaims.View, IdentityPermissionSet.RoleClaims.Edit));
                }
            }

            var claims = await this.roleManager.GetClaimsAsync(role);
            foreach (var claim in claims)
            {
                await this.roleManager.RemoveClaimAsync(role, claim);
            }

            foreach (var claim in selectedClaims)
            {
                var addResult = await this.roleManager.AddPermissionClaim(role, claim.Value);
                if (!addResult.Succeeded)
                {
                    errors.AddRange(addResult.Errors.Select(e => e.Description));
                }
            }

            var addedClaims = await this.roleClaimService.GetAllByRoleIdAsync(role.Id.ToString());
            if (addedClaims.IsSuccess)
            {
                foreach (var claim in selectedClaims)
                {
                    var addedClaim = addedClaims.Value.SingleOrDefault(x => x.Type == claim.Type && x.Value == claim.Value);
                    if (addedClaim != null)
                    {
                        claim.Id = addedClaim.Id;
                        claim.RoleId = addedClaim.RoleId;
                        var saveResult = await this.roleClaimService.SaveAsync(claim);
                        if (saveResult.IsFailure)
                        {
                            errors.AddRange(saveResult.Messages);
                        }
                    }
                }
            }
            else
            {
                errors.AddRange(addedClaims.Messages);
            }

            if (errors.Any())
            {
                return Result.Failure(errors);
            }

            await this.InvalidatePermissionsCacheAsync();

            return Result.Success("Permissions Updated.");
        }
        catch (Exception ex)
        {
            return Result.Failure(ex.Message);
        }
    }

    public async Task<int> GetCountAsync()
    {
        return await this.roleManager.Roles.CountAsync();
    }

    private List<RoleClaimModel> GetAllPermissions()
    {
        var result = new List<RoleClaimModel>();

        foreach (var permissionSet in this.permissionSets.SafeNull())
        {
            result.GetAllPermissions(permissionSet, this.moduleAccessors);
        }

        return result;
    }

    private async Task InvalidatePermissionsCacheAsync()
    {
        await this.cacheProvider?.RemoveStartsWithAsync("identity-permissions-");
    }
}