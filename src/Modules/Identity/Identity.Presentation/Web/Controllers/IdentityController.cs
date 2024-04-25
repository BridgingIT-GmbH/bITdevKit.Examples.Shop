namespace Modules.Identity.Presentation.Web.Controllers;

using System.Threading.Tasks;
using BridgingIT.DevKit.Common;
using BridgingIT.DevKit.Presentation.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Modules.Identity.Application;

[ApiController]
[Authorize]
public class IdentityController : IdentityControllerBase
{
    private readonly IMapper mapper;
    private readonly ITokenService tokenService;
    private readonly IAccountService accountService;
    private readonly IUserService userService;
    private readonly IRoleService roleService;
    private readonly IRoleClaimService roleClaimService;
    private readonly ICurrentUserService currentUserService;

    public IdentityController(
        IMapper mapper,
        ITokenService tokenService,
        IAccountService accountService,
        IUserService userService,
        IRoleService roleService,
        IRoleClaimService roleClaimService,
        ICurrentUserService currentUserService)
    {
        this.mapper = mapper;
        this.tokenService = tokenService;
        this.accountService = accountService;
        this.userService = userService;
        this.roleService = roleService;
        this.roleClaimService = roleClaimService;
        this.currentUserService = currentUserService;
    }

    public override async Task<ActionResult<ResultResponseModel>> EchoGet()
    {
        await Task.Delay(1);
        return new OkObjectResult(Result.Success().WithMessage("echo"));
    }

    // Accounts ======================================================================================
    public override async Task<ActionResult<ResultOfUserResponseModel>> AccountGetProfile()
    {
        var result = await this.userService.GetUserAsync(this.currentUserService.UserId);

        return result.ToOkActionResult<UserModel, ResultOfUserResponseModel>(this.mapper);
    }

    public override async Task<ActionResult<ResultResponseModel>> AccountChangePassword([FromBody] ChangePasswordRequestModel model)
    {
        var result = await this.accountService.ChangePasswordAsync(model.Password, model.NewPassword, this.currentUserService.UserId);

        return result.ToOkActionResult<ResultResponseModel>(this.mapper);
    }

    public override async Task<ActionResult<ResultOfStringResponseModel>> AccountGetProfilePicture(string userId)
    {
        var result = await this.accountService.GetProfilePictureAsync(userId);

        return result.ToOkActionResult<string, ResultOfStringResponseModel>(this.mapper);
    }

    public override async Task<ActionResult<ResultResponseModel>> AccountUpdateProfile([FromBody] UpdateProfileRequestModel model)
    {
        var result = await this.accountService.UpdateProfileAsync(model.Email, model.FirstName, model.LastName, model.PhoneNumber, this.currentUserService.UserId);

        return result.ToOkActionResult<ResultResponseModel>(this.mapper);
    }

    public override async Task<ActionResult<ResultOfStringResponseModel>> AccountUpdateProfilePicture(string userId, [FromBody] UpdateProfilePictureRequestModel model)
    {
        var result = await this.accountService.UpdateProfilePictureAsync(model.FileName, model.Extension, model.Data, this.currentUserService.UserId);

        return result.ToOkActionResult<string, ResultOfStringResponseModel>(this.mapper);
    }

    // Tokens ======================================================================================
    [AllowAnonymous]
    public override async Task<ActionResult<ResultOfTokenResponseModel>> TokenAcquire([FromBody] TokenRequestModel model)
    {
        var result = await this.tokenService.GetTokenAsync(model.Email, model.Password);

        return result.ToOkActionResult<TokenModel, ResultOfTokenResponseModel>(this.mapper);
    }

    [AllowAnonymous]
    public override async Task<ActionResult<ResultOfTokenResponseModel>> TokenRefresh([FromBody] TokenRefreshRequestModel model)
    {
        var result = await this.tokenService.GetRefreshTokenAsync(model.Token, model.RefreshToken);

        return result.ToOkActionResult<TokenModel, ResultOfTokenResponseModel>(this.mapper);
    }

    // Users ======================================================================================
    [Authorize(Policy = IdentityPermissionSet.Users.View)] // TODO: use [AuthorizePermission(IdentityPermissionSet.Users.View)]
    public override async Task<ActionResult<ResultOfUsersResponseModel>> UserGetAll()
    {
        var result = await this.userService.GetAllAsync();

        return result.ToOkActionResult<IEnumerable<UserModel>, ResultOfUsersResponseModel>(this.mapper);
    }

    [AllowAnonymous]
    public override async Task<ActionResult<ResultResponseModel>> UserRegister([FromBody] RegisterRequestModel model)
    {
        var result = await this.userService.RegisterUserAsync(
            new UserRegisterModel
            {
                ActivateUser = model.ActivateUser,
                AutoConfirmEmail = model.AutoConfirmEmail,
                ConfirmPassword = model.ConfirmPassword,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Password = model.Password,
                PhoneNumber = model.PhoneNumber,
                UserName = model.UserName,
            }, this.Request.Headers["origin"]);

        return result.ToOkActionResult<ResultResponseModel>(this.mapper);
    }

    [Authorize(Policy = IdentityPermissionSet.Users.Delete)] // TODO: use [AuthorizePermission(IdentityPermissionSet.Users.Delete)]
    public override async Task<ActionResult<ResultResponseModel>> UserDelete([FromQuery] string id)
    {
        var result = await this.userService.DeleteUserAsync(id);

        return result.ToOkActionResult<ResultResponseModel>(this.mapper);
    }

    [Authorize(Policy = IdentityPermissionSet.Users.View)] // TODO: use [AuthorizePermission(IdentityPermissionSet.Users.View)]
    public override async Task<ActionResult<ResultOfUserResponseModel>> UserGetById(string id)
    {
        var result = await this.userService.GetUserAsync(id);

        return result.ToOkActionResult<UserModel, ResultOfUserResponseModel>(this.mapper);
    }

    [Authorize(Policy = IdentityPermissionSet.Users.View)] // TODO: use [AuthorizePermission(IdentityPermissionSet.Users.View)]
    public override async Task<ActionResult<ResultOfUserRolesResponseModel>> UserGetRoles(string id)
    {
        var result = await this.userService.GetRolesAsync(id);

        return result.ToOkActionResult<UserRolesModel, ResultOfUserRolesResponseModel>(this.mapper);
    }

    [Authorize(Policy = IdentityPermissionSet.Users.Edit)] // TODO: use [AuthorizePermission(IdentityPermissionSet.Users.Edit)]
    public override async Task<ActionResult<ResultResponseModel>> UserUpdateRoles(string id, [FromBody] UpdateUserRolesRequestModel model)
    {
        var result = await this.userService.UpdateRolesAsync(
            new UpdateUserRolesModel
            {
                UserId = model.UserId,
                UserRoles = model.UserRoles.Select(r => new Application.UserRoleModel { RoleName = r.RoleName, RoleDescription = r.RoleDescription, Selected = r.Selected }).ToList()
            });

        return result.ToOkActionResult<ResultResponseModel>(this.mapper);
    }

    [AllowAnonymous]
    public override async Task<ActionResult<ResultOfStringResponseModel>> UserConfirmEmail([FromQuery] string userId, [FromQuery] string code)
    {
        var result = await this.userService.ConfirmEmailAsync(userId, code);

        return result.ToOkActionResult<ResultOfStringResponseModel>(this.mapper);
    }

    public override async Task<ActionResult<ResultResponseModel>> UserToggleUserStatus([FromBody] ToggleUserStatusRequestModel model)
    {
        var result = await this.userService.ToggleUserStatusAsync(model.UserId, model.ActivateUser);

        return result.ToOkActionResult<ResultResponseModel>(this.mapper);
    }

    [AllowAnonymous]
    public override async Task<ActionResult<ResultResponseModel>> UserForgotPassword([FromBody] ForgotPasswordRequestModel model)
    {
        var result = await this.userService.ForgotPasswordAsync(model.Email, this.Request.Headers["origin"]);

        return result.ToOkActionResult<ResultResponseModel>(this.mapper);
    }

    [AllowAnonymous]
    public override async Task<ActionResult<ResultResponseModel>> UserResetPassword([FromBody] ResetPasswordRequestModel model)
    {
        var result = await this.userService.ResetPasswordAsync(model.Email, model.Password, model.Token);

        return result.ToOkActionResult<ResultResponseModel>(this.mapper);
    }

    [Authorize(Policy = IdentityPermissionSet.Users.Export)] // TODO: use [AuthorizePermission(IdentityPermissionSet.Users.Export)]
    public override async Task<ActionResult<ResultOfStringResponseModel>> UserExport([FromQuery] string searchString = "")
    {
        var data = await this.userService.ExportToExcelAsync(searchString);
        var result = new Result<string>
        {
            Value = data
        };

        return result.ToOkActionResult<ResultOfStringResponseModel>(this.mapper);
    }

    // Roles ======================================================================================
    [Authorize(Policy = IdentityPermissionSet.Roles.View)] // TODO: use [AuthorizePermission(IdentityPermissionSet.Roles.View)]
    public override async Task<ActionResult<ResultOfRolesResponseModel>> RoleGetAll()
    {
        var result = await this.roleService.GetAllAsync();

        return result.ToOkActionResult<IEnumerable<RoleModel>, ResultOfRolesResponseModel>(this.mapper);
    }

    [Authorize(Policy = IdentityPermissionSet.Roles.Create)] // TODO: use [AuthorizePermission(IdentityPermissionSet.Roles.Create)]
    public override async Task<ActionResult<ResultResponseModel>> RoleCreate([FromBody] RoleRequestModel model)
    {
        var result = await this.roleService.SaveAsync(
            new RoleModel
            {
                Id = model.Id,
                Name = model.Name,
                Description = model.Description
            });

        return result.ToOkActionResult<ResultResponseModel>(this.mapper);
    }

    [Authorize(Policy = IdentityPermissionSet.Roles.Delete)] // TODO: use [AuthorizePermission(IdentityPermissionSet.Roles.Delete)]
    public override async Task<ActionResult<ResultResponseModel>> RoleDelete(string id)
    {
        var result = await this.roleService.DeleteAsync(id);

        return result.ToOkActionResult<ResultResponseModel>(this.mapper);
    }

    [Authorize(Policy = IdentityPermissionSet.RoleClaims.View)] // TODO: use [AuthorizePermission(IdentityPermissionSet.RoleClaims.View)]
    public override async Task<ActionResult<ResultOfPermissionResponseModel>> RoleGetPermissionsByRoleId(string roleId)
    {
        var result = await this.roleService.GetAllPermissionsAsync(roleId);

        return result.ToOkActionResult<PermissionModel, ResultOfPermissionResponseModel>(this.mapper);
    }

    [Authorize(Policy = IdentityPermissionSet.RoleClaims.Edit)] // TODO: use [AuthorizePermission(IdentityPermissionSet.RoleClaims.Edit)]
    public override async Task<ActionResult<ResultResponseModel>> RoleUpdate([FromBody] PermissionRequestModel model)
    {
        var result = await this.roleService.UpdatePermissionsAsync(new PermissionModel
        {
            RoleId = model.RoleId,
            RoleClaims = model.RoleClaims.Select(r => new RoleClaimModel
            {
                Id = r.Id,
                Group = r.Group,
                Description = r.Description,
                Type = r.Type,
                RoleId = r.RoleId,
                Value = r.Value,
                Selected = r.Selected
            }).ToList()
        });

        return result.ToOkActionResult<ResultResponseModel>(this.mapper);
    }

    // RoleClaims ======================================================================================
    [Authorize(Policy = IdentityPermissionSet.RoleClaims.View)] // TODO: use [AuthorizePermission(IdentityPermissionSet.RoleClaims.View)]
    public override async Task<ActionResult<ResultOfRoleClaimsResponseModel>> RoleClaimGetAll()
    {
        var result = await this.roleClaimService.GetAllAsync();

        return result.ToOkActionResult<IEnumerable<RoleClaimModel>, ResultOfRoleClaimsResponseModel>(this.mapper);
    }

    [Authorize(Policy = IdentityPermissionSet.RoleClaims.View)] // TODO: use [AuthorizePermission(IdentityPermissionSet.RoleClaims.View)]
    public override async Task<ActionResult<ResultOfRoleClaimsResponseModel>> RoleClaimGetAllByRoleId(string roleId)
    {
        var result = await this.roleClaimService.GetAllByRoleIdAsync(roleId);

        return result.ToOkActionResult<IEnumerable<RoleClaimModel>, ResultOfRoleClaimsResponseModel>(this.mapper);
    }

    [Authorize(Policy = IdentityPermissionSet.RoleClaims.Create)] // TODO: use [AuthorizePermission(IdentityPermissionSet.RoleClaims.Create)]
    public override async Task<ActionResult<ResultResponseModel>> RoleClaimCreate([FromBody] RoleClaimRequestModel request)
    {
        var result = await this.roleClaimService.SaveAsync(
                new RoleClaimModel
                {
                    Id = request.Id,
                    RoleId = request.RoleId,
                    Description = request.Description,
                    Group = request.Group,
                    Selected = request.Selected,
                    Type = request.Type,
                    Value = request.Value
                });

        return result.ToOkActionResult<ResultResponseModel>(this.mapper);
    }

    [Authorize(Policy = IdentityPermissionSet.RoleClaims.Delete)] // TODO: use [AuthorizePermission(IdentityPermissionSet.RoleClaims.Delete)]
    public override async Task<ActionResult<ResultResponseModel>> RoleClaimDelete(int id)
    {
        var result = await this.roleClaimService.DeleteAsync(id);

        return result.ToOkActionResult<ResultResponseModel>(this.mapper);
    }
}