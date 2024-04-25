namespace Modules.Catalog.Tests.Integration;

//using System.Security.Claims;
//using Microsoft.Extensions.DependencyInjection;
//using Modules.Catalog.Application;

[CollectionDefinition(nameof(PresentationCollection))]
public class PresentationCollection : // https://xunit.net/docs/shared-context#collection-fixture
    ICollectionFixture<CustomWebApplicationFactoryFixture<Program>>
{
}

//public static class WebApplicationFactoryFixture
//{
//    public static CustomWebApplicationFactoryFixture<Program> Setup(this CustomWebApplicationFactoryFixture<Program> fixture, ITestOutputHelper output)
//    {
//        return fixture.WithOutput(output)
//            .WithServices(services => services.AddScoped(sp =>
//                new FakeAuthenticationHandlerOptions()
//                    .AddClaim(ClaimTypes.NameIdentifier, this.identity)
//                    .AddClaim(ClaimTypes.Name, "Tester")
//                    .AddClaim(ClaimTypes.Email, "test@acmeshop.com")
//                    .AddClaims("Permission", new CatalogPermissionSet().GetPermissions())))
//            .WithFakeAuthentication(true);
//    }
//}