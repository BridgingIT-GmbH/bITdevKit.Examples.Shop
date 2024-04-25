namespace Modules.Identity.Presentation.Web.Client.Models;

using FluentValidation;
using Microsoft.Extensions.Localization;

public class ForgotPasswordRequestModelValidator : AbstractValidator<ForgotPasswordRequestModel>
{
    public ForgotPasswordRequestModelValidator(IStringLocalizer<ForgotPasswordRequestModelValidator> localizer)
    {
        this.RuleFor(request => request.Email)
            .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage(_ => localizer["Email is required"])
            .EmailAddress().WithMessage(_ => localizer["Email is not correct"]);
    }
}