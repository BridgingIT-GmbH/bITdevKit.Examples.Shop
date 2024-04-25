namespace Modules.Shared.Tests.Integration.Presentation;

using BridgingIT.DevKit.Common;

[CollectionDefinition(nameof(PresentationCollection))]
public class PresentationCollection : // https://xunit.net/docs/shared-context#collection-fixture
    ICollectionFixture<CustomWebApplicationFactoryFixture<Program>>
{
}