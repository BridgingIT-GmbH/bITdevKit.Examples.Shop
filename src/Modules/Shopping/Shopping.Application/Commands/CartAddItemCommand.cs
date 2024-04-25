namespace Modules.Shopping.Application;

using BridgingIT.DevKit.Application.Commands;
using BridgingIT.DevKit.Common;
using FluentValidation;
using FluentValidation.Results;
using Modules.Shopping.Domain;

public class CartAddItemCommand : CommandRequestBase<Result<Cart>>
{
    public CartAddItemCommand(string identity, string sku, int quantity)
    {
        this.SKU = sku;
        this.Quantity = quantity;
        this.Identity = identity;
    }

    public string Identity { get; }

    public string SKU { get; }

    public int Quantity { get; }

    public override ValidationResult Validate() =>
        new Validator().Validate(this);

    public class Validator : AbstractValidator<CartAddItemCommand>
    {
        public Validator()
        {
            this.RuleFor(c => c.Identity).NotNull().NotEmpty().WithMessage("Must not be empty.");
            this.RuleFor(c => c.SKU).NotNull().NotEmpty().WithMessage("Must not be empty.");
            this.RuleFor(c => c.Quantity).InclusiveBetween(1, 99).WithMessage("Must be between 1 and 99.");
        }
    }
}