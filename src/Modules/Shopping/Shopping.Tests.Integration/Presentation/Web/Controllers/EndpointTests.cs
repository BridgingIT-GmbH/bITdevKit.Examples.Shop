namespace Modules.Shopping.Tests.Integration.Presentation.Web.Controllers;

using System.Security.Claims;
using BridgingIT.DevKit.Application.Storage;
using BridgingIT.DevKit.Common;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Modules.Shopping.Application;
using Modules.Shopping.Application.Integration;
using Modules.Shopping.Presentation.Web.Controllers;
using Modules.Shopping.Tests.Integration.Presentation;

[Collection(nameof(PresentationCollection))] // https://xunit.net/docs/shared-context#collection-fixture
[IntegrationTest("Infrastructure")]
[Module("Shopping")]
public class EndpointTests : IClassFixture<CustomWebApplicationFactoryFixture<Program>>
{
    private readonly CustomWebApplicationFactoryFixture<Program> fixture;
    private static readonly string identity = Guid.NewGuid().ToString();

    public EndpointTests(ITestOutputHelper output, CustomWebApplicationFactoryFixture<Program> fixture)
    {
        this.fixture = fixture
            .WithOutput(output)
            .WithServices(services => services.AddScoped(sp =>
                new FakeAuthenticationHandlerOptions()
                    .AddClaim(ClaimTypes.NameIdentifier, identity)
                    .AddClaim(ClaimTypes.Name, "Tester")
                    .AddClaim(ClaimTypes.Email, "test@acmeshop.com")
                    .AddClaims("Permission", new ShoppingPermissionSet().GetPermissions())))
            .WithFakeAuthentication(true);
    }

    [Theory]
    [InlineData("api/shopping/echo")]
    public async Task EchoGetTest(string route)
    {
        // arrang/act
        this.fixture.Output.WriteLine($"Start Endpoint test for route: {route}");
        var response = await this.fixture.CreateClient().GetAsync(route).AnyContext();
        this.fixture.Output.WriteLine($"Finish Endpoint test for route: {route} (status={(int)response.StatusCode})");

        // assert
        response.Should().Be200Ok(); // https://github.com/adrianiftode/FluentAssertions.Web
        response.Should().MatchInContent("*echo*");
    }

    [Theory]
    [InlineData("api/shopping/carts/{{identity}}")]
    public async Task CartGetByIdentityTest(string route)
    {
        // arrang/act
        route = route.Replace("{{identity}}", identity);
        this.fixture.Output.WriteLine($"Start Endpoint test for route: {route} {identity}");
        var response = await this.fixture.CreateClient().GetAsync(route).AnyContext();
        this.fixture.Output.WriteLine($"Finish Endpoint test for route: {route} (status={(int)response.StatusCode})");

        // assert
        response.Should().Be200Ok(); // https://github.com/adrianiftode/FluentAssertions.Web
        response.Should().Satisfy<ResultOfCartDto>(
            dto =>
            {
                dto.ShouldNotBeNull();
                dto.IsSuccess.ShouldBeTrue();
                dto.Data.ShouldNotBeNull();
                dto.Data.Identity.ShouldBe(identity);
                //dto.Data.Items?.Count().ShouldBe(0);
            });
    }

    [Theory]
    [InlineData("api/shopping/carts/{{identity}}/items/{{sku}}?quantity=2")]
    public async Task CartAddItemTest(string route)
    {
        // arrang/act
        var ticks = DateTime.UtcNow.Ticks;
        var sku = $"SKU{ticks}";

        var documentStoreClient = this.fixture.ServiceProvider.GetRequiredService<IDocumentStoreClient<ReferenceDataProduct>>();
        await documentStoreClient.UpsertAsync(
            new DocumentKey(nameof(ReferenceDataProduct), sku),
            new ReferenceDataProduct(sku) { Price = 1.99m, Name = $"Product {ticks}" });

        route = route.Replace("{{identity}}", identity).Replace("{{sku}}", sku);
        this.fixture.Output.WriteLine($"Start Endpoint test for route: {route} {identity}");
        var response = await this.fixture.CreateClient().PutAsync(route, null).AnyContext();
        this.fixture.Output.WriteLine($"Finish Endpoint test for route: {route} (status={(int)response.StatusCode})");

        // assert
        response.Should().Be200Ok(); // https://github.com/adrianiftode/FluentAssertions.Web
        response.Should().Satisfy<ResultOfCartDto>(
            dto =>
            {
                dto.ShouldNotBeNull();
                dto.IsSuccess.ShouldBeTrue();
                dto.Data.ShouldNotBeNull();
                dto.Data.Identity.ShouldBe(identity);
                dto.Data.Items.ShouldNotBeNull();
                dto.Data.Items.Count().ShouldBeGreaterThan(0);
            });
    }
}