namespace Modules.Catalog.Domain;

using BridgingIT.DevKit.Domain.Model;

public class Brand : AggregateRoot<Guid>, IAuditable // TODO: use strongly typed ids https://www.youtube.com/watch?v=z4SB5BkQX7M
{
    public string Name { get; set; }

    public string Description { get; set; }

    public IEnumerable<string> Keywords { get; set; }

    public string PictureSvg { get; set; }

    public string PictureFileName { get; set; }

    public string PictureUri { get; set; }

    public AuditState AuditState { get; set; } = new AuditState();
}