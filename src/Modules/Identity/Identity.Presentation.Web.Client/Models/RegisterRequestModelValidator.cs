namespace Modules.Identity.Presentation.Web.Client.Models;

using FluentValidation;
using Microsoft.Extensions.Localization;

public class RegisterRequestModelValidator : AbstractValidator<RegisterRequestModel>
{
    public RegisterRequestModelValidator(IStringLocalizer<RegisterRequestModelValidator> localizer)
    {
        this.RuleFor(request => request.FirstName)
            .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage(_ => localizer["First Name is required"]);
        this.RuleFor(request => request.LastName)
            .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage(_ => localizer["Last Name is required"]);
        this.RuleFor(request => request.Email)
            .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage(_ => localizer["Email is required"])
            .EmailAddress().WithMessage(_ => localizer["Email is not correct"]);
        this.RuleFor(request => request.UserName)
            .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage(_ => localizer["UserName is required"])
            .MinimumLength(6).WithMessage(localizer["UserName must be at least of length 6"]);
        this.RuleFor(request => request.Password)
            .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage(_ => localizer["Password is required"])
            .MinimumLength(8).WithMessage(localizer["Password must be at least of length 8"])
            .Matches("[A-Z]").WithMessage(localizer["Password must contain at least one capital letter"])
            .Matches("[a-z]").WithMessage(localizer["Password must contain at least one lowercase letter"])
            .Matches("[0-9]").WithMessage(localizer["Password must contain at least one digit"]);
        this.RuleFor(request => request.ConfirmPassword)
            .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage(_ => localizer["Password Confirmation is required"])
            .Equal(request => request.Password).WithMessage(_ => localizer["Passwords don't match"]);
    }
}