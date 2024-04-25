namespace Modules.Identity.Presentation;

using Mapster;

public class IdentityMapperRegister : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        //config.ForType<Result<TokenModel>, ResultOfTokenResponseModel>()
        //    .Map(dest => dest.Value.Status, src => src.Value.Status.Name);
    }
}