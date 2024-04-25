namespace Modules.Identity.Tests.Integration.Presentation.Web.Controllers;

using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using BridgingIT.DevKit.Common;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Modules.Identity.Application;
using Modules.Identity.Presentation.Web.Controllers;

[Collection(nameof(PresentationCollection))] // https://xunit.net/docs/shared-context#collection-fixture
[IntegrationTest("Presentation")]
[Module("Identity")]
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
                    .AddClaims("Permission", new IdentityPermissionSet().GetPermissions())))
            .WithFakeAuthentication(true);
    }

    [Theory]
    [InlineData("api/identity/tokens")]
    public async Task TokenAcquireTest(string route)
    {
        // arrange
        var content = new StringContent(
            JsonSerializer.Serialize(new TokenRequestModel { Email = "admin@acmeshop.com", Password = "Fidespic032" }), Encoding.UTF8, System.Net.Mime.MediaTypeNames.Application.Json);

        // act
        var response = await this.fixture.CreateClient().PostAsync(route, content).AnyContext();
        var result = await response.Content?.ReadAsAsync<ResultOfTokenResponseModel>();

        // asert
        response.Should().Be200Ok(); // https://github.com/adrianiftode/FluentAssertions.Web
        result.Should().NotBeNull();
        result.Value.Should().NotBeNull();
        result.Value.Token.Should().NotBeNull();
        result.Value.RefreshToken.Should().NotBeNull();
    }

    [Theory]
    [InlineData("api/identity/tokens/refresh")]
    public async Task TokenRefreshTest(string route)
    {
        // arrange
        var token = await this.AcquireToken();
        var content = new StringContent(
            JsonSerializer.Serialize(new TokenRefreshRequestModel { Token = token.Value.Token, RefreshToken = token.Value.RefreshToken }), Encoding.UTF8, System.Net.Mime.MediaTypeNames.Application.Json);

        // act
        var response = await this.fixture.CreateClient().PostAsync(route, content).AnyContext();
        var result = await response.Content?.ReadAsAsync<ResultOfTokenResponseModel>();

        // asert
        response.Should().Be200Ok(); // https://github.com/adrianiftode/FluentAssertions.Web
        result.Should().NotBeNull();
        result.Value.Should().NotBeNull();
        result.Value.Token.Should().NotBeNull();
        result.Value.RefreshToken.Should().NotBeNull();
    }

    private async Task<ResultOfTokenResponseModel> AcquireToken()
    {
        // arrange
        var content = new StringContent(
            JsonSerializer.Serialize(new TokenRequestModel { Email = "admin@acmeshop.com", Password = "Fidespic032" }), Encoding.UTF8, System.Net.Mime.MediaTypeNames.Application.Json);

        // act
        var response = await this.fixture.CreateClient().PostAsync("api/identity/tokens", content).AnyContext();
        return await response.Content?.ReadAsAsync<ResultOfTokenResponseModel>();
    }
}
