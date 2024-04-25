namespace Modules.Identity.Infrastructure;

using BridgingIT.DevKit.Common;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Modules.Identity.Application;

public class AccountService : IAccountService
{
    private readonly ILogger<AccountService> logger;
    private readonly UserManager<AppIdentityUser> userManager;
    private readonly SignInManager<AppIdentityUser> signInManager;
    private readonly IStringLocalizer<AccountService> localizer;

    //private readonly IUploadService uploadService;

    public AccountService(
        ILogger<AccountService> logger,
        UserManager<AppIdentityUser> userManager,
        SignInManager<AppIdentityUser> signInManager,
        IStringLocalizer<AccountService> localizer)
    //IUploadService uploadService
    {
        this.logger = logger;
        this.userManager = userManager;
        this.signInManager = signInManager;
        this.localizer = localizer;
        //this.uploadService = uploadService;
    }

    public async Task<Result> ChangePasswordAsync(string password, string newPassword, string userId)
    {
        var user = await this.userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return Result.Failure("User Not Found.");
        }

        var identityResult = await this.userManager.ChangePasswordAsync(
            user,
            password,
            newPassword);
        var errors = identityResult.Errors.Select(e => e.Description).ToList();
        return identityResult.Succeeded ? Result.Success() : Result.Failure(errors);
    }

    public async Task<Result> UpdateProfileAsync(string email, string firstName, string lastName, string phoneNumber, string userId)
    {
        //if (!string.IsNullOrWhiteSpace(request.PhoneNumber))
        //{
        //    var userWithSamePhoneNumber = await this.userManager.Users.FirstOrDefaultAsync(x => x.PhoneNumber == request.PhoneNumber);
        //    if (userWithSamePhoneNumber != null)
        //    {
        //        return await Result.FailAsync(string.Format("Phone number {0} is already used.", request.PhoneNumber));
        //    }
        //}

        var userWithSameEmail = await this.userManager.FindByEmailAsync(email);
        if (userWithSameEmail == null || userWithSameEmail.Id == Guid.Parse(userId))
        {
            var user = await this.userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return Result.Failure("User Not Found.");
            }

            user.FirstName = firstName;
            user.LastName = lastName;
            user.PhoneNumber = phoneNumber;
            if (phoneNumber != await this.userManager.GetPhoneNumberAsync(user))
            {
                var setPhoneResult = await this.userManager.SetPhoneNumberAsync(user, phoneNumber);
            }

            var identityResult = await this.userManager.UpdateAsync(user);
            var errors = identityResult.Errors.Select(e => e.Description).ToList();
            await this.signInManager.RefreshSignInAsync(user);

            return identityResult.Succeeded ? Result.Success() : Result.Failure(errors);
        }
        else
        {
            return Result.Failure(string.Format("Email {0} is already used.", email));
        }
    }

    // TODO: store profile picture in db instead of filesystem
    public async Task<Result<string>> GetProfilePictureAsync(string userId)
    {
        return await Task.FromResult(Result<string>.Success(string.Empty, string.Empty));
        //    var user = await this._userManager.FindByIdAsync(userId);
        //    if (user == null)
        //    {
        //        return await Result<string>.FailAsync("User Not Found");
        //    }

        //    return await Result<string>.SuccessAsync(value: user.ImageUrl);
    }

    // TODO: store profile picture in db instead of filesystem (DocumentStorage)
    public async Task<Result<string>> UpdateProfilePictureAsync(string fileName, string extension, byte[] data, string userId)
    {
        return await Task.FromResult(Result<string>.Success(string.Empty, string.Empty));
        //    var user = await this._userManager.FindByIdAsync(userId);
        //    if (user == null)
        //    {
        //        return await Result<string>.FailAsync(message: "User Not Found");
        //    }

        //    var filePath = this.uploadService.UploadAsync(request);
        //    user.ImageUrl = filePath;
        //    var identityResult = await this._userManager.UpdateAsync(user);
        //    var errors = identityResult.Errors.Select(e => e.Description).ToList();

        //    return identityResult.Succeeded
        //        ? await Result<string>.SuccessAsync(value: filePath)
        //        : await Result<string>.FailAsync(errors);
    }
}
