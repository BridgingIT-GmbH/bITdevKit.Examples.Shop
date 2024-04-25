namespace Modules.Shopping.Presentation.Web.Controllers;

using System.Security.Claims;
using System.Threading.Tasks;
using BridgingIT.DevKit.Common;
using MediatR;
//using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Modules.Shopping.Application;
using Modules.Shopping.Application.Queries;
using Modules.Shopping.Domain;
using NSwag.Annotations;

[ApiController]
public class ShoppingController : ShoppingControllerBase
{
    private readonly IMediator mediator;
    private readonly IMapper mapper;

    public ShoppingController(IMediator mediator, IMapper mapper)
    {
        EnsureArg.IsNotNull(mediator, nameof(mediator));
        EnsureArg.IsNotNull(mapper, nameof(mapper));

        this.mediator = mediator;
        this.mapper = mapper;
    }

    [OpenApiTag("shopping")]
    public override async Task<ActionResult<ResultResponseModel>> EchoGet()
    {
        await Task.Delay(1);
        return new OkObjectResult(
            new ResultResponseModel() { IsSuccess = true, Messages = new List<string>() { "echo" } });
    }

    //[Authorize(Policy = ShoppingPermissionSet.Carts.View)]
    [OpenApiTag("shopping/carts")]
    public override async Task<ActionResult<ResultOfCartDto>> CartGetByIdentity(string identity)
    {
        var response = await this.mediator.Send(
            new CartFindOneQuery(this.HttpContext.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value)).AnyContext();

        return new OkObjectResult(
            new ResultOfCartDto
            {
                IsSuccess = response.Result.IsSuccess,
                Messages = response.Result.Messages?.ToList(),
                Data = this.mapper.Map<Cart, CartDto>(response.Result.Value)
            });
    }

    //[Authorize(Policy = ShoppingPermissionSet.Carts.Edit)]
    [OpenApiTag("shopping/carts")]
    public override async Task<ActionResult<ResultOfCartDto>> CartAddItem(string identity, string sku, [FromQuery] int quantity)
    {
        var response = await this.mediator.Send(
            new CartAddItemCommand(this.HttpContext.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value, sku, quantity)).AnyContext();
        //this.Response.Headers.Add("Location", $"/api/shopping/carts/{this.HttpContext.User?.Identity?.Name}");
#pragma warning disable ASP0019 // Suggest using IHeaderDictionary.Append or the indexer
        this.Response.Headers.Add("X-Entity-Id", response.Result.Value.Id.ToString());
#pragma warning restore ASP0019 // Suggest using IHeaderDictionary.Append or the indexer
                               //this.Response.StatusCode = (int)HttpStatusCode.Created;
        return new OkObjectResult(
            new ResultOfCartDto
            {
                IsSuccess = response.Result.IsSuccess,
                Messages = response.Result.Messages?.ToList(),
                Data = this.mapper.Map<Cart, CartDto>(response.Result.Value)
            });
        //return this.Created(
        //    $"/api/shopping/carts/{this.HttpContext.User?.Identity?.Name}",
        //    new ResultOfCartDto
        //    {
        //        Succeeded = response.Result.IsSuccess,
        //        Messages = response.Result.Messages?.ToList(),
        //        Data = this.mapper.Map(response.Result.Data)
        //    });
    }

    //[Authorize(Policy = ShoppingPermissionSet.Carts.Delete)]
    [OpenApiTag("shopping/carts")]
    public override async Task<IActionResult> CartDeleteItem(string identity, string sku)
    {
        await Task.Delay(1);
        return this.NoContent();
    }
}