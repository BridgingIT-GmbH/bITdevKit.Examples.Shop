namespace Modules.Inventory.Application;

using BridgingIT.DevKit.Application.Queries;
using BridgingIT.DevKit.Common;
using FluentValidation;
using FluentValidation.Results;
using Modules.Inventory.Domain;

public class StockFindAllQuery : QueryRequestBase<PagedResult<Stock>>
{
    public StockFindAllQuery(int pageNumber = 1, int pageSize = 10)
    {
        this.PageNumber = pageNumber <= 0 ? 1 : pageNumber;
        this.PageSize = pageSize <= 0 ? 10 : pageSize;
    }

    public int PageNumber { get; set; }

    public int PageSize { get; set; }

    public override ValidationResult Validate() =>
        new Validator().Validate(this);

    public class Validator : AbstractValidator<StockFindAllQuery>
    {
        public Validator()
        {
            this.RuleFor(x => x.PageNumber).GreaterThanOrEqualTo(1);
            this.RuleFor(x => x.PageSize).GreaterThanOrEqualTo(10);
            //this.RuleFor(c => c.TenantId).Must(id => Guid.TryParse(id, out var idOut)).WithMessage("Invalid guid.");
        }
    }
}