namespace Modules.Identity.Infrastructure;

using BridgingIT.DevKit.Common;
using Microsoft.EntityFrameworkCore;
using Modules.Identity.Application;
using Modules.Identity.Infrastructure.EntityFramework;

public class RoleClaimService : IRoleClaimService
{
    private readonly IdentityDbContext dbContext;
    private readonly ICacheProvider cacheProvider;

    public RoleClaimService(
        IdentityDbContext dbContext,
        ICacheProvider cacheProvider = null)
    {
        this.dbContext = dbContext;
        this.cacheProvider = cacheProvider;
    }

    public async Task<Result<IList<RoleClaimModel>>> GetAllAsync()
    {
        var roleClaims = await this.dbContext.RoleClaims.ToListAsync();
        return Result<IList<RoleClaimModel>>.Success(roleClaims.ConvertAll(r => this.Map(r)));
    }

    public async Task<int> GetCountAsync()
    {
        return await this.dbContext.RoleClaims.CountAsync();
    }

    public async Task<Result<RoleClaimModel>> GetByIdAsync(int id)
    {
        var roleClaim = await this.dbContext.RoleClaims.SingleOrDefaultAsync(x => x.Id == id);
        return Result<RoleClaimModel>.Success(this.Map(roleClaim));
    }

    public async Task<Result<IList<RoleClaimModel>>> GetAllByRoleIdAsync(string roleId)
    {
        var rId = Guid.Parse(roleId);
        var roleClaims = await this.dbContext.RoleClaims
            .Include(x => x.Role)
            .Where(x => x.RoleId == rId)
            .ToListAsync();
        var response = roleClaims.ConvertAll(r => this.Map(r));
        return Result<IList<RoleClaimModel>>.Success(response);
    }

    public async Task<Result> SaveAsync(RoleClaimModel request)
    {
        if (string.IsNullOrWhiteSpace(request.RoleId))
        {
            return Result<string>.Failure("Role is required.");
        }

        var rId = Guid.Parse(request.RoleId);
        if (request.Id == 0)
        {
            var existingRoleClaim =
                await this.dbContext.RoleClaims
                    .SingleOrDefaultAsync(x =>
                        x.RoleId == rId && x.ClaimType == request.Type && x.ClaimValue == request.Value);
            if (existingRoleClaim != null)
            {
                return Result.Failure("Similar Role Claim already exists.");
            }

            var roleClaim = this.Map(request);
            await this.dbContext.RoleClaims.AddAsync(roleClaim);
            await this.dbContext.SaveChangesAsync();
            return Result.Success(string.Format("Role Claim {0} created.", request.Value));
        }
        else
        {
            var existingRoleClaim =
                await this.dbContext.RoleClaims
                    .Include(x => x.Role)
                    .SingleOrDefaultAsync(x => x.Id == request.Id);
            if (existingRoleClaim == null)
            {
                return Result.Success("Role Claim does not exist.");
            }
            else
            {
                existingRoleClaim.ClaimType = request.Type;
                existingRoleClaim.ClaimValue = request.Value;
                existingRoleClaim.Group = request.Group;
                existingRoleClaim.Description = request.Description;
                existingRoleClaim.RoleId = Guid.Parse(request.RoleId);
                this.dbContext.RoleClaims.Update(existingRoleClaim);
                await this.dbContext.SaveChangesAsync();
                await this.InvalidatePermissionsCacheAsync();

                //return await Result<string>.SuccessAsync(string.Format("Role Claim {0} for Role {1} updated.", request.Value, existingRoleClaim.Role.Name));
                return Result.Success(string.Format("Role Claim {0} for Role {1} updated.", request.Value, existingRoleClaim.RoleId));
            }
        }
    }

    public async Task<Result> DeleteAsync(int id)
    {
        var existingRoleClaim = await this.dbContext.RoleClaims
            .Include(x => x.Role)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (existingRoleClaim != null)
        {
            this.dbContext.RoleClaims.Remove(existingRoleClaim);
            await this.dbContext.SaveChangesAsync();
            await this.InvalidatePermissionsCacheAsync();

            //return await Result<string>.SuccessAsync(string.Format("Role Claim {0} for {1} Role deleted.", existingRoleClaim.ClaimValue, existingRoleClaim.Role.Name));
            return Result.Success(string.Format("Role Claim {0} for {1} Role deleted.", existingRoleClaim.ClaimValue, existingRoleClaim.RoleId));
        }
        else
        {
            return Result<string>.Failure("Role Claim does not exist.");
        }
    }

    private RoleClaimModel Map(AppIdentityRoleClaim source)
    {
        return new RoleClaimModel
        {
            Id = source.Id,
            RoleId = source.RoleId.ToString(),
            Description = source.Description,
            Group = source.Group,
            Type = source.ClaimType,
            Value = source.ClaimValue
        };
    }

    private AppIdentityRoleClaim Map(RoleClaimModel source)
    {
        return new AppIdentityRoleClaim
        {
            Id = source.Id,
            RoleId = Guid.Parse(source.RoleId),
            Description = source.Description,
            Group = source.Group,
            ClaimType = source.Type,
            ClaimValue = source.Value
        };
    }

    private async Task InvalidatePermissionsCacheAsync()
    {
        await this.cacheProvider?.RemoveStartsWithAsync("identity-permissions-");
    }
}
