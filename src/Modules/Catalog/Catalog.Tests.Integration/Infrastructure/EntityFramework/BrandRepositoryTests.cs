namespace Modules.Catalog.Tests.Integration.Infrastructure.EntityFramework;

using BridgingIT.DevKit.Common;
using BridgingIT.DevKit.Domain.Repositories;
using BridgingIT.DevKit.Domain.Specifications;
using Microsoft.Extensions.DependencyInjection;
//using Modules.Catalog.Application;
using Modules.Catalog.Domain;

[Collection(nameof(PresentationCollection))] // https://xunit.net/docs/shared-context#collection-fixture
[IntegrationTest("Infrastructure")]
[Module("Catalog")]
public class BrandRepositoryTests : IClassFixture<CustomWebApplicationFactoryFixture<Program>> // https://xunit.net/docs/shared-context#class-fixture
{
    private readonly CustomWebApplicationFactoryFixture<Program> fixture;
    private readonly IGenericRepository<Brand> sut;

    public BrandRepositoryTests(ITestOutputHelper output, CustomWebApplicationFactoryFixture<Program> fixture)
    {
        this.fixture = fixture.WithOutput(output);
        this.sut = this.fixture.ServiceProvider.GetRequiredService<IGenericRepository<Brand>>();
    }

    [Fact]
    public async Task InsertTest()
    {
        for (var i = 0; i < 5; i++)
        {
            var ticks = DateTime.UtcNow.Ticks;
            var entity = new Brand
            {
                Name = "Brand " + ticks,
                Description = "Brand " + ticks,
            };

            entity.AuditState.SetCreated("test");
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
        var entity = new Brand
        {
            Name = "Brand " + ticks,
            Description = "Brand " + ticks,
        };

        entity.AuditState.SetCreated("test");
        await this.sut.InsertAsync(entity).AnyContext();

        // act
        using (var scope = this.fixture.Services.CreateScope())
        {
            var scopedSut = scope.ServiceProvider.GetRequiredService<IGenericRepository<Brand>>();
            entity.Name = "Brand upd " + ticks;
            entity.Description = "Brand upd " + ticks;

            await scopedSut.UpsertAsync(entity).AnyContext();
        }

        // assert
        using (var scope = this.fixture.Services.CreateScope())
        {
            var scopedSut = scope.ServiceProvider.GetRequiredService<IGenericRepository<Brand>>();
            var result = await scopedSut.FindOneAsync(entity.Id).AnyContext();

            result.ShouldNotBeNull();
            result.Id.ShouldBe(entity.Id);
            result.Name.ShouldBe("Brand upd " + ticks);
            result.Description.ShouldBe("Brand upd " + ticks);
        }
    }

    [Fact]
    public async Task UpsertDisconnectedTest()
    {
        // arrange
        var ticks = DateTime.UtcNow.Ticks;
        var entity = new Brand
        {
            Name = "Brand " + ticks,
            Description = "Brand " + ticks
        };

        entity.AuditState.SetCreated("test");
        await this.sut.InsertAsync(entity).AnyContext();

        // act
        using (var scope = this.fixture.Services.CreateScope())
        {
            var scopedSut = scope.ServiceProvider.GetRequiredService<IGenericRepository<Brand>>();
            var disconnectedEntity = new Brand
            {
                Id = entity.Id, // has same id as entity > should update
                Name = "Brand upd " + ticks,
                Description = "Brand upd " + ticks
            };

            await scopedSut.UpsertAsync(disconnectedEntity).AnyContext();
        }

        // assert
        using (var scope = this.fixture.Services.CreateScope())
        {
            var scopedSut = scope.ServiceProvider.GetRequiredService<IGenericRepository<Brand>>();
            var result = await scopedSut.FindOneAsync(entity.Id).AnyContext();

            result.ShouldNotBeNull();
            result.Id.ShouldBe(entity.Id);
            result.Name.ShouldBe("Brand upd " + ticks);
            result.Description.ShouldBe("Brand upd " + ticks);
        }
    }

    [Fact]
    public async Task ExistsTest()
    {
        var ticks = DateTime.UtcNow.Ticks;
        var entity = new Brand
        {
            Name = "Brand " + ticks,
            Description = "Brand " + ticks
        };

        entity.AuditState.SetCreated("test");
        await this.sut.InsertAsync(entity).AnyContext();

        // act/assert
        using var scope = this.fixture.Services.CreateScope();
        var scopedSut = scope.ServiceProvider.GetRequiredService<IGenericRepository<Brand>>();
        var result = await scopedSut.ExistsAsync(entity.Id).AnyContext();

        result.ShouldBeTrue();
    }

    [Fact]
    public async Task FindOneByIdTest()
    {
        var ticks = DateTime.UtcNow.Ticks;
        var entity = new Brand
        {
            Name = "Brand " + ticks,
            Description = "Brand " + ticks
        };

        entity.AuditState.SetCreated("test");
        await this.sut.InsertAsync(entity).AnyContext();

        // assert
        using var scope = this.fixture.Services.CreateScope();
        var scopedSut = scope.ServiceProvider.GetRequiredService<IGenericRepository<Brand>>();
        var result = await scopedSut.FindOneAsync(entity.Id).AnyContext();

        result.ShouldNotBeNull();
        result.Id.ShouldBe(entity.Id);
    }

    [Fact]
    public async Task FindOneByIdAsStringTest()
    {
        var ticks = DateTime.UtcNow.Ticks;
        var entity = new Brand
        {
            Name = "Brand " + ticks,
            Description = "Brand " + ticks
        };

        entity.AuditState.SetCreated("test");
        await this.sut.InsertAsync(entity).AnyContext();

        // assert
        using var scope = this.fixture.Services.CreateScope();
        var scopedSut = scope.ServiceProvider.GetRequiredService<IGenericRepository<Brand>>();
        var result = await scopedSut.FindOneAsync(entity.Id.ToString()).AnyContext();

        result.ShouldNotBeNull();
        result.Id.ShouldBe(entity.Id);
    }

    [Fact]
    public async Task FindOneBySpecificationTest()
    {
        var ticks = DateTime.UtcNow.Ticks;
        var entity = new Brand
        {
            Name = "Brand " + ticks,
            Description = "Brand " + ticks
        };

        entity.AuditState.SetCreated("test");
        await this.sut.InsertAsync(entity).AnyContext();

        // assert
        using var scope = this.fixture.Services.CreateScope();
        var scopedSut = scope.ServiceProvider.GetRequiredService<IGenericRepository<Brand>>();
        var result = await scopedSut.FindOneAsync(
            new Specification<Brand>(e => e.Name == entity.Name)).AnyContext();

        result.ShouldNotBeNull();
        result.Id.ShouldBe(entity.Id);
    }

    [Fact]
    public async Task FindOneByIdSpecificationTest()
    {
        var ticks = DateTime.UtcNow.Ticks;
        var entity = new Brand
        {
            Name = "Brand " + ticks,
            Description = "Brand " + ticks
        };

        entity.AuditState.SetCreated("test");
        await this.sut.InsertAsync(entity).AnyContext();

        // assert
        using var scope = this.fixture.Services.CreateScope();
        var scopedSut = scope.ServiceProvider.GetRequiredService<IGenericRepository<Brand>>();
        var result = await scopedSut.FindOneAsync(
            new Specification<Brand>(e => e.Id == entity.Id)).AnyContext();

        result.ShouldNotBeNull();
        result.Id.ShouldBe(entity.Id);
    }

    [Fact]
    public async Task FindAllTest()
    {
        var ticks = DateTime.UtcNow.Ticks;
        var entity = new Brand
        {
            Name = "Brand " + ticks,
            Description = "Brand " + ticks
        };

        entity.AuditState.SetCreated("test");
        await this.sut.InsertAsync(entity).AnyContext();

        // assert
        using var scope = this.fixture.Services.CreateScope();
        var scopedSut = scope.ServiceProvider.GetRequiredService<IGenericRepository<Brand>>();
        var result = await scopedSut.FindAllAsync().AnyContext();

        result.ShouldNotBeNull();
        result.Count().ShouldBeGreaterThan(0);
    }

    [Fact]
    public async Task FindAllBySpecificationTest()
    {
        var ticks = DateTime.UtcNow.Ticks;
        var entity = new Brand
        {
            Name = "Brand " + ticks,
            Description = "Brand " + ticks
        };

        entity.AuditState.SetCreated("test");
        await this.sut.InsertAsync(entity).AnyContext();

        // assert
        using var scope = this.fixture.Services.CreateScope();
        var scopedSut = scope.ServiceProvider.GetRequiredService<IGenericRepository<Brand>>();
        var result = await scopedSut.FindAllAsync(
            new Specification<Brand>(e => e.Name == entity.Name)).AnyContext();

        result.ShouldNotBeNull();
        result.Count().ShouldBeGreaterThan(0);
    }

    [Fact]
    public async Task FindAllWithSkipTake()
    {
        for (var i = 0; i < 15; i++)
        {
            var ticks = DateTime.UtcNow.Ticks;
            var entity = new Brand
            {
                Name = "Brand " + ticks,
                Description = "Brand " + ticks
            };

            entity.AuditState.SetCreated("test");
            await this.sut.InsertAsync(entity).AnyContext();
        }

        // assert
        using var scope = this.fixture.Services.CreateScope();
        var scopedSut = scope.ServiceProvider.GetRequiredService<IGenericRepository<Brand>>();
        var result = await scopedSut.FindAllAsync(
            new FindOptions<Brand> { Skip = 5, Take = 5 }).AnyContext();

        result.ShouldNotBeNull();
        result.Count().ShouldBe(5);
    }

    [Fact]
    public async Task DeleteByEntityTest()
    {
        var ticks = DateTime.UtcNow.Ticks;
        var entity = new Brand
        {
            Name = "Brand " + ticks,
            Description = "Brand " + ticks
        };

        entity.AuditState.SetCreated("test");
        await this.sut.InsertAsync(entity).AnyContext();

        var result = await this.sut.DeleteAsync(entity).AnyContext();

        result.ShouldBe(RepositoryActionResult.Deleted);
    }

    [Fact]
    public async Task DeleteByIdTest()
    {
        var ticks = DateTime.UtcNow.Ticks;
        var entity = new Brand
        {
            Name = "Brand " + ticks,
            Description = "Brand " + ticks
        };

        entity.AuditState.SetCreated("test");
        await this.sut.InsertAsync(entity).AnyContext();

        var result = await this.sut.DeleteAsync(entity.Id).AnyContext();

        result.ShouldBe(RepositoryActionResult.Deleted);
    }

    [Fact]
    public async Task DeleteByIdAsStringTest()
    {
        var ticks = DateTime.UtcNow.Ticks;
        var entity = new Brand
        {
            Name = "Brand " + ticks,
            Description = "Brand " + ticks
        };

        entity.AuditState.SetCreated("test");
        await this.sut.InsertAsync(entity).AnyContext();

        var result = await this.sut.DeleteAsync(entity.Id.ToString()).AnyContext();

        result.ShouldBe(RepositoryActionResult.Deleted);
    }

    [Fact]
    public async Task UpsertTrackedTest()
    {
        // arrange: get an existing entity
        var entities = await this.sut.FindAllAsync().AnyContext(); // =tracked
        var entity = entities.FirstOrDefault();
        entity.ShouldNotBeNull();

        // act: update some properties
        var ticks = DateTime.UtcNow.Ticks;
        entity.Name = "Brand " + ticks;
        entity.Description = "Brand " + ticks;
        entity.AuditState.SetUpdated("test");
        await this.sut.UpsertAsync(entity);

        // assert
        using var scope = this.fixture.Services.CreateScope();
        var scopedSut = scope.ServiceProvider.GetRequiredService<IGenericRepository<Brand>>();
        var entity2 = await scopedSut.FindOneAsync(entity.Id).AnyContext();
        entity2.ShouldNotBeNull();
        entity2.Id.ShouldBe(entity.Id);
        entity2.Name.ShouldBe("Brand " + ticks);
        entity2.Description.ShouldBe("Brand " + ticks);
    }

    [Fact]
    public async Task UpsertNotTrackedTest()
    {
        // arrange: get an existing entity
        var entities = await this.sut.FindAllAsync(
            options: new FindOptions<Brand> { NoTracking = true }).AnyContext(); // =not tracked
        var entity = entities.FirstOrDefault();
        entity.ShouldNotBeNull();

        // act: update some properties
        var ticks = DateTime.UtcNow.Ticks;
        entity.Name = "Brand " + ticks;
        entity.Description = "Brand " + ticks;
        entity.AuditState.SetUpdated("test");
        await this.sut.UpsertAsync(entity);

        // asert
        using var scope = this.fixture.Services.CreateScope();
        var scopedSut = scope.ServiceProvider.GetRequiredService<IGenericRepository<Brand>>();
        var entity2 = await scopedSut.FindOneAsync(entity.Id).AnyContext();
        entity2.ShouldNotBeNull();
        entity2.Id.ShouldBe(entity.Id);
        entity2.Name.ShouldBe("Brand " + ticks);
        entity2.Description.ShouldBe("Brand " + ticks);
    }
}
