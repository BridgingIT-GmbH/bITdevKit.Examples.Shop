namespace Modules.Shopping.Domain;

using System;
using BridgingIT.DevKit.Domain.Model;

public class Tenant : AggregateRoot<TenantId>
{
    private readonly List<CartId> cartIds = new();

    private Tenant()
    {
    }

    private Tenant(string name, string description = null)
    {
        this.Name = name;
        this.Description = description;
    }

    public string Name { get; init; }

    public string Description { get; init; }

    public IReadOnlyList<CartId> CartIds => this.cartIds.AsReadOnly();

    public static Tenant For(string name, string description = null)
    {
        return new Tenant(name, description);
    }
}

public class TenantId : GuidTypedId
{
    public TenantId()
        : this(Guid.NewGuid())
    {
    }

    public TenantId(Guid value)
        : base(value)
    {
    }

    public static implicit operator TenantId(Guid id) => new(id);

    public override string ToString() => this.Value.ToString();
}