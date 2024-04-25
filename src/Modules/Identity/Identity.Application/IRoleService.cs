namespace Modules.Identity.Application;

using BridgingIT.DevKit.Common;

public interface IRoleService
{
    Task<Result<IEnumerable<RoleModel>>> GetAllAsync();

    Task<int> GetCountAsync();

    Task<Result<RoleModel>> GetByIdAsync(string id);

    Task<Result> SaveAsync(RoleModel role);

    Task<Result> DeleteAsync(string id);

    Task<Result<PermissionModel>> GetAllPermissionsAsync(string roleId);

    Task<Result> UpdatePermissionsAsync(PermissionModel request);
}