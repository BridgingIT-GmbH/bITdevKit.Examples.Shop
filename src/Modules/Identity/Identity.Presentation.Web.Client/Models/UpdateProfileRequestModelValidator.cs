namespace Modules.Identity.Presentation.Web.Client.Models;

using FluentValidation;
using Microsoft.Extensions.Localization;

public class UpdateProfileRequestModelValidator : AbstractValidator<UpdateProfileRequestModel>
{
    public UpdateProfileRequestModelValidator(IStringLocalizer<UpdateProfileRequestModelValidator> localizer)
    {
        this.RuleFor(request => request.FirstName)
            .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage(_ => localizer["First Name is required"]);
        this.RuleFor(request => request.LastName)
            .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage(_ => localizer["Last Name is required"]);
    }
}