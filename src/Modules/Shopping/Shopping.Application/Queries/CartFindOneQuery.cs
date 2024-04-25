namespace Modules.Shopping.Application.Queries;

using BridgingIT.DevKit.Application.Queries;
using BridgingIT.DevKit.Common;
using FluentValidation;
using FluentValidation.Results;
using Modules.Shopping.Domain;

/// <summary>
/// This class is used for querying a single cart item from the domain.
/// If no cart was found a new cart will be returned.
/// </summary>
public class CartFindOneQuery : QueryRequestBase<Result<Cart>>
{
    public CartFindOneQuery(string identity)
    {
        EnsureArg.IsNotNullOrEmpty(identity, nameof(identity));

        this.Identity = identity;
    }

    public string Identity { get; }

    public override ValidationResult Validate() =>
        new Validator().Validate(this);

    public class Validator : AbstractValidator<CartFindOneQuery>
    {
        public Validator()
        {
            this.RuleFor(c => c.Identity).NotNull().NotEmpty().WithMessage("Must not be empty.");
        }
    }
}