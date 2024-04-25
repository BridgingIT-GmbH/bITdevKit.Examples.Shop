namespace Modules.Inventory.Tests.Integration.Infrastructure.LiteDb;

using System.Threading.Tasks;
using BridgingIT.DevKit.Common;
using BridgingIT.DevKit.Domain.Repositories;
using BridgingIT.DevKit.Domain.Specifications;
using Microsoft.Extensions.DependencyInjection;
//using Modules.Inventory.Application;
using Modules.Inventory.Domain;

[Collection(nameof(PresentationCollection))] // https://xunit.net/docs/shared-context#collection-fixture
[IntegrationTest("Infrastructure")]
[Module("Inventory")]
public class StockRepositoryTests : IClassFixture<CustomWebApplicationFactoryFixture<Program>> // https://xunit.net/docs/shared-context#class-fixture
{
    private readonly CustomWebApplicationFactoryFixture<Program> fixture;
    private readonly IGenericRepository<Stock> sut;

    public StockRepositoryTests(ITestOutputHelper output, CustomWebApplicationFactoryFixture<Program> fixture)
    {
        this.fixture = fixture.WithOutput(output);
        // OR this.factory = new CustomWebApplicationFactory<Program>(testOutputHelper, services =>
        //    services.PostConfigure<InventoryModuleConfiguration>(c =>
        //        c.ConnectionStrings.AddOrUpdate("LiteDb", "Filename=data_inventory_test.db;Connection=shared")));
        // ^^ combine above PostConfigure with a TestcontainerDatabase (docker) > https://youtu.be/8IRNC7qZBmk?t=728
        // OR this.factory = new CustomWebApplicationFactory<Program>(testOutputHelper, environment: "docker"); which is more static and uses */appsettings.docker.json
        this.sut = this.fixture.ServiceProvider.GetRequiredService<IGenericRepository<Stock>>();
    }

    [Fact]
    public async Task InsertTest()
    {
        for (var i = 0; i < 5; i++)
        {
            var ticks = DateTime.UtcNow.Ticks;
            var entity = new Stock
            {
                SKU = GuidGenerator.Create($"{nameof(Stock)}_{ticks}").ToString("N"),
                AvailableQuantity = 12,
                OnRestock = true,
                RestockQuantity = 6,
                RestockThreshold = 10,
                MaxStockThreshold = 20
            };

            entity.State.SetCreated("test");
            var result = await this.sut.InsertAsync(entity).AnyContext();

            result.ShouldNotBeNull();
            result.Id.ShouldNotBe(Guid.Empty);
        }
    }

    [Fact]
    public async Task UpsertTest()
    {
        // arrange
        var ticks = DateTime.UtcNow.Ticks;
        var entity = new Stock
        {
            SKU = GuidGenerator.Create($"{nameof(Stock)}_{ticks}").ToString("N"),
            AvailableQuantity = 12,
            OnRestock = true,
            RestockQuantity = 6,
            RestockThreshold = 10,
            MaxStockThreshold = 20
        };

        entity.State.SetCreated("test");
        await this.sut.InsertAsync(entity).AnyContext();

        // act
        using (var scope = this.fixture.Services.CreateScope())
        {
            ticks = DateTime.UtcNow.Ticks;
            var scopedSut = scope.ServiceProvider.GetRequiredService<IGenericRepository<Stock>>();
            entity.SKU = GuidGenerator.Create($"{nameof(Stock)}_{ticks}").ToString("N");
            entity.AvailableQuantity = 10;

            await scopedSut.UpsertAsync(entity).AnyContext();
        }

        // assert
        using (var scope = this.fixture.Services.CreateScope())
        {
            var scopedSut = scope.ServiceProvider.GetRequiredService<IGenericRepository<Stock>>();
            var result = await scopedSut.FindOneAsync(entity.Id).AnyContext();

            result.ShouldNotBeNull();
            result.Id.ShouldBe(entity.Id);
            result.SKU.ShouldBe(GuidGenerator.Create($"{nameof(Stock)}_{ticks}").ToString("N"));
            result.AvailableQuantity.ShouldBe(10);
        }
    }

    [Fact]
    public async Task UpsertDisconnectedTest()
    {
        // arrange
        var ticks = DateTime.UtcNow.Ticks;
        var entity = new Stock
        {
            SKU = GuidGenerator.Create($"{nameof(Stock)}_{ticks}").ToString("N"),
            AvailableQuantity = 12,
            OnRestock = true,
            RestockQuantity = 6,
            RestockThreshold = 10,
            MaxStockThreshold = 20
        };

        entity.State.SetCreated("test");
        await this.sut.InsertAsync(entity).AnyContext();

        // act
        using (var scope = this.fixture.Services.CreateScope())
        {
            ticks = DateTime.UtcNow.Ticks;
            var scopedSut = scope.ServiceProvider.GetRequiredService<IGenericRepository<Stock>>();
            var disconnectedEntity = new Stock
            {
                Id = entity.Id, // has same id as entity > should update
                SKU = GuidGenerator.Create($"{nameof(Stock)}_{ticks}").ToString("N"),
                AvailableQuantity = 10
            };

            await scopedSut.UpsertAsync(disconnectedEntity).AnyContext();
        }

        // assert
        using (var scope = this.fixture.Services.CreateScope())
        {
            var scopedSut = scope.ServiceProvider.GetRequiredService<IGenericRepository<Stock>>();
            var result = await scopedSut.FindOneAsync(entity.Id).AnyContext();

            result.ShouldNotBeNull();
            result.Id.ShouldBe(entity.Id);
            result.SKU.ShouldBe(GuidGenerator.Create($"{nameof(Stock)}_{ticks}").ToString("N"));
            result.AvailableQuantity.ShouldBe(10);
        }
    }

    [Fact]
    public async Task ExistsTest()
    {
        var ticks = DateTime.UtcNow.Ticks;
        var entity = new Stock
        {
            SKU = GuidGenerator.Create($"{nameof(Stock)}_{ticks}").ToString("N"),
            AvailableQuantity = 12,
            OnRestock = true,
            RestockQuantity = 6,
            RestockThreshold = 10,
            MaxStockThreshold = 20
        };

        entity.State.SetCreated("test");
        await this.sut.InsertAsync(entity).AnyContext();

        // act/assert
        using var scope = this.fixture.Services.CreateScope();
        var scopedSut = scope.ServiceProvider.GetRequiredService<IGenericRepository<Stock>>();
        var result = await scopedSut.ExistsAsync(entity.Id).AnyContext();

        result.ShouldBeTrue();
    }

    [Fact]
    public async Task FindOneByIdTest()
    {
        var ticks = DateTime.UtcNow.Ticks;
        var entity = new Stock
        {
            SKU = GuidGenerator.Create($"{nameof(Stock)}_{ticks}").ToString("N"),
            AvailableQuantity = 12,
            OnRestock = true,
            RestockQuantity = 6,
            RestockThreshold = 10,
            MaxStockThreshold = 20
        };

        entity.State.SetCreated("test");
        await this.sut.InsertAsync(entity).AnyContext();

        // assert
        using var scope = this.fixture.Services.CreateScope();
        var scopedSut = scope.ServiceProvider.GetRequiredService<IGenericRepository<Stock>>();
        var result = await scopedSut.FindOneAsync(entity.Id).AnyContext();

        result.ShouldNotBeNull();
        result.Id.ShouldBe(entity.Id);
    }

    [Fact]
    public async Task FindOneByIdAsStringTest()
    {
        var ticks = DateTime.UtcNow.Ticks;
        var entity = new Stock
        {
            SKU = GuidGenerator.Create($"{nameof(Stock)}_{ticks}").ToString("N"),
            AvailableQuantity = 12,
            OnRestock = true,
            RestockQuantity = 6,
            RestockThreshold = 10,
            MaxStockThreshold = 20
        };

        entity.State.SetCreated("test");
        await this.sut.InsertAsync(entity).AnyContext();

        // assert
        using var scope = this.fixture.Services.CreateScope();
        var scopedSut = scope.ServiceProvider.GetRequiredService<IGenericRepository<Stock>>();
        var result = await scopedSut.FindOneAsync(entity.Id.ToString()).AnyContext();

        result.ShouldNotBeNull();
        result.Id.ShouldBe(entity.Id);
    }

    [Fact]
    public async Task FindOneBySpecificationTest()
    {
        var ticks = DateTime.UtcNow.Ticks;
        var entity = new Stock
        {
            SKU = GuidGenerator.Create($"{nameof(Stock)}_{ticks}").ToString("N"),
            AvailableQuantity = 12,
            OnRestock = true,
            RestockQuantity = 6,
            RestockThreshold = 10,
            MaxStockThreshold = 20
        };

        entity.State.SetCreated("test");
        await this.sut.InsertAsync(entity).AnyContext();

        // assert
        using var scope = this.fixture.Services.CreateScope();
        var scopedSut = scope.ServiceProvider.GetRequiredService<IGenericRepository<Stock>>();
        var result = await scopedSut.FindOneAsync(
            new Specification<Stock>(e => e.SKU == entity.SKU)).AnyContext();

        result.ShouldNotBeNull();
        result.Id.ShouldBe(entity.Id);
    }

    [Fact]
    public async Task FindOneByIdSpecificationTest()
    {
        var ticks = DateTime.UtcNow.Ticks;
        var entity = new Stock
        {
            SKU = GuidGenerator.Create($"{nameof(Stock)}_{ticks}").ToString("N"),
            AvailableQuantity = 12,
            OnRestock = true,
            RestockQuantity = 6,
            RestockThreshold = 10,
            MaxStockThreshold = 20
        };

        entity.State.SetCreated("test");
        await this.sut.InsertAsync(entity).AnyContext();

        // assert
        using var scope = this.fixture.Services.CreateScope();
        var scopedSut = scope.ServiceProvider.GetRequiredService<IGenericRepository<Stock>>();
        var result = await scopedSut.FindOneAsync(
            new Specification<Stock>(e => e.Id == entity.Id)).AnyContext();

        result.ShouldNotBeNull();
        result.Id.ShouldBe(entity.Id);
    }

    [Fact]
    public async Task FindAllTest()
    {
        var ticks = DateTime.UtcNow.Ticks;
        var entity = new Stock
        {
            SKU = GuidGenerator.Create($"{nameof(Stock)}_{ticks}").ToString("N"),
            AvailableQuantity = 12,
            OnRestock = true,
            RestockQuantity = 6,
            RestockThreshold = 10,
            MaxStockThreshold = 20
        };

        entity.State.SetCreated("test");
        await this.sut.InsertAsync(entity).AnyContext();

        // assert
        using var scope = this.fixture.Services.CreateScope();
        var scopedSut = scope.ServiceProvider.GetRequiredService<IGenericRepository<Stock>>();
        var result = await scopedSut.FindAllAsync(
            options: new FindOptions<Stock>(
                order: new OrderOption<Stock>(o => o.SKU, OrderDirection.Descending))).AnyContext();

        result.ShouldNotBeNull();
        result.Count().ShouldBeGreaterThan(0);
    }

    [Fact]
    public async Task FindAllBySpecificationTest()
    {
        var ticks = DateTime.UtcNow.Ticks;
        var entity = new Stock
        {
            SKU = GuidGenerator.Create($"{nameof(Stock)}_{ticks}").ToString("N"),
            AvailableQuantity = 12,
            OnRestock = true,
            RestockQuantity = 6,
            RestockThreshold = 10,
            MaxStockThreshold = 20
        };

        entity.State.SetCreated("test");
        await this.sut.InsertAsync(entity).AnyContext();

        // assert
        using var scope = this.fixture.Services.CreateScope();
        var scopedSut = scope.ServiceProvider.GetRequiredService<IGenericRepository<Stock>>();
        var result = await scopedSut.FindAllAsync(
            new Specification<Stock>(e => e.SKU == entity.SKU)).AnyContext();

        result.ShouldNotBeNull();
        result.Count().ShouldBeGreaterThan(0);
    }

    [Fact]
    public async Task FindAllWithSkipTake()
    {
        for (var i = 0; i < 15; i++)
        {
            var ticks = DateTime.UtcNow.Ticks;
            var entity = new Stock
            {
                SKU = GuidGenerator.Create($"{nameof(Stock)}_{ticks}").ToString("N"),
                AvailableQuantity = 12,
                OnRestock = true,
                RestockQuantity = 6,
                RestockThreshold = 10,
                MaxStockThreshold = 20
            };

            entity.State.SetCreated("test");
            await this.sut.InsertAsync(entity).AnyContext();
        }

        // assert
        using var scope = this.fixture.Services.CreateScope();
        var scopedSut = scope.ServiceProvider.GetRequiredService<IGenericRepository<Stock>>();
        var result = await scopedSut.FindAllAsync(
            new FindOptions<Stock> { Skip = 5, Take = 5 }).AnyContext();

        result.ShouldNotBeNull();
        result.Count().ShouldBe(5);
    }

    [Fact]
    public async Task DeleteByEntityTest()
    {
        var ticks = DateTime.UtcNow.Ticks;
        var entity = new Stock
        {
            SKU = GuidGenerator.Create($"{nameof(Stock)}_{ticks}").ToString("N"),
            AvailableQuantity = 12,
            OnRestock = true,
            RestockQuantity = 6,
            RestockThreshold = 10,
            MaxStockThreshold = 20
        };

        entity.State.SetCreated("test");
        await this.sut.InsertAsync(entity).AnyContext();

        var result = await this.sut.DeleteAsync(entity).AnyContext();

        result.ShouldBe(RepositoryActionResult.Deleted);
    }

    [Fact]
    public async Task DeleteByIdTest()
    {
        var ticks = DateTime.UtcNow.Ticks;
        var entity = new Stock
        {
            SKU = GuidGenerator.Create($"{nameof(Stock)}_{ticks}").ToString("N"),
            AvailableQuantity = 12,
            OnRestock = true,
            RestockQuantity = 6,
            RestockThreshold = 10,
            MaxStockThreshold = 20
        };

        entity.State.SetCreated("test");
        await this.sut.InsertAsync(entity).AnyContext();

        var result = await this.sut.DeleteAsync(entity.Id).AnyContext();

        result.ShouldBe(RepositoryActionResult.Deleted);
    }

    [Fact]
    public async Task DeleteByIdAsStringTest()
    {
        var ticks = DateTime.UtcNow.Ticks;
        var entity = new Stock
        {
            SKU = GuidGenerator.Create($"{nameof(Stock)}_{ticks}").ToString("N"),
            AvailableQuantity = 12,
            OnRestock = true,
            RestockQuantity = 6,
            RestockThreshold = 10,
            MaxStockThreshold = 20
        };

        entity.State.SetCreated("test");
        await this.sut.InsertAsync(entity).AnyContext();

        var result = await this.sut.DeleteAsync(entity.Id.ToString()).AnyContext();

        result.ShouldBe(RepositoryActionResult.Deleted);
    }
}
