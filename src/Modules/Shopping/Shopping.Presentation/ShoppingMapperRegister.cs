namespace Modules.Shopping.Presentation;

using BridgingIT.DevKit.Common;
using Mapster;
using Modules.Shopping.Domain;
using Modules.Shopping.Presentation.Web.Controllers;

public class ShoppingMapperRegister : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.ForType<CartId, Guid>()
            .Map(d => d, src => src.Value);

        config.ForType<CartItemId, Guid>()
            .Map(d => d, s => s.Value);

        //config.ForType<CartDto, Cart>()
        //    .ConstructUsing(s => Cart.ForUser(s.Identity));

        config.ForType<CartDto, Cart>()
            .MapWith(s => Cart.ForUser(s.Identity), true)
            .AfterMapping((s, d) =>
                s.Items.ForEach(i => d.AddItem(
                    CartProduct.For(i.Sku, i.Name, Amount.For(i.UnitPrice)), i.Quantity)));
    }
}