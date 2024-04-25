namespace Modules.Identity.Presentation.Web.Client.Models;

using FluentValidation;
using Microsoft.Extensions.Localization;

public class RoleRequestModelValidator : AbstractValidator<RoleRequestModel>
{
    public RoleRequestModelValidator(IStringLocalizer<RoleRequestModelValidator> localizer)
    {
        this.RuleFor(request => request.Name)
            .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage(_ => localizer["Name is required"]);
    }
}