namespace Modules.Catalog.Application.Commands;

using BridgingIT.DevKit.Application.Entities;
using FluentValidation;
using Modules.Catalog.Domain;

[GeneratedControllerApi]
public class ProductCreateCommand : EntityCreateCommandBase<Product>
{
    public ProductCreateCommand(Product entity, string identity = null)
        : base(entity, identity)
    {
        this.AddValidator<ProductCreateValidator>();
    }

    public class ProductCreateValidator : AbstractValidator<EntityCreateCommandBase<Product>>
    {
        public ProductCreateValidator()
        {
            this.RuleFor(c => c.Entity).NotNull().NotEmpty().ChildRules(c =>
            {
                c.RuleFor(c => c.Name).NotNull().NotEmpty().WithMessage("Must not be empty.");
                c.RuleFor(m => m.Price).GreaterThan(0).WithMessage("Invalid money value.");
                c.RuleFor(c => c.Barcode).NotNull().NotEmpty().WithMessage("Must not be empty.");
                c.RuleFor(c => c.BrandId).Must(id => id != Guid.Empty).WithMessage("Invalid guid.");
                c.RuleFor(c => c.TypeId).Must(id => id != Guid.Empty).WithMessage("Invalid guid.");
            });
        }
    }
}