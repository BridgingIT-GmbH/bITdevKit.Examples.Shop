namespace Modules.Ordering.Application;

using FluentValidation;

public class OrderingModuleConfiguration
{
    public IDictionary<string, string> ConnectionStrings { get; set; }

    public class Validator : AbstractValidator<OrderingModuleConfiguration>
    {
    }
}