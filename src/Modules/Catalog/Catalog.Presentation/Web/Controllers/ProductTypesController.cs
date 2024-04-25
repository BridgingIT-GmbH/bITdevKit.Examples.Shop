//namespace Modules.Catalog.Presentation.Web.Controllers;

//using System.Net;
//using BridgingIT.DevKit.Common;
//using MediatR;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using Modules.Catalog.Application;
//using Modules.Catalog.Application.Queries;
//using Modules.Catalog.Domain;
//using NSwag.Annotations;

//[Authorize]
//[Route("api/catalog/producttypes")]
//[ApiController]
//public class ProductTypesController : ControllerBase
//{
//    private readonly IMediator mediator;

//    public ProductTypesController(IMediator mediator)
//    {
//        this.mediator = mediator;
//    }

//    [HttpGet]
//    [Authorize(Policy = CatalogPermissionSet.Products.View)]
//    [ProducesResponseType((int)HttpStatusCode.OK)]
//    [ProducesResponseType((int)HttpStatusCode.NotFound)]
//    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
//    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
//    [OpenApiTag("catalog/producttypes")]
//    public async Task<ActionResult<PagedResult<ProductType>>> GetAll(int pageNumber, int pageSize, string searchString = null, string orderBy = null)
//    {
//        return new OkObjectResult((await this.mediator.Send(
//            new ProductTypeFindAllQuery(pageNumber, pageSize, searchString, orderBy)).AnyContext()).Result);
//    }
//}