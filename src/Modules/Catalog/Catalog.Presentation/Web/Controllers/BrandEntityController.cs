namespace Modules.Catalog.Presentation.Web.Controllers;

using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Modules.Catalog.Application.Commands;
using Modules.Catalog.Application;
using NSwag.Annotations;
using BridgingIT.DevKit.Common;

public partial class BrandEntityController : ControllerBase
{
    [HttpGet("export")]
    [Authorize(Policy = CatalogPermissionSet.Brands.Export)]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    [OpenApiTag("catalog/brands")]
    public async Task<ActionResult<Result<string>>> Export(string searchString = "")
    {
        return new OkObjectResult((await this.requestMediator.Send(
            new BrandExportAllCommand(searchString)).AnyContext()).Result);
    }

    [HttpPost("import")]
    [Authorize(Policy = CatalogPermissionSet.Brands.Import)]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    [OpenApiTag("catalog/brands")]
    public async Task<ActionResult<Result<int>>> Import(UploadRequestModel model)
    {
        return new OkObjectResult((await this.requestMediator.Send(
            new BrandImportAllCommand(model.Data, this.HttpContext.User?.Identity?.Name)).AnyContext()).Result);
    }
}