namespace Modules.Catalog.Application.Commands;

using BridgingIT.DevKit.Application.Commands;
using BridgingIT.DevKit.Common;
using FluentValidation;
using FluentValidation.Results;

public class BrandImportAllCommand : CommandRequestBase<Result<int>>
{
    public BrandImportAllCommand(byte[] data, string identity = null)
    {
        this.Data = data;
        this.Identity = identity;
    }

    public byte[] Data { get; set; }

    public string Identity { get; }

    public override ValidationResult Validate() =>
        new Validator().Validate(this);

    public class Validator : AbstractValidator<BrandImportAllCommand>
    {
        public Validator()
        {
            this.RuleFor(c => c.Data).NotNull().NotEmpty();
        }
    }
}
