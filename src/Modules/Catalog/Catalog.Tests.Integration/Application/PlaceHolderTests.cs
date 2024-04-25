namespace Modules.Catalog.Tests.Integration.Application;

[IntegrationTest("Application")]
[Module("Catalog")]
public class PlaceHolderTests
{
    [Fact]
    public void Test()
    {
        true.ShouldBe(true);
    }
}
