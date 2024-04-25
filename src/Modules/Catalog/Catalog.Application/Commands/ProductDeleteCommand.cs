namespace Modules.Catalog.Application.Commands;

using BridgingIT.DevKit.Application.Entities;
using Modules.Catalog.Domain;

[GeneratedControllerApi]
public class ProductDeleteCommand : EntityDeleteCommandBase<Product>
{
    public ProductDeleteCommand(string id, string identity = null)
        : base(id, identity)
    {
    }
}