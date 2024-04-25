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
//[Route("api/catalog/brands")]
//[ApiController]
//public class BrandController : ControllerBase
//{
//    private readonly IMediator mediator;

//    public BrandController(IMediator mediator)
//    {
//        this.mediator = mediator;
//    }

//    [HttpGet]
//    [Authorize(Policy = CatalogPermissionSet.Brands.View)]
//    [ProducesResponseType((int)HttpStatusCode.OK)]
//    [ProducesResponseType((int)HttpStatusCode.NotFound)]
//    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
//    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
//    [OpenApiTag("catalog/brands")]
//    public async Task<ActionResult<PagedResult<Brand>>> GetAll(
//        int pageNumber, int pageSize, string searchString = null, string orderBy = null, bool includeDeleted = false)
//    {
//        var query = new BrandFindAllQuery(pageNumber, pageSize, searchString, orderBy)
//        {
//            IncludeDeleted = includeDeleted
//        };
//        var result = (await this.mediator.Send(query).AnyContext()).Result;
//        return result.ToOkActionResult();

//        //return new OkObjectResult((await this.mediator.Send(
//        //    new BrandFindAllQuery(pageNumber, pageSize, searchString, orderBy)
//        //    {
//        //        IncludeDeleted = includeDeleted
//        //    }).AnyContext()).Result);
//    }

//    [HttpGet("{id}")]
//    [Authorize(Policy = CatalogPermissionSet.Brands.View)]
//    [ProducesResponseType((int)HttpStatusCode.OK)]
//    [ProducesResponseType((int)HttpStatusCode.NotFound)]
//    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
//    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
//    [OpenApiTag("catalog/brands")]
//    public async Task<ActionResult<Result<Brand>>> GetById(string id)
//    {
//        var response = await this.mediator.Send(
//            new BrandFindOneQuery(id)).AnyContext();

//        return response.Result == null
//            ? this.NotFound() : this.Ok(response.Result);
//    }

//    [HttpPost]
//    [Authorize(Policy = CatalogPermissionSet.Brands.Create)]
//    [ProducesResponseType((int)HttpStatusCode.Created)]
//    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
//    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
//    [OpenApiTag("catalog/brands")]
//    public async Task<ActionResult<Result<EntityCreatedCommandResult>>> Post(Brand entity)
//    {
//        var response = await this.mediator.Send(
//            new BrandCreateCommand(entity, this.HttpContext.User?.Identity?.Name)).AnyContext();
//        //this.Response.Headers.Add("Location", $"/api/catalog/brand/{response.Result.Data.EntityId}");
//        this.Response.Headers.Add("X-Entity-Id", response.Result.Value.EntityId);
//        //this.Response.StatusCode = (int)HttpStatusCode.Created;
//        //return this.Created($"/api/catalog/brand/{response.Result.Data.EntityId}", response.Result);
//        return new CreatedAtActionResult(
//            nameof(GetById),
//            nameof(ProductController).Replace("Controller", string.Empty),
//            new { id = response.Result.Value.EntityId },
//            response.Result);
//    }

//    [HttpPut("{id}")]
//    [Authorize(Policy = CatalogPermissionSet.Brands.Edit)]
//    [ProducesResponseType((int)HttpStatusCode.OK)]
//    [ProducesResponseType((int)HttpStatusCode.Created)]
//    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
//    [ProducesResponseType((int)HttpStatusCode.NotFound)]
//    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
//    [OpenApiTag("catalog/brands")]
//    public async Task<ActionResult<Result<EntityUpdatedCommandResult>>> Put(string id, Brand entity)
//    {
//        if (id != entity?.Id.ToString())
//        {
//            this.Response.StatusCode = (int)HttpStatusCode.BadRequest;
//            return null;
//        }

//        var response = await this.mediator.Send(
//            new BrandUpdateCommand(entity, this.HttpContext.User?.Identity?.Name)).AnyContext();
//        //this.Response.Headers.Add("Location", $"/api/catalog/brand/{response.Result.Data.EntityId}");
//        this.Response.Headers.Add("X-Entity-Id", response.Result.Value.EntityId);
//        //this.Response.StatusCode = (int)HttpStatusCode.OK;
//        //return this.Ok(response.Result);
//        return new OkObjectResult(response.Result);
//    }

//    [HttpDelete("{id}")]
//    [Authorize(Policy = CatalogPermissionSet.Brands.Delete)]
//    [ProducesResponseType((int)HttpStatusCode.OK)]
//    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
//    [ProducesResponseType((int)HttpStatusCode.NotFound)]
//    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
//    [OpenApiTag("catalog/brands")]
//    public async Task<ActionResult<Result<EntityDeletedCommandResult>>> Delete(string id)
//    {
//        var response = await this.mediator.Send(
//            new BrandDeleteCommand(id, this.HttpContext.User?.Identity?.Name)).AnyContext();
//        this.Response.Headers.Add("X-Entity-Id", response.Result.Value.EntityId);
//        //this.Response.StatusCode = (int)HttpStatusCode.OK;
//        return new OkObjectResult(response.Result);
//    }

//    [HttpGet("export")]
//    [Authorize(Policy = CatalogPermissionSet.Brands.Export)]
//    [ProducesResponseType((int)HttpStatusCode.OK)]
//    [ProducesResponseType((int)HttpStatusCode.NotFound)]
//    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
//    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
//    [OpenApiTag("catalog/brands")]
//    public async Task<ActionResult<Result<string>>> Export(string searchString = "")
//    {
//        return new OkObjectResult((await this.mediator.Send(
//            new BrandExportAllCommand(searchString)).AnyContext()).Result);
//    }

//    [HttpPost("import")]
//    [Authorize(Policy = CatalogPermissionSet.Brands.Import)]
//    [ProducesResponseType((int)HttpStatusCode.OK)]
//    [ProducesResponseType((int)HttpStatusCode.NotFound)]
//    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
//    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
//    [OpenApiTag("catalog/brands")]
//    public async Task<ActionResult<Result<int>>> Import(UploadRequestModel model)
//    {
//        return new OkObjectResult((await this.mediator.Send(
//            new BrandImportAllCommand(model.Data, this.HttpContext.User?.Identity?.Name)).AnyContext()).Result);
//    }
//}
