namespace Modules.Identity.Presentation.Web.Client.Models;

using FluentValidation;
using Microsoft.Extensions.Localization;

public class ChangePasswordRequestModelValidator : AbstractValidator<ChangePasswordRequestModel>
{
    public ChangePasswordRequestModelValidator(IStringLocalizer<ChangePasswordRequestModelValidator> localizer)
    {
        this.RuleFor(request => request.Password)
            .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage(_ => localizer["Current Password is required!"]);
        this.RuleFor(request => request.NewPassword)
            .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage(_ => localizer["Password is required!"])
            .MinimumLength(8).WithMessage(localizer["Password must be at least of length 8"])
            .Matches("[A-Z]").WithMessage(localizer["Password must contain at least one capital letter"])
            .Matches("[a-z]").WithMessage(localizer["Password must contain at least one lowercase letter"])
            .Matches("[0-9]").WithMessage(localizer["Password must contain at least one digit"]);
        this.RuleFor(request => request.ConfirmNewPassword)
            .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage(_ => localizer["Password Confirmation is required!"])
            .Equal(request => request.NewPassword).WithMessage(_ => localizer["Passwords don't match"]);
    }
}