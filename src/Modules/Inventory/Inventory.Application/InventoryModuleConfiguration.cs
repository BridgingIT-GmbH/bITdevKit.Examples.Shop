namespace Modules.Inventory.Application;

using FluentValidation;

public class InventoryModuleConfiguration
{
    public IDictionary<string, string> ConnectionStrings { get; set; }

    public class Validator : AbstractValidator<InventoryModuleConfiguration>
    {
    }
}