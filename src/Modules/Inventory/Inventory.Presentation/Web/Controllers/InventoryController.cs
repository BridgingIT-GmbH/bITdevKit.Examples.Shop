namespace Modules.Inventory.Presentation.Web.Controllers;

using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;

[ApiController]
public class InventoryController : InventoryControllerBase
{
    private readonly IMediator mediator;

    public InventoryController(IMediator mediator)
    {
        this.mediator = mediator;
    }

    [OpenApiTag("inventory")]
    public override async Task<ActionResult<ResultResponseModel>> EchoGet()
    {
        await Task.Delay(1);
        return new OkObjectResult(
            new ResultResponseModel() { IsSuccess = true, Messages = new List<string>() { "echo" } });
    }
}