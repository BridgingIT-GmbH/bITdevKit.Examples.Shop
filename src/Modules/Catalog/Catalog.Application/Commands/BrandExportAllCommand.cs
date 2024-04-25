namespace Modules.Catalog.Application.Commands;

using BridgingIT.DevKit.Application.Commands;
using BridgingIT.DevKit.Common;
using FluentValidation;
using FluentValidation.Results;

public class BrandExportAllCommand : CommandRequestBase<Result<string>>
{
    public BrandExportAllCommand(string searchString = null)
    {
        this.SearchString = searchString ?? string.Empty;
    }

    public string SearchString { get; set; }

    public override ValidationResult Validate() =>
        new Validator().Validate(this);

    public class Validator : AbstractValidator<BrandExportAllCommand>
    {
        public Validator()
        {
            //this.RuleFor(c => c.TenantId).Must(id => Guid.TryParse(id, out var idOut)).WithMessage("Invalid guid.");
        }
    }
}
