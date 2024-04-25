namespace Modules.Shared.Tests.Integration.Presentation.Web.Controllers;

using BridgingIT.DevKit.Common;
using BridgingIT.DevKit.Presentation;
using FluentAssertions;
using Modules.Shared.Tests.Integration.Presentation;

[Collection(nameof(PresentationCollection))] // https://xunit.net/docs/shared-context#collection-fixture
[IntegrationTest("Infrastructure")]
[Module("Shared")]
public class EndpointTests : IClassFixture<CustomWebApplicationFactoryFixture<Program>>
{
    private readonly CustomWebApplicationFactoryFixture<Program> fixture;

    public EndpointTests(ITestOutputHelper output, CustomWebApplicationFactoryFixture<Program> fixture)
    {
        this.fixture = fixture.WithOutput(output);
    }

    [Theory]
    [InlineData("api/_system/echo")]
    public async Task SystemEchoGetTest(string route)
    {
        // arrang/act
        this.fixture.Output.WriteLine($"Start Endpoint test for route: {route}");
        var response = await this.fixture.CreateClient().GetAsync(route).AnyContext();
        this.fixture.Output.WriteLine($"Finish Endpoint test for route: {route} (status={(int)response.StatusCode})");

        // assert
        response.Should().Be200Ok(); // https://github.com/adrianiftode/FluentAssertions.Web
        response.Should().MatchInContent("echo");
    }

    [Theory]
    [InlineData("api/_system/info")]
    public async Task SystemInfoGetTest(string route)
    {
        // arrange/act
        this.fixture.Output.WriteLine($"Start Endpoint test for route: {route}");
        var response = await this.fixture.CreateClient().GetAsync(route).AnyContext();
        this.fixture.Output.WriteLine($"Finish Endpoint test for route: {route} (status={(int)response.StatusCode})");

        // assert
        response.Should().Be200Ok(); // https://github.com/adrianiftode/FluentAssertions.Web
        response.Should().Satisfy<SystemInfo>(
            model =>
            {
                model.ShouldNotBeNull();
                model.Runtime.ShouldNotBeNull();
                model.Runtime.Count.ShouldBeGreaterThan(0);
                model.Request.ShouldNotBeNull();
                model.Request.Count.ShouldBeGreaterThan(0);
            });
    }
}
