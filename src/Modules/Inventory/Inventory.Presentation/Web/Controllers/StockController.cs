namespace Modules.Inventory.Presentation.Web.Controllers;

using System.Net;
using BridgingIT.DevKit.Application.Commands;
using BridgingIT.DevKit.Common;
using MediatR;
//using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Modules.Inventory.Application;
using Modules.Inventory.Domain;
using NSwag.Annotations;

//[Authorize]
[Route("api/inventory/stocks")]
[ApiController]
public class StockController : ControllerBase
{
    private readonly IMediator mediator;

    public StockController(IMediator mediator)
    {
        this.mediator = mediator;
    }

    [HttpGet]
    //[Authorize(Policy = PermissionSet.Stocks.View)]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    [OpenApiTag("inventory/stocks")]
    public async Task<ActionResult<PagedResult<Stock>>> GetAll(int pageNumber, int pageSize)
    {
        return new OkObjectResult((await this.mediator.Send(
            new StockFindAllQuery(pageNumber, pageSize)).AnyContext()).Result);
    }

    [HttpGet("{id}")]
    //[Authorize(Policy = PermissionSet.Stocks.View)]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    [OpenApiTag("inventory/stocks")]
    public async Task<ActionResult<Result<Stock>>> GetById(string id)
    {
        var response = (await this.mediator.Send(
            new StockFindAllQuery()).AnyContext()).Result?.Value?.FirstOrDefault(e => e.Id == Guid.Parse(id));

        return response == null
            ? this.NotFound() : new OkObjectResult(response);
    }

    [HttpPost]
    //[Authorize(Policy = PermissionSet.Stocks.Create)]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    [OpenApiTag("inventory/stocks")]
    public async Task<ActionResult<Result<EntityCreatedCommandResult>>> Post(Stock entity)
    {
        var response = await this.mediator.Send(
            new StockCreateCommand(entity, this.HttpContext.User?.Identity?.Name)).AnyContext();
        //this.Response.Headers.Add("Location", $"/api/inventory/stocks/{response.Result.Data.EntityId}");
#pragma warning disable ASP0019 // Suggest using IHeaderDictionary.Append or the indexer
        this.Response.Headers.Add("X-Entity-Id", response.Result.Value.EntityId);
#pragma warning restore ASP0019 // Suggest using IHeaderDictionary.Append or the indexer
        //this.Response.StatusCode = (int)HttpStatusCode.Created;
        //return new OkObjectResult(response.Result);
        return new CreatedAtActionResult(
            nameof(GetById),
            nameof(StockController),
            new { identity = this.HttpContext.User?.Identity?.Name },
            response.Result);
    }

    //[HttpPut]
    //[Authorize(Policy = PermissionSet.Stocks.Edit)]
    //[Route("{id}")]
    //[ProducesResponseType((int)HttpStatusCode.OK)]
    //[ProducesResponseType((int)HttpStatusCode.Created)]
    //[ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    //[ProducesResponseType((int)HttpStatusCode.NotFound)]
    //[ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    //[OpenApiTag("inventory/stocks")]
    //public async Task<ActionResult<Result<EntityUpdatedCommandResult>>> Put(string id, Stock entity)
    //{
    //    if (id != entity?.Id.ToString())
    //    {
    //        this.Response.StatusCode = (int)HttpStatusCode.BadRequest;
    //        return null;
    //    }

    //    var response = await this.mediator.Send(
    //        new StockUpdateCommand(entity, this.HttpContext.User?.Identity?.Name)).AnyContext();
    //    this.Response.Headers.Add("Location", $"/api/inventory/stocks/{response.Result.Data.EntityId}");
    //    this.Response.Headers.Add("X-Entity-Id", response.Result.Data.EntityId);
    //    this.Response.StatusCode = (int)HttpStatusCode.OK;
    //    return new OkObjectResult(response.Result);
    //}

    //[HttpDelete]
    //[Authorize(Policy = PermissionSet.Stocks.Delete)]
    //[Route("{id}")]
    //[ProducesResponseType((int)HttpStatusCode.OK)]
    //[ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    //[ProducesResponseType((int)HttpStatusCode.NotFound)]
    //[ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    //[OpenApiTag("inventory/stocks")]
    //public async Task<ActionResult<Result<EntityDeletedCommandResult>>> Delete(string id)
    //{
    //    var response = await this.mediator.Send(
    //        new StockDeleteCommand(id, this.HttpContext.User?.Identity?.Name)).AnyContext();
    //    this.Response.Headers.Add("X-Entity-Id", response.Result.Data.EntityId);
    //    this.Response.StatusCode = (int)HttpStatusCode.OK;
    //    return new OkObjectResult(response.Result);
    //}

    //[Authorize(Policy = PermissionSet.Stocks.Export)]
    //[HttpGet("export")]
    //[ProducesResponseType((int)HttpStatusCode.OK)]
    //[ProducesResponseType((int)HttpStatusCode.NotFound)]
    //[ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    //[ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    //[OpenApiTag("inventory/stocks")]
    //public async Task<ActionResult<Result<string>>> Export(string searchString = "")
    //{
    //    return new OkObjectResult((await this.mediator.Send(
    //        new StockExportAllCommand(searchString)).AnyContext()).Result);
    //}

    //[Authorize(Policy = PermissionSet.Stocks.Import)]
    //[HttpPost("import")]
    //[ProducesResponseType((int)HttpStatusCode.OK)]
    //[ProducesResponseType((int)HttpStatusCode.NotFound)]
    //[ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    //[ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    //[OpenApiTag("inventory/stocks")]
    //public async Task<ActionResult<Result<int>>> Import(UploadRequestModel model)
    //{
    //    return new OkObjectResult((await this.mediator.Send(
    //        new StockImportAllCommand(model.Data, this.HttpContext.User?.Identity?.Name)).AnyContext()).Result);
    //}
}
