namespace Modules.Identity.Application;

using System.Security.Claims;
using System.Threading.Tasks;
using BridgingIT.DevKit.Common;

public interface IUserService
{
    Task<Result<IEnumerable<UserModel>>> GetAllAsync();

    Task<int> GetUserCountAsync();

    Task<IResult<UserModel>> GetUserAsync(string userId);

    Task<IResult<string>> RegisterUserAsync(UserRegisterModel request, string origin);

    Task<IResult<string>> CreateUserAsync(ClaimsPrincipal principal);

    Task<IResult> ToggleUserStatusAsync(string userId, bool activateUser);

    Task<IResult> DeleteUserAsync(string userId);

    Task<IResult<UserRolesModel>> GetRolesAsync(string id);

    Task<IResult<IEnumerable<string>>> GetPermissionsAsync(string userId);

    Task<IResult<bool>> HasPermissionAsync(string userId, string permission);

    Task<IResult> UpdateRolesAsync(UpdateUserRolesModel request);

    Task<IResult<string>> ConfirmEmailAsync(string userId, string code);

    Task<IResult> ForgotPasswordAsync(string email, string origin);

    Task<IResult> ResetPasswordAsync(string email, string password, string token);

    Task<string> ExportToExcelAsync(string searchString = "");
}