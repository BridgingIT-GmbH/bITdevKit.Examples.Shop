namespace Modules.Catalog.Presentation.Web.Client.Components;

using FluentValidation;

public class ProductModelValidator : AbstractValidator<ProductModel>
{
    public ProductModelValidator()
    {
        this.RuleFor(m => m.Name)
            .NotEmpty()
            .MaximumLength(256);

        this.RuleFor(m => m.Description)
            .MaximumLength(2048);

        this.RuleFor(m => m.Barcode)
            .NotEmpty()
            .MaximumLength(256);

        this.RuleFor(m => m.Sku)
            .NotEmpty()
            .MaximumLength(256);

        this.RuleFor(m => m.Price)
            .NotEmpty()
            .GreaterThan(0)
            .LessThan(10000);

        this.RuleFor(m => m.Size)
            .MaximumLength(256);

        this.RuleFor(m => m.BrandId)
            .Must(id => id != Guid.Empty);

        this.RuleFor(m => m.TypeId)
            .Must(id => id != Guid.Empty);
    }
}