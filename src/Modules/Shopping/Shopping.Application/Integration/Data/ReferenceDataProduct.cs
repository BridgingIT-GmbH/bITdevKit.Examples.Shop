namespace Modules.Shopping.Application.Integration;

using System.Diagnostics;

[DebuggerDisplay("SKU={SKU}")]
public class ReferenceDataProduct
{
    public ReferenceDataProduct()
    {
    }

    public ReferenceDataProduct(string sku)
    {
        EnsureArg.IsNotNullOrEmpty(sku, nameof(sku));

        this.SKU = sku;
    }

    public Guid Id { get; set; } = Guid.Empty;

    public string SKU { get; init; }

    public string Name { get; set; }

    public decimal? Price { get; set; }

    public int? Stock { get; set; }
}