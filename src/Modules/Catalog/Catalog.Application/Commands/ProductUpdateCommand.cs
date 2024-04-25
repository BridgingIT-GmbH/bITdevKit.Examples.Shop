namespace Modules.Catalog.Application.Commands;

using BridgingIT.DevKit.Application.Entities;
using FluentValidation;
using Modules.Catalog.Domain;

[GeneratedControllerApi]
public class ProductUpdateCommand : EntityUpdateCommandBase<Product>
{
    public ProductUpdateCommand(Product entity, string identity = null)
        : base(entity, identity)
    {
        this.AddValidator<ProductUpdateValidator>();
    }

    public class ProductUpdateValidator : AbstractValidator<EntityUpdateCommandBase<Product>>
    {
        public ProductUpdateValidator()
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