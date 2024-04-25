//namespace Modules.Catalog.Presentation.Web.Controllers;

//using System.Net;
//using BridgingIT.DevKit.Application.Commands;
//using BridgingIT.DevKit.Common;
//using BridgingIT.DevKit.Presentation.Web;
//using MediatR;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using Modules.Catalog.Application;
//using Modules.Catalog.Application.Commands;
//using Modules.Catalog.Application.Queries;
//using Modules.Catalog.Domain;
//using NSwag.Annotations;

//[Authorize]
//[Route("api/catalog/products")]
//[ApiController]
//public class ProductController : ControllerBase
//{
//    private readonly IMediator mediator;

//    public ProductController(IMediator mediator)
//    {
//        this.mediator = mediator;
//    }

//    [HttpGet]
//    [Authorize(Policy = CatalogPermissionSet.Products.View)]
//    [ProducesResponseType((int)HttpStatusCode.OK)]
//    [ProducesResponseType((int)HttpStatusCode.NotFound)]
//    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
//    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
//    [OpenApiTag("catalog/products")]
//    public async Task<ActionResult<PagedResult<Product>>> GetAll(int pageNumber, int pageSize, string searchString = null, string orderBy = null)
//    {
//        var query = new ProductFindAllQuery(pageNumber, pageSize, searchString, orderBy);
//        var result = (await this.mediator.Send(query).AnyContext()).Result;

//        return result.ToOkActionResult();

//        //return new OkObjectResult((await this.mediator.Send(
//        //    new ProductFindAllQuery(pageNumber, pageSize, searchString, orderBy)).AnyContext()).Result);
//    }

//    [HttpGet("{id}")]
//    [Authorize(Policy = CatalogPermissionSet.Products.View)]
//    [ProducesResponseType((int)HttpStatusCode.OK)]
//    [ProducesResponseType((int)HttpStatusCode.NotFound)]
//    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
//    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
//    [OpenApiTag("catalog/products")]
//    public async Task<ActionResult<Result<Product>>> GetById(string id)
//    {
//        var response = await this.mediator.Send(
//            new ProductFindOneQuery(id)).AnyContext();

//        return response.Result == null
//            ? this.NotFound() : this.Ok(response.Result);
//    }

//    [HttpPost]
//    [Authorize(Policy = CatalogPermissionSet.Products.Create)]
//    [ProducesResponseType((int)HttpStatusCode.Created)]
//    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
//    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
//    [OpenApiTag("catalog/products")]
//    public async Task<ActionResult<Result<EntityCreatedCommandResult>>> Post(Product entity)
//    {
//        var response = await this.mediator.Send(
//            new ProductCreateCommand(entity, this.HttpContext.User?.Identity?.Name)).AnyContext();
//        //this.Response.Headers.Add("Location", $"/api/catalog/product/{response.Result.Data.EntityId}");
//        this.Response.Headers.Add("X-Entity-Id", response.Result.Value.EntityId);
//        //this.Response.StatusCode = (int)HttpStatusCode.Created;
//        //return this.Created($"/api/catalog/product/{response.Result.Data.EntityId}", response.Result);
//        return new CreatedAtActionResult(
//            nameof(GetById),
//            nameof(ProductController).Replace("Controller", string.Empty),
//            new { id = response.Result.Value.EntityId },
//            response.Result);
//    }

//    [HttpPut("{id}")]
//    [Authorize(Policy = CatalogPermissionSet.Products.Edit)]
//    [ProducesResponseType((int)HttpStatusCode.OK)]
//    [ProducesResponseType((int)HttpStatusCode.Created)]
//    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
//    [ProducesResponseType((int)HttpStatusCode.NotFound)]
//    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
//    [OpenApiTag("catalog/products")]
//    public async Task<ActionResult<Result<EntityUpdatedCommandResult>>> Put(string id, Product entity)
//    {
//        if (id != entity?.Id.ToString())
//        {
//            this.Response.StatusCode = (int)HttpStatusCode.BadRequest;
//            return null;
//        }

//        var response = await this.mediator.Send(
//            new ProductUpdateCommand(entity, this.HttpContext.User?.Identity?.Name)).AnyContext();
//        //this.Response.Headers.Add("Location", $"/api/catalog/product/{response.Result.Data.EntityId}");
//        this.Response.Headers.Add("X-Entity-Id", response.Result.Value.EntityId);
//        //this.Response.StatusCode = (int)HttpStatusCode.OK;
//        //return this.Ok(response.Result);
//        return new OkObjectResult(response.Result);
//    }

//    [HttpDelete("{id}")]
//    [Authorize(Policy = CatalogPermissionSet.Products.Delete)]
//    [ProducesResponseType((int)HttpStatusCode.OK)]
//    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
//    [ProducesResponseType((int)HttpStatusCode.NotFound)]
//    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
//    [OpenApiTag("catalog/products")]
//    public async Task<ActionResult<Result<EntityDeletedCommandResult>>> Delete(string id)
//    {
//        var response = await this.mediator.Send(
//            new ProductDeleteCommand(id, this.HttpContext.User?.Identity?.Name)).AnyContext();
//        this.Response.Headers.Add("X-Entity-Id", response.Result.Value.EntityId);
//        //this.Response.StatusCode = (int)HttpStatusCode.OK;
//        return new OkObjectResult(response.Result);
//    }

//    [HttpGet("export")]
//    [Authorize(Policy = CatalogPermissionSet.Products.Export)]
//    [ProducesResponseType((int)HttpStatusCode.OK)]
//    [ProducesResponseType((int)HttpStatusCode.NotFound)]
//    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
//    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
//    [OpenApiTag("catalog/products")]
//    public async Task<ActionResult<Result<string>>> Export(string searchString = "")
//    {
//        return new OkObjectResult((await this.mediator.Send(
//            new ProductExportAllCommand(searchString)).AnyContext()).Result);
//    }

//    [HttpPost("import")]
//    [Authorize(Policy = CatalogPermissionSet.Products.Import)]
//    [ProducesResponseType((int)HttpStatusCode.OK)]
//    [ProducesResponseType((int)HttpStatusCode.NotFound)]
//    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
//    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
//    [OpenApiTag("catalog/brands")]
//    public async Task<ActionResult<Result<int>>> Import(UploadRequestModel model)
//    {
//        return new OkObjectResult((await this.mediator.Send(
//            new ProductImportAllCommand(model.Data, this.HttpContext.User?.Identity?.Name)).AnyContext()).Result);
//    }
//}
