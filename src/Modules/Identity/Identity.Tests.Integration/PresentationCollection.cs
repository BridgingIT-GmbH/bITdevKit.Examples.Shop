namespace Modules.Identity.Tests.Integration;

[CollectionDefinition(nameof(PresentationCollection))]
public class PresentationCollection : // https://xunit.net/docs/shared-context#collection-fixture
    ICollectionFixture<CustomWebApplicationFactoryFixture<Program>>
{
}