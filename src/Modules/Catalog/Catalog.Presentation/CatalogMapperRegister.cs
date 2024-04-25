namespace Modules.Catalog.Presentation;

using BridgingIT.DevKit.Common;
using Mapster;
using Modules.Catalog.Presentation.Web.Controllers;

public class CatalogMapperRegister : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.ForType<Result<Domain.Product>, ResultOfProductResponseModel>()
            .Map(dest => dest.Value.Sku, src => src.Value.Sku);
    }
}