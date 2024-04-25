namespace Modules.Shopping.Tests.Integration.Application;

using BridgingIT.DevKit.Application.Storage;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Modules.Shopping.Application;
using Modules.Shopping.Application.Integration;
using Modules.Shopping.Tests.Integration.Presentation;
using Xunit.Abstractions;

[Collection(nameof(PresentationCollection))] // https://xunit.net/docs/shared-context#collection-fixture
[IntegrationTest("Application")]
[Module("Shopping")]
public class CartAddItemCommandHandlerTests : IClassFixture<CustomWebApplicationFactoryFixture<Program>> // https://xunit.net/docs/shared-context#class-fixture
{
    private readonly CustomWebApplicationFactoryFixture<Program> fixture;
    private readonly IMediator mediator;
    private readonly IDocumentStoreClient<ReferenceDataProduct> documentStoreClient;

    public CartAddItemCommandHandlerTests(ITestOutputHelper output, CustomWebApplicationFactoryFixture<Program> fixture)
    {
        this.fixture = fixture.WithOutput(output);
        this.mediator = this.fixture.ServiceProvider.GetRequiredService<IMediator>();
        this.documentStoreClient = this.fixture.ServiceProvider.GetRequiredService<IDocumentStoreClient<ReferenceDataProduct>>();
    }

    [Fact]
    public async Task Process_NoExistingCartAndKnownProduct_CartWithSingleItemCreated()
    {
        // Arrange
        var ticks = DateTime.UtcNow.Ticks;
        var identity = $"TEST{ticks}";
        var sku = $"SKU{ticks}";
        await this.documentStoreClient.UpsertAsync(
            new(nameof(ReferenceDataProduct), sku + 1),
            new ReferenceDataProduct(sku + 1) { Name = $"Product name {ticks}1", Price = 3.99m });
        var command = new CartAddItemCommand(identity, sku + 1, 2);

        // Act
        var response = await this.mediator.Send(command);

        // Assert
        response.ShouldNotBeNull();
        response.Cancelled.ShouldBeFalse();
        response.Result?.Value.ShouldNotBeNull();
        response.Result.Value.Identity.ShouldBe(identity);
        response.Result.Value.Id.Value.ShouldNotBe(Guid.Empty);
        response.Result.Value.TotalPrice.Amount.ShouldBe(3.99m * 2);
        response.Result.Value.Items.Count.ShouldBe(1);
    }

    [Fact]
    public async Task Process_NoExistingCartAndKnownProducts_CartWithMultipleItemsCreated()
    {
        // Arrange
        var ticks = DateTime.UtcNow.Ticks;
        var identity = $"TEST{ticks}";
        var sku = $"SKU{ticks}";
        await this.documentStoreClient.UpsertAsync(
            new(nameof(ReferenceDataProduct), sku + 1),
            new ReferenceDataProduct(sku + 1) { Name = $"Product name {ticks}1", Price = 3.99m });
        await this.documentStoreClient.UpsertAsync(
            new(nameof(ReferenceDataProduct), sku + 2),
            new ReferenceDataProduct(sku + 2) { Name = $"Product name {ticks}2", Price = 1.99m });
        await this.documentStoreClient.UpsertAsync(
            new(nameof(ReferenceDataProduct), sku + 3),
            new ReferenceDataProduct(sku + 3) { Name = $"Product name {ticks}3", Price = 12.99m });
        var command1 = new CartAddItemCommand(identity, sku + 1, 2);
        var command2 = new CartAddItemCommand(identity, sku + 2, 1);
        var command3 = new CartAddItemCommand(identity, sku + 3, 3);

        // Act
        await this.mediator.Send(command1);
        await this.mediator.Send(command2);
        var response = await this.mediator.Send(command3);

        // Assert
        response.ShouldNotBeNull();
        response.Cancelled.ShouldBeFalse();
        response.Result?.Value.ShouldNotBeNull();
        response.Result.Value.Identity.ShouldBe(identity);
        response.Result.Value.Id.Value.ShouldNotBe(Guid.Empty);
        response.Result.Value.TotalPrice.Amount.ShouldBe((3.99m * command1.Quantity) + (1.99m * command2.Quantity) + (12.99m * command3.Quantity));
        response.Result.Value.Items.Count.ShouldBe(3);
    }

    [Fact]
    public async Task Process_NoExistingCartAndUnknownProduct_ThrowsException()
    {
        // Arrange
        var ticks = DateTime.UtcNow.Ticks;
        var identity = $"TEST{ticks}";
        var sku = $"SKU{ticks}";
        var command = new CartAddItemCommand(identity, sku, 2);

        // Act/Assert
        await Should.ThrowAsync<Exception>(async () => await this.mediator.Send(command));
    }
}