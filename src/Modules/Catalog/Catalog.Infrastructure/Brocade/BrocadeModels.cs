namespace Modules.Catalog.Infrastructure;

using System.Text.Json.Serialization;

#pragma warning disable SA1649 // File name should match first type name
public class BrocadeProduct
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("gtin14")]
    public string Gtin14 { get; set; }

    [JsonPropertyName("size")]
    public string Size { get; set; }

    [JsonPropertyName("brand_name")]
    public string BrandName { get; set; }
}
#pragma warning restore SA1649 // File name should match first type name
