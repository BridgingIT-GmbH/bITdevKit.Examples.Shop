namespace Modules.Shopping.Application;

using FluentValidation;

public class ShoppingModuleConfiguration
{
    public IDictionary<string, string> ConnectionStrings { get; set; }

    public class Validator : AbstractValidator<ShoppingModuleConfiguration>
    {
    }
}