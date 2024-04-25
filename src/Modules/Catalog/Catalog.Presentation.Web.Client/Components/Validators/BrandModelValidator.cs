namespace Modules.Catalog.Presentation.Web.Client.Components;

using FluentValidation;

public class BrandModelValidator : AbstractValidator<BrandModel>
{
    public BrandModelValidator()
    {
        this.RuleFor(m => m.Name)
            .NotEmpty()
            .MaximumLength(256);

        this.RuleFor(m => m.Description)
            .MaximumLength(2048);
    }
}