namespace Modules.Identity.Infrastructure;

using System.Globalization;
using System.Security.Claims;
using System.Text.Encodings.Web;
using BridgingIT.DevKit.Application.Collaboration;
using BridgingIT.DevKit.Common;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Web;
using Modules.Identity.Application;
using Modules.Identity.Infrastructure.EntityFramework;

public class UserService : IUserService
{
    private readonly ILogger<UserService> logger;
    private readonly UserManager<AppIdentityUser> userManager;
    private readonly RoleManager<AppIdentityRole> roleManager;
    private readonly IdentityDbContext identityDbContext;
    private readonly IStringLocalizer<UserService> localizer;
    private readonly IMailService mailService;
    private readonly ICurrentUserService currentUserService;
    private readonly IExcelInterchangeService excelService;
    private readonly ICacheProvider cacheProvider;

    public UserService(
        ILogger<UserService> logger, // TODO: use ILoggerFactory here!
        UserManager<AppIdentityUser> userManager,
        RoleManager<AppIdentityRole> roleManager,
        IdentityDbContext identityDbContext,
        IStringLocalizer<UserService> localizer,
        IMailService mailService,
        ICurrentUserService currentUserService,
        IExcelInterchangeService excelService,
        ICacheProvider cacheProvider = null)
    {
        this.logger = logger;
        this.userManager = userManager;
        this.roleManager = roleManager;
        this.identityDbContext = identityDbContext;
        this.localizer = localizer;
        this.mailService = mailService;
        this.currentUserService = currentUserService;
        this.excelService = excelService;
        this.cacheProvider = cacheProvider;
    }

    public async Task<Result<IEnumerable<UserModel>>> GetAllAsync()
    {
        var users = await this.userManager.Users.ToListAsync();

        return Result<IEnumerable<UserModel>>.Success(
            users.ConvertAll(u => this.Map(u)));
    }

    public async Task<IResult<string>> RegisterUserAsync(UserRegisterModel request, string origin)
    {
        var userWithSameUserName = await this.userManager.FindByNameAsync(request.UserName);
        if (userWithSameUserName != null)
        {
            return Result<string>.Failure(string.Format(this.localizer["Username {0} is already taken."], request.UserName));
        }

        var user = new AppIdentityUser
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            NormalizedEmail = request.Email.ToUpperInvariant(),
            UserName = request.UserName,
            NormalizedUserName = request.UserName.ToUpperInvariant(),
            EmailConfirmed = request.AutoConfirmEmail,
            PhoneNumber = request.PhoneNumber,
            PhoneNumberConfirmed = true,
            IsActive = request.ActivateUser,
        };

        //if (!string.IsNullOrWhiteSpace(request.PhoneNumber))
        //{
        //    var userWithSamePhoneNumber = await this.userManager.Users.FirstOrDefaultAsync(x => x.PhoneNumber == request.PhoneNumber);
        //    if (userWithSamePhoneNumber != null)
        //    {
        //        return await Result.Failure(string.Format(localizer["Phone number {0} is already registered."], request.PhoneNumber));
        //    }
        //}

        var userWithSameEmail = await this.userManager.FindByEmailAsync(request.Email);
        if (userWithSameEmail == null)
        {
            var result = await this.userManager.CreateAsync(user, request.Password);
            if (result.Succeeded)
            {
                await this.userManager.AddToRoleAsync(user, RoleConstants.BasicRole);
                if (!request.AutoConfirmEmail)
                {
                    var verificationUrl = await this.GetVerificationUrl(user, origin);
                    var mailRequest = new MailRequest
                    {
                        Sender = "no-reply@acmeshop.com",
                        Recipient = user.Email,
                        Subject = this.localizer["Confirm Registration"],
                        HtmlBody = string.Format(this.localizer["Please confirm your account by <a href='{0}'>clicking here</a>."], verificationUrl)
                    };
                    //BackgroundJob.Enqueue(() => this._mailService.SendAsync(mailRequest));
                    await this.mailService.SendAsync(mailRequest);
                    return Result<string>.Success(user.Id.ToString(), string.Format(this.localizer["User {0} Registered. Please check your Mailbox to verify!", user.UserName]));
                }

                return Result<string>.Success(user.Id.ToString(), string.Format(this.localizer["User {0} Registered.", user.UserName]));
            }
            else
            {
                return Result<string>.Failure(result.Errors.Select(a => a.Description).ToList());
            }
        }
        else
        {
            return Result<string>.Failure(string.Format(this.localizer["Email {0} is already registered.", request.Email]));
        }
    }

    public async Task<IResult<string>> CreateUserAsync(ClaimsPrincipal principal)
    {
        var objectId = principal.GetObjectId();
        if (string.IsNullOrWhiteSpace(objectId))
        {
            return Result<string>.Failure(string.Format(this.localizer["Principal {0} has no objectid."], principal.GetDisplayName()));
        }

        var user = await this.userManager.Users?.Where(u => u.ObjectId == objectId)?.FirstOrDefaultAsync()
            ?? await this.CreateAppUser(principal);

        if (principal.FindFirstValue(ClaimTypes.Role) is string role &&
            await this.roleManager.RoleExistsAsync(role) &&
            !await this.userManager.IsInRoleAsync(user, role))
        {
            await this.userManager.AddToRoleAsync(user, role);
        }

        return Result<string>.Success(user.Id.ToString(), string.Format(this.localizer["User {0} Registered.", user.UserName]));
    }

    private async Task<AppIdentityUser> CreateAppUser(ClaimsPrincipal principal) // rename UpsertUserAsync()
    {
        var email = principal.FindFirstValue(ClaimTypes.Upn);
        var userName = principal.GetDisplayName();
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(userName))
        {
            throw new Exception("Username or Email not valid.");
        }

        var user = await this.userManager.FindByNameAsync(userName);
        if (user is not null && !string.IsNullOrWhiteSpace(user.ObjectId))
        {
            throw new Exception($"Username {userName} is already taken.");
        }

        if (user is null)
        {
            user = await this.userManager.FindByEmailAsync(email);
            if (user is not null && !string.IsNullOrWhiteSpace(user.ObjectId))
            {
                throw new Exception($"Username {email} is already taken.");
            }
        }

        IdentityResult result;
        if (user is not null)
        {
            user.ObjectId = principal.GetObjectId();
            result = await this.userManager.UpdateAsync(user);

            await this.InvalidatePermissionsCacheAsync(user.Id.ToString());
        }
        else
        {
            user = new AppIdentityUser
            {
                ObjectId = principal.GetObjectId(),
                FirstName = principal.FindFirstValue(ClaimTypes.GivenName),
                LastName = principal.FindFirstValue(ClaimTypes.Surname),
                Email = email,
                NormalizedEmail = email.ToUpperInvariant(),
                UserName = userName,
                NormalizedUserName = userName.ToUpperInvariant(),
                EmailConfirmed = true,
                PhoneNumber = principal.FindFirstValue(ClaimTypes.HomePhone),
                PhoneNumberConfirmed = true,
                IsActive = true
            };
            result = await this.userManager.CreateAsync(user);
        }

        if (!result.Succeeded)
        {
            throw new Exception("Validation Errors Occurred: " + result.Errors.Select(e => e.Description).ToString(", "));
        }

        return user;
    }

    public async Task<IResult<UserModel>> GetUserAsync(string userId)
    {
        var user = await this.userManager.Users.Where(u => u.Id == Guid.Parse(userId)).FirstOrDefaultAsync();

        return Result<UserModel>.Success(
            this.Map(user, (await this.GetPermissionsAsync(userId)).Value));
    }

    public async Task<IResult> ToggleUserStatusAsync(string userId, bool activateUser)
    {
        var user = await this.userManager.FindByIdAsync(userId);
        var isAdmin = await this.userManager.IsInRoleAsync(user, RoleConstants.AdministratorRole);
        if (isAdmin)
        {
            return Result.Failure(this.localizer["Administrators Profile's Status cannot be toggled"]);
        }

        if (user != null)
        {
            user.IsActive = activateUser;
            var identityResult = await this.userManager.UpdateAsync(user);
        }

        return Result.Success();
    }

    public async Task<IResult> DeleteUserAsync(string userId)
    {
        var user = await this.userManager.FindByIdAsync(userId);
        var isAdmin = await this.userManager.IsInRoleAsync(user, RoleConstants.AdministratorRole);
        if (isAdmin)
        {
            return Result.Failure(this.localizer["Administrators Profile's cannot be removed"]);
        }

        if (user != null)
        {
            var identityResult = await this.userManager.DeleteAsync(user);
        }

        await this.InvalidatePermissionsCacheAsync(userId);

        return Result.Success();
    }

    public async Task<IResult<UserRolesModel>> GetRolesAsync(string userId)
    {
        var user = await this.userManager.FindByIdAsync(userId);
        var roles = await this.roleManager.Roles.ToListAsync();
        var models = new List<UserRoleModel>();

        foreach (var role in roles)
        {
            var model = new UserRoleModel
            {
                RoleName = role.Name,
                RoleDescription = role.Description,
                Selected = await this.userManager.IsInRoleAsync(user, role.Name)
            };
            models.Add(model);
        }

        var result = new UserRolesModel { UserRoles = models };
        return Result<UserRolesModel>.Success(result);
    }

    public async Task<IResult> UpdateRolesAsync(UpdateUserRolesModel request)
    {
        var user = await this.userManager.FindByIdAsync(request.UserId);
        if (user.Email == "admin@acmeshop.com")
        {
            return Result.Failure(this.localizer["Not Allowed."]);
        }

        var existingRoles = await this.userManager.GetRolesAsync(user);
        var roles = request.UserRoles.Where(x => x.Selected).ToList();

        var currentUser = await this.userManager.FindByIdAsync(this.currentUserService.UserId);
        if (!await this.userManager.IsInRoleAsync(currentUser, RoleConstants.AdministratorRole))
        {
            var tryToAddAdministratorRole = roles
                .Any(x => x.RoleName == RoleConstants.AdministratorRole);
            var userHasAdministratorRole = existingRoles.Any(x => x == RoleConstants.AdministratorRole);
            if ((tryToAddAdministratorRole && !userHasAdministratorRole) || (!tryToAddAdministratorRole && userHasAdministratorRole))
            {
                return Result.Failure(this.localizer["Not Allowed to add or delete Administrator Role if you have not this role."]);
            }
        }

        await this.userManager.RemoveFromRolesAsync(user, existingRoles);
        await this.userManager.AddToRolesAsync(user, roles.Select(ur => ur.RoleName));
        await this.InvalidatePermissionsCacheAsync(user.Id.ToString());

        return Result.Success(this.localizer["Roles Updated"]);
    }

    public async Task<IResult<IEnumerable<string>>> GetPermissionsAsync(string userId)
    {
        if (this.cacheProvider != null && this.cacheProvider.TryGet($"identity-permissions-{userId}", out IEnumerable<string> cachedResult))
        {
            return Result<IEnumerable<string>>.Success(cachedResult);
        }
        else
        {
            var user = await this.userManager.FindByIdAsync(userId);
            var userRoles = await this.userManager.GetRolesAsync(user);
            var permissions = new List<string>();

            foreach (var role in await this.roleManager.Roles
                .Where(r => userRoles.Contains(r.Name!))
                .ToListAsync())
            {
                permissions.AddRange(await this.identityDbContext.RoleClaims
                    .Where(c => c.RoleId == role.Id && c.ClaimType == "Permission")
                    .Select(c => c.ClaimValue!)
                    .ToListAsync());
            }

            this.cacheProvider?.Set($"identity-permissions-{userId}", permissions);

            return Result<IEnumerable<string>>.Success(permissions.Distinct());
        }
    }

    public async Task<IResult<bool>> HasPermissionAsync(string userId, string permission)
    {
        var permissions = (await this.GetPermissionsAsync(userId)).Value;

        return Result<bool>.Success(permissions?.Contains(permission) ?? false);
    }

    public async Task<IResult<string>> ConfirmEmailAsync(string userId, string code)
    {
        var user = await this.userManager.FindByIdAsync(userId);
        code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
        var result = await this.userManager.ConfirmEmailAsync(user, code);
        if (result.Succeeded)
        {
            return Result<string>.Success(user.Id.ToString(), string.Format(this.localizer["Account Confirmed for {0}. You can now use the /api/identity/token endpoint to generate JWT.", user.Email]));
        }
        else
        {
            throw new Exception(string.Format(this.localizer["An error occurred while confirming {0}", user.Email]));
        }
    }

    public async Task<IResult> ForgotPasswordAsync(string email, string origin)
    {
        var user = await this.userManager.FindByEmailAsync(email);
        if (user == null || !await this.userManager.IsEmailConfirmedAsync(user))
        {
            // Don't reveal that the user does not exist or is not confirmed
            return Result.Failure(this.localizer["An Error has occurred!"]);
        }

        // For more information on how to enable account confirmation and password reset please
        // visit https://go.microsoft.com/fwlink/?LinkID=532713
        var code = await this.userManager.GeneratePasswordResetTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
        const string route = "account/reset-password";
        var endpointUri = new Uri(string.Concat($"{origin}/", route));
        var passwordResetUrl = QueryHelpers.AddQueryString(endpointUri.ToString(), "Token", code);
        var mailRequest = new MailRequest
        {
            Sender = "no-reply@acmeshop.com",
            Recipient = email,
            Subject = this.localizer["Reset Password"],
            HtmlBody = string.Format(this.localizer["Please reset your password by <a href='{0}'>clicking here</a>.", HtmlEncoder.Default.Encode(passwordResetUrl)])
        };
        //BackgroundJob.Enqueue(() => this._mailService.SendAsync(mailRequest));
        await this.mailService.SendAsync(mailRequest);

        return Result.Success(this.localizer["Password Reset Mail has been sent to your authorized Email."]);
    }

    public async Task<IResult> ResetPasswordAsync(string email, string password, string token)
    {
        var user = await this.userManager.FindByEmailAsync(email);
        if (user == null)
        {
            return Result.Failure(this.localizer["An Error has occured!"]) // user not found
                .WithError<NotFoundResultError>();
        }

        var result = await this.userManager.ResetPasswordAsync(user, token, password);
        if (result.Succeeded)
        {
            return Result.Success(this.localizer["Password Reset Successful!"]);
        }
        else
        {
            return Result.Failure(this.localizer["An Error has occured!"]);
        }
    }

    public async Task<int> GetUserCountAsync()
    {
        return await this.userManager.Users.CountAsync();
    }

    public async Task<string> ExportToExcelAsync(string searchString = "")
    {
        var users = string.IsNullOrEmpty(searchString)
            ? await this.userManager.Users
                .OrderByDescending(a => a.CreatedOn)
                .ToListAsync()
            : await this.userManager.Users
                .Where(u => u.FirstName.Contains(searchString) || u.LastName.Contains(searchString) || u.Email.Contains(searchString) || u.PhoneNumber.Contains(searchString) || u.UserName.Contains(searchString))
                .OrderByDescending(a => a.CreatedOn)
                .ToListAsync();

        return await this.excelService.ExportAsync(
            users,
            mappers: new Dictionary<string, Func<AppIdentityUser, object>>
            {
                { this.localizer["Id"], item => item.Id },
                { this.localizer["FirstName"], item => item.FirstName },
                { this.localizer["LastName"], item => item.LastName },
                { this.localizer["UserName"], item => item.UserName },
                { this.localizer["Email"], item => item.Email },
                { this.localizer["EmailConfirmed"], item => item.EmailConfirmed },
                { this.localizer["PhoneNumber"], item => item.PhoneNumber },
                { this.localizer["PhoneNumberConfirmed"], item => item.PhoneNumberConfirmed },
                { this.localizer["IsActive"], item => item.IsActive },
                { this.localizer["CreatedOn (Local)"], item => DateTime.SpecifyKind(item.CreatedOn, DateTimeKind.Utc).ToLocalTime().ToString("G", CultureInfo.CurrentCulture) },
                { this.localizer["CreatedOn (UTC)"], item => item.CreatedOn.ToString("G", CultureInfo.CurrentCulture) },
                { this.localizer["ImageUrl"], item => item.ImageUrl },
            }, new ExcelInterchangeServiceOptions { SheetName = this.localizer["Users"] });
    }

    private async Task<string> GetVerificationUrl(AppIdentityUser user, string origin)
    {
        var code = await this.userManager.GenerateEmailConfirmationTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
        var endpointUri = new Uri(string.Concat($"{origin}/", "api/identity/user/confirm-email/"));
        var verificationUri = QueryHelpers.AddQueryString(endpointUri.ToString(), "userId", user.Id.ToString());

        return QueryHelpers.AddQueryString(verificationUri, "code", code);
    }

    private UserModel Map(AppIdentityUser source, IEnumerable<string> permissions = null)
    {
        return new UserModel
        {
            Email = source.Email,
            EmailConfirmed = source.EmailConfirmed,
            FirstName = source.FirstName,
            Id = source.Id.ToString(),
            IsActive = source.IsActive,
            LastName = source.LastName,
            PhoneNumber = source.PhoneNumber,
            ImageUrl = source.ImageUrl,
            UserName = source.UserName,
            Permissions = permissions
        };
    }

    private async Task InvalidatePermissionsCacheAsync(string userId)
    {
        await this.cacheProvider?.RemoveAsync($"identity-permissions-{userId}");
    }
}