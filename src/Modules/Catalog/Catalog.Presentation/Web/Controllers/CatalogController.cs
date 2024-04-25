namespace Modules.Catalog.Presentation.Web.Controllers;

using System.Net;
using System.Threading.Tasks;
using BridgingIT.DevKit.Application.Commands;
using BridgingIT.DevKit.Common;
using BridgingIT.DevKit.Presentation.Web;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Modules.Catalog.Application;
using Modules.Catalog.Application.Commands;
using Modules.Catalog.Application.Queries;
using NSwag.Annotations;

[ApiController]
public class CatalogController : CatalogControllerBase
{
    private readonly IMediator mediator;
    private readonly IMapper mapper;

    public CatalogController(IMediator mediator, IMapper mapper)
    {
        this.mediator = mediator;
        this.mapper = mapper;
    }

    [OpenApiTag("catalog")]
    public override async Task<ActionResult<ResultResponseModel>> EchoGet()
    {
        await Task.Delay(1);
        return new OkObjectResult(
            new ResultResponseModel() { IsSuccess = true, Messages = new List<string>() { "echo" } });
    }

    // Brands ======================================================================================
    [Authorize(Policy = CatalogPermissionSet.Brands.View)]
    public override async Task<ActionResult<PagedResultOfBrandsResponseModel>> BrandGetAll([FromQuery] int? pageNumber, [FromQuery] int? pageSize, [FromQuery] string searchString, [FromQuery] string orderBy, [FromQuery] bool? includeDeleted = false)
    {
        var query = new BrandFindAllQuery(pageNumber ?? 1, pageSize ?? int.MaxValue, searchString, orderBy)
        {
            IncludeDeleted = includeDeleted ?? false
        };
        var result = (await this.mediator.Send(query).AnyContext()).Result;

        return result.ToOkActionResult<PagedResultOfBrandsResponseModel>(this.mapper);
    }

    [Authorize(Policy = CatalogPermissionSet.Brands.View)]
    public override async Task<ActionResult<ResultOfBrandResponseModel>> BrandGetById(string id)
    {
        var result = (await this.mediator.Send(
            new BrandFindOneQuery(id)).AnyContext()).Result;

        return result.ToOkActionResult<ResultOfBrandResponseModel>(this.mapper);
    }

    [Authorize(Policy = CatalogPermissionSet.Brands.Create)]
    public override async Task<ActionResult<ResultOfEntityCreatedResponseModel>> BrandPost([FromBody] BrandModel model)
    {
        var result = (await this.mediator.Send(
            new BrandCreateCommand(this.mapper.Map<BrandModel, Domain.Brand>(model), this.HttpContext.User?.Identity?.Name)).AnyContext()).Result;

#pragma warning disable ASP0019 // Suggest using IHeaderDictionary.Append or the indexer
        this.Response.Headers.Add("X-Entity-Id", result.Value.EntityId); // TODO: really needed?
#pragma warning restore ASP0019 // Suggest using IHeaderDictionary.Append or the indexer
        return result.ToCreatedActionResult<EntityCreatedCommandResult, ResultOfEntityCreatedResponseModel>(
            this.mapper,
            "Catalog_Brand-GetById",
            new { id = result.Value.EntityId });
        //return result.ToCreatedActionResult<EntityCreatedCommandResult, ResultResponseModel>(
        //    this.mapper,
        //    nameof(BrandGetById),
        //    nameof(CatalogController).Replace("Controller", string.Empty),
        //    new { id = result.Value.EntityId });

        //return new CreatedAtActionResult(
        //    nameof(BrandGetById),
        //    nameof(CatalogController).Replace("Controller", string.Empty),
        //    new { id = response.Result.Value.EntityId },
        //    response.Result);
    }

    [Authorize(Policy = CatalogPermissionSet.Brands.Edit)]
    public override async Task<ActionResult<ResultOfEntityUpdatedResponseModel>> BrandPut(string id, [FromBody] BrandModel model)
    {
        if (id != model?.Id.ToString())
        {
            this.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return null;
        }

        var result = (await this.mediator.Send(
            new BrandUpdateCommand(this.mapper.Map<BrandModel, Domain.Brand>(model), this.HttpContext.User?.Identity?.Name)).AnyContext()).Result;

#pragma warning disable ASP0019 // Suggest using IHeaderDictionary.Append or the indexer
        this.Response.Headers.Add("X-Entity-Id", result.Value.EntityId);
#pragma warning restore ASP0019 // Suggest using IHeaderDictionary.Append or the indexer
        return result.ToUpdatedActionResult<EntityUpdatedCommandResult, ResultOfEntityUpdatedResponseModel>(this.mapper);
    }

    [Authorize(Policy = CatalogPermissionSet.Brands.Delete)]
    public override async Task<ActionResult<ResultOfEntityDeletedResponseModel>> BrandDelete(string id)
    {
        var result = (await this.mediator.Send(
            new BrandDeleteCommand(id, this.HttpContext.User?.Identity?.Name)).AnyContext()).Result;
#pragma warning disable ASP0019 // Suggest using IHeaderDictionary.Append or the indexer
        this.Response.Headers.Add("X-Entity-Id", result.Value.EntityId);
#pragma warning restore ASP0019 // Suggest using IHeaderDictionary.Append or the indexer

        return result.ToOkActionResult<EntityDeletedCommandResult, ResultOfEntityDeletedResponseModel>(this.mapper);
    }

    [Authorize(Policy = CatalogPermissionSet.Brands.Export)]
    public override async Task<ActionResult<ResultOfStringResponseModel>> BrandExport([FromQuery] string searchString = "")
    {
        var result = (await this.mediator.Send(
            new BrandExportAllCommand(searchString)).AnyContext()).Result;

        return result.ToOkActionResult<ResultOfStringResponseModel>(this.mapper);
    }

    [Authorize(Policy = CatalogPermissionSet.Brands.Import)]
    public override async Task<ActionResult<ResultOfIntegerResponseModel>> BrandImport([FromBody] UploadRequestModel model)
    {
        var result = (await this.mediator.Send(
            new BrandImportAllCommand(model.Data, this.HttpContext.User?.Identity?.Name)).AnyContext()).Result;

        return result.ToOkActionResult<ResultOfIntegerResponseModel>(this.mapper);
    }

    // Products ====================================================================================
    [Authorize(Policy = CatalogPermissionSet.Products.View)]
    public override async Task<ActionResult<PagedResultOfProductsResponseModel>> ProductGetAll([FromQuery] int? pageNumber, [FromQuery] int? pageSize, [FromQuery] string searchString, [FromQuery] string orderBy)
    {
        var query = new ProductFindAllQuery(pageNumber ?? 1, pageSize ?? int.MaxValue, searchString, orderBy);
        var result = (await this.mediator.Send(query).AnyContext()).Result;

        return result.ToOkActionResult<PagedResultOfProductsResponseModel>(this.mapper);
    }

    [Authorize(Policy = CatalogPermissionSet.Products.View)]
    public override async Task<ActionResult<ResultOfProductResponseModel>> ProductGetById(string id)
    {
        var result = (await this.mediator.Send(
            new ProductFindOneQuery(id)).AnyContext()).Result;

        return result.ToOkActionResult<ResultOfProductResponseModel>(this.mapper);
    }

    [Authorize(Policy = CatalogPermissionSet.Products.Create)]
    public override async Task<ActionResult<ResultOfEntityCreatedResponseModel>> ProductPost([FromBody] ProductModel model)
    {
        var result = (await this.mediator.Send(
            new ProductCreateCommand(this.mapper.Map<ProductModel, Domain.Product>(model), this.HttpContext.User?.Identity?.Name)).AnyContext()).Result;

#pragma warning disable ASP0019 // Suggest using IHeaderDictionary.Append or the indexer
        this.Response.Headers.Add("X-Entity-Id", result.Value.EntityId); // TODO: really needed?
#pragma warning restore ASP0019 // Suggest using IHeaderDictionary.Append or the indexer
        return result.ToCreatedActionResult<EntityCreatedCommandResult, ResultOfEntityCreatedResponseModel>(
            this.mapper,
            "Catalog_Product-GetById",
            new { id = result.Value.EntityId });
        //return result.ToCreatedActionResult<EntityCreatedCommandResult, ResultResponseModel>(
        //    this.mapper,
        //    nameof(ProductGetById),
        //    nameof(CatalogController).Replace("Controller", string.Empty),
        //    new { id = result.Value.EntityId });

        //return new CreatedAtActionResult(
        //    nameof(ProductGetById),
        //    nameof(CatalogController).Replace("Controller", string.Empty),
        //    new { id = response.Result.Value.EntityId },
        //    response.Result);
    }

    [Authorize(Policy = CatalogPermissionSet.Products.Edit)]
    public override async Task<ActionResult<ResultOfEntityUpdatedResponseModel>> ProductPut(string id, [FromBody] ProductModel model)
    {
        if (id != model?.Id.ToString())
        {
            this.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return null;
        }

        var result = (await this.mediator.Send(
            new ProductUpdateCommand(this.mapper.Map<ProductModel, Domain.Product>(model), this.HttpContext.User?.Identity?.Name)).AnyContext()).Result;

#pragma warning disable ASP0019 // Suggest using IHeaderDictionary.Append or the indexer
        this.Response.Headers.Add("X-Entity-Id", result.Value.EntityId); // TODO: really needed?
#pragma warning restore ASP0019 // Suggest using IHeaderDictionary.Append or the indexer
        return result.ToUpdatedActionResult<EntityUpdatedCommandResult, ResultOfEntityUpdatedResponseModel>(this.mapper);
    }

    public override async Task<ActionResult<ResultOfEntityDeletedResponseModel>> ProductDelete(string id)
    {
        var result = (await this.mediator.Send(
            new ProductDeleteCommand(id, this.HttpContext.User?.Identity?.Name)).AnyContext()).Result;
#pragma warning disable ASP0019 // Suggest using IHeaderDictionary.Append or the indexer
        this.Response.Headers.Add("X-Entity-Id", result.Value.EntityId); // TODO: really needed?
#pragma warning restore ASP0019 // Suggest using IHeaderDictionary.Append or the indexer

        return result.ToOkActionResult<EntityDeletedCommandResult, ResultOfEntityDeletedResponseModel>(this.mapper);
    }

    [Authorize(Policy = CatalogPermissionSet.Products.Export)]
    public override async Task<ActionResult<ResultOfStringResponseModel>> ProductExport([FromQuery] string searchString = "")
    {
        var result = (await this.mediator.Send(
            new ProductExportAllCommand(searchString)).AnyContext()).Result;

        return result.ToOkActionResult<ResultOfStringResponseModel>(this.mapper);
    }

    [Authorize(Policy = CatalogPermissionSet.Products.Import)]
    public override async Task<ActionResult<ResultOfIntegerResponseModel>> ProductImport([FromBody] UploadRequestModel model)
    {
        var result = (await this.mediator.Send(
            new ProductImportAllCommand(model.Data, this.HttpContext.User?.Identity?.Name)).AnyContext()).Result;

        return result.ToOkActionResult<ResultOfIntegerResponseModel>(this.mapper);
    }

    // ProductTypes ================================================================================
    [Authorize(Policy = CatalogPermissionSet.Products.View)]
    public override async Task<ActionResult<PagedResultOfProductTypesResponseModel>> ProductTypesGetAll([FromQuery] int? pageNumber, [FromQuery] int? pageSize, [FromQuery] string searchString, [FromQuery] string orderBy)
    {
        var query = new ProductTypeFindAllQuery(pageNumber ?? 1, pageSize ?? int.MaxValue, searchString, orderBy);

        var result = (await this.mediator.Send(query).AnyContext()).Result;

        return result.ToOkActionResult<PagedResultOfProductTypesResponseModel>(this.mapper);
    }
}