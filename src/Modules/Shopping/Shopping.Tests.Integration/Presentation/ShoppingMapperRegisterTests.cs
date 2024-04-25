namespace Modules.Shopping.Tests.Integration.Presentation;

using MapsterMapper;
using Modules.Shopping.Domain;
using Modules.Shopping.Presentation;
using Modules.Shopping.Presentation.Web.Controllers;

public class ShoppingMapperRegisterTests // TODO: move to UnitTests project
{
    private readonly Mapper sut;

    public ShoppingMapperRegisterTests()
    {
        var mapperConfig = new Mapster.TypeAdapterConfig();
        mapperConfig.Scan(typeof(ShoppingMapperRegister).Assembly);
        //mapperConfig.Apply(new[] { typeof(ShoppingMapperRegister) });

        this.sut = new Mapper(mapperConfig);
    }

    [Fact]
    public void Mapper_EntityToDto_MapsAccordingToProfile()
    {
        // arrange
        var entity = Cart.ForUser("f5d87eba-c374-45db-a5e4-7b43fd25e7bf");
        entity.AddItem(CartProduct.For("SKU0123456789", "Product X", 3.99m), 1);
        entity.AddItem(CartProduct.For("SKU9876543210", "Product Y", 2.99m), 3);
        entity.AddItem(CartProduct.For("SKU0123456789", "Product X", 3.99m), 1);

        // act
        var dto = this.sut.Map<CartDto>(entity);

        // assert
        dto.ShouldNotBeNull();
        dto.Identity.ShouldNotBeNull();
        dto.Identity.ShouldBe(entity.Identity);
        dto.TotalPrice.ShouldBe<decimal>(entity.TotalPrice);
        dto.TotalPrice.ShouldBe((2 * 3.99m) + (3 * 2.99m));
        dto.Items.ShouldNotBeNull();
        dto.Items.Count().ShouldBe(entity.Items.Count);
    }

    [Fact]
    public void Mapper_DtoToEntity_MapsAccordingToProfile()
    {
        // arrange
        var dto = new CartDto
        {
            Identity = "f5d87eba-c374-45db-a5e4-7b43fd25e7bf",
            Items = new List<CartItemDto>
            {
                new CartItemDto
                {
                    Sku = "SKU0123456789",
                    Name = "Product X",
                    UnitPrice = 3.99m,
                    Quantity = 1
                },
                new CartItemDto
                {
                    Sku = "SKU9876543210",
                    Name = "Product Y",
                    UnitPrice = 2.99m,
                    Quantity = 3
                },
                new CartItemDto
                {
                    Sku = "SKU0123456789",
                    Name = "Product X",
                    UnitPrice = 3.99m,
                    Quantity = 1
                }
            }
        };

        var entity = this.sut.Map<Cart>(dto);

        // assert
        entity.ShouldNotBeNull();
        entity.Identity.ShouldNotBeNull();
        entity.Identity.ShouldBe(entity.Identity);
        entity.TotalPrice.Amount.ShouldBe<decimal>(entity.TotalPrice);
        entity.TotalPrice.Amount.ShouldBe((2 * 3.99m) + (3 * 2.99m));
        entity.Items.ShouldNotBeNull();
        entity.Items.Count().ShouldBe(entity.Items.Count);
    }
}