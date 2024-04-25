namespace Modules.Ordering.Presentation.Web.Controllers;

using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;

[ApiController]
public class OrderingController : OrderingControllerBase
{
    // private readonly IMediator mediator;

    // public OrderingController(IMediator mediator)
    // {
    //     this.mediator = mediator;
    // }

    [OpenApiTag("ordering")]
    public override async Task<ActionResult<ResultResponseModel>> EchoGet()
    {
        await Task.Delay(1);
        return new OkObjectResult(
            new ResultResponseModel() { IsSuccess = true, Messages = new List<string>() { "echo" } });
    }
}