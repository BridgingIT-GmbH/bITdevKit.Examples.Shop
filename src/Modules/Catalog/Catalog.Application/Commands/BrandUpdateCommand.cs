namespace Modules.Catalog.Application.Commands;

using BridgingIT.DevKit.Application.Entities;
using FluentValidation;
using Modules.Catalog.Domain;

[GeneratedControllerApi(Module = "Catalog", Policy = CatalogPermissionSet.Brands.Edit)]
public class BrandUpdateCommand : EntityUpdateCommandBase<Brand>
{
    public BrandUpdateCommand(Brand entity, string identity = null)
        : base(entity, identity)
    {
        this.AddValidator<BrandUpdateValidator>();
    }

    public class BrandUpdateValidator : AbstractValidator<EntityUpdateCommandBase<Brand>>
    {
        public BrandUpdateValidator()
        {
            this.RuleFor(c => c.Entity).NotNull().NotEmpty().ChildRules(c => c
                    .RuleFor(c => c.Name).NotNull().NotEmpty().WithMessage("Must not be empty."));
        }
    }
}