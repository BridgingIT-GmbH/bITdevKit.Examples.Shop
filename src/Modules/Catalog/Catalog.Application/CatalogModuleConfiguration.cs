namespace Modules.Catalog.Application;

using FluentValidation;

public class CatalogModuleConfiguration
{
    public string BrocadeApiKey { get; set; }

    public string BrocadeUrl { get; set; }

    public string DummyJsonProductsUrl { get; set; }

    public IDictionary<string, string> ConnectionStrings { get; set; }

    public class Validator : AbstractValidator<CatalogModuleConfiguration>
    {
        public Validator()
        {
            this.RuleFor(c => c.BrocadeApiKey)
                .NotNull().NotEmpty()
                .WithMessage($"{nameof(BrocadeApiKey)} cannot be null or empty");
            this.RuleFor(c => c.BrocadeUrl)
                .NotNull().NotEmpty()
                .WithMessage($"{nameof(BrocadeUrl)} cannot be null or empty");
            this.RuleFor(c => c.DummyJsonProductsUrl)
                .NotNull().NotEmpty()
                .WithMessage($"{nameof(DummyJsonProductsUrl)} cannot be null or empty");
        }
    }
}