namespace Modules.Inventory.Application;

using System;
using BridgingIT.DevKit.Application.Commands;
using BridgingIT.DevKit.Common;
using FluentValidation;
using FluentValidation.Results;
using Modules.Inventory.Domain;

public class StockCreateCommand : CommandRequestBase<Result<EntityCreatedCommandResult>>
{
    public StockCreateCommand(Stock entity, string identity = null)
    {
        this.Entity = entity;
        this.Identity = identity;
    }

    public Stock Entity { get; }

    public string Identity { get; }

    public override ValidationResult Validate() =>
        new Validator().Validate(this);

    public class Validator : AbstractValidator<StockCreateCommand>
    {
        public Validator()
        {
            this.RuleFor(c => c.Entity).NotNull().NotEmpty().ChildRules(c =>
            {
                c.RuleFor(c => c.Id).Must(id => id == Guid.Empty).WithMessage("Invalid guid.");
                c.RuleFor(c => c.SKU).NotNull().NotEmpty().WithMessage("Must not be empty.");
            });
        }
    }
}
