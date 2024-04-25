namespace Modules.Identity.Presentation.Web.Client.Models;

using FluentValidation;
using Microsoft.Extensions.Localization;

public class TokenRequestModelValidator : AbstractValidator<TokenRequestModel>
{
    public TokenRequestModelValidator(IStringLocalizer<TokenRequestModelValidator> localizer)
    {
        this.RuleFor(request => request.Email)
            .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage(_ => localizer["Email is required"])
            .EmailAddress().WithMessage(x => localizer["Email is not correct"]);
        this.RuleFor(request => request.Password)
            .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage(_ => localizer["Password is required"]);
    }
}