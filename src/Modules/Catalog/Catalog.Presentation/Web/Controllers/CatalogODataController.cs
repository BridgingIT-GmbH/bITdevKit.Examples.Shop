namespace Modules.Catalog.Presentation.Web.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Modules.Catalog.Application.Integration;
using NSwag.Annotations;

//[Authorize]
[Route("api/catalog/odata")]
[ApiController]
public class CatalogODataController : ControllerBase // https://www.learmoreseekmore.com/2023/03/demo-on-odata-v8-in-dotnet7-api-application.html
{
    private readonly IODataAdapter adapter;

    public CatalogODataController(IODataAdapter adapter)
    {
        this.adapter = adapter;
    }

    [HttpGet("products")]
    [EnableQuery]
    [OpenApiTag("catalog")]
    public IActionResult GetProducts()
    {
        return this.Ok(this.adapter.Products.AsQueryable());
    }

    [HttpGet("brands")]
    [EnableQuery]
    [OpenApiTag("catalog")]
    public IActionResult GetBrands()
    {
        return this.Ok(this.adapter.Brands.AsQueryable());
    }
}