namespace Modules.Catalog.Infrastructure;

using System.Text.Json.Serialization;

#pragma warning disable SA1649 // File name should match first type name

public class DummyJsonResponse
{
    [JsonPropertyName("products")]
    public IEnumerable<DummyJsonProduct> Products { get; set; }
}

public class DummyJsonProduct
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    [JsonPropertyName("title")]
    public string Title { get; set; }
    [JsonPropertyName("description")]
    public string Description { get; set; }
    [JsonPropertyName("price")]
    public decimal Price { get; set; }
    [JsonPropertyName("discountPercentage")]
    public decimal DiscountPercentage { get; set; }
    [JsonPropertyName("rating")]
    public decimal Rating { get; set; }
    [JsonPropertyName("stock")]
    public int Stock { get; set; }
    [JsonPropertyName("brand")]
    public string Brand { get; set; }
    [JsonPropertyName("category")]
    public string Category { get; set; }
    [JsonPropertyName("thumbnail")]
    public string Thumbnail { get; set; }
    [JsonPropertyName("images")]
    public string[] Images { get; set; }
}
#pragma warning restore SA1649 // File name should match first type name