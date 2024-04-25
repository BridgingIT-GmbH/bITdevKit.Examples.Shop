namespace Modules.Catalog.Domain;

using BridgingIT.DevKit.Domain.Model;

public class Product : AggregateRoot<Guid>, IAuditable // TODO: use strongly typed ids https://www.youtube.com/watch?v=z4SB5BkQX7M
{
    public string Name { get; set; }

    public string Description { get; set; }

    public IEnumerable<string> Keywords { get; set; }

    public string PictureSvg { get; set; }

    public string PictureFileName { get; set; }

    public string PictureUri { get; set; }

    public string Sku { get; set; }

    public string Barcode { get; set; }

    public string Size { get; set; }

    public int Rating { get; set; }

    public decimal Price { get; set; }

    public Guid TypeId { get; set; } // TODO: use strongly typed ids https://www.youtube.com/watch?v=z4SB5BkQX7M

    public ProductType Type { get; set; }

    public Guid BrandId { get; set; } // TODO: use strongly typed ids https://www.youtube.com/watch?v=z4SB5BkQX7M

    public Brand Brand { get; set; }

    public AuditState AuditState { get; set; } = new AuditState();
}