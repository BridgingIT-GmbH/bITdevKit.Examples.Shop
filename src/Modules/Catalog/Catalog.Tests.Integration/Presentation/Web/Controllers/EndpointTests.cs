namespace Modules.Catalog.Tests.Integration.Presentation.Web.Controllers;

using System.Net.Http;
using System.Security.Claims;
using BridgingIT.DevKit.Common;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Modules.Catalog.Application;
using Modules.Catalog.Domain;

[Collection(nameof(PresentationCollection))] // https://xunit.net/docs/shared-context#collection-fixture
[IntegrationTest("Presentation")]
[Module("Catalog")]
public class EndpointTests : IClassFixture<CustomWebApplicationFactoryFixture<Program>> // https://xunit.net/docs/shared-context#class-fixture
{
    private readonly CustomWebApplicationFactoryFixture<Program> fixture;
    private readonly string identity = Guid.NewGuid().ToString();

    public EndpointTests(ITestOutputHelper output, CustomWebApplicationFactoryFixture<Program> fixture)
    {
        this.fixture = fixture
            .WithOutput(output)
            .WithServices(services => services.AddScoped(sp =>
                new FakeAuthenticationHandlerOptions()
                    .AddClaim(ClaimTypes.NameIdentifier, this.identity)
                    .AddClaim(ClaimTypes.Name, "Tester")
                    .AddClaim(ClaimTypes.Email, "test@acmeshop.com")
                    .AddClaims("Permission", new CatalogPermissionSet().GetPermissions())))
            .WithFakeAuthentication(true);
    }

    [Theory]
    [InlineData("api/catalog/brands")]
    public async Task BrandsGetTest(string route)
    {
        // arrang/act
        var response = await this.fixture.CreateClient().GetAsync(route).AnyContext();
        var result = await response.Content?.ReadAsAsync<PagedResult<Brand>>();

        // asert
        response.Should().Be200Ok(); // https://github.com/adrianiftode/FluentAssertions.Web
        result.Should().NotBeNull();
        result.Value.Should().NotBeNull();
        result.Value.Should().HaveCountGreaterThan(0);
    }

    [Theory]
    [InlineData("api/catalog/products")]
    public async Task ProductsGetTest(string route)
    {
        // arrang/act
        var response = await this.fixture.CreateClient().GetAsync(route).AnyContext();
        var result = await response.Content?.ReadAsAsync<PagedResult<Product>>();

        // asert
        response.Should().Be200Ok(); // https://github.com/adrianiftode/FluentAssertions.Web
        result.Should().NotBeNull();
        result.Value.Should().NotBeNull();
        result.Value.Should().HaveCountGreaterThan(0);
    }
}
