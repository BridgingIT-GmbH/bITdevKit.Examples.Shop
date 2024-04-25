namespace Modules.Catalog.Application.Commands;

using BridgingIT.DevKit.Application.Entities;
using FluentValidation;
using Modules.Catalog.Domain;

[GeneratedControllerApi(Module = "Catalog", Policy = CatalogPermissionSet.Brands.Create)]
public class BrandCreateCommand : EntityCreateCommandBase<Brand>
{
    public BrandCreateCommand(Brand entity, string identity = null)
        : base(entity, identity)
    {
        this.AddValidator<BrandCreateValidator>();
    }

    public class BrandCreateValidator : AbstractValidator<EntityCreateCommandBase<Brand>>
    {
        public BrandCreateValidator()
        {
            this.RuleFor(c => c.Entity).NotNull().NotEmpty().ChildRules(c =>
            {
                c.RuleFor(c => c.Name).NotNull().NotEmpty().WithMessage("Must not be empty.");
                c.RuleFor(c => c.Name).MinimumLength(3).WithMessage("Must not be too short.");
            });
        }
    }
}