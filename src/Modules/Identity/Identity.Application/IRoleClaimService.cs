namespace Modules.Identity.Application;

using BridgingIT.DevKit.Common;

public interface IRoleClaimService
{
    Task<Result<IList<RoleClaimModel>>> GetAllAsync();

    Task<int> GetCountAsync();

    Task<Result<RoleClaimModel>> GetByIdAsync(int id);

    Task<Result<IList<RoleClaimModel>>> GetAllByRoleIdAsync(string roleId);

    Task<Result> SaveAsync(RoleClaimModel request);

    Task<Result> DeleteAsync(int id);
}