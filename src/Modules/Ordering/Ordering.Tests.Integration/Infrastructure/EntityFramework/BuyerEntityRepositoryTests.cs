namespace Modules.Ordering.Tests.Integration.Infrastructure.EntityFramework;

using System;
using System.Threading.Tasks;
using BridgingIT.DevKit.Common;
using BridgingIT.DevKit.Domain.Repositories;
using BridgingIT.DevKit.Domain.Specifications;
using Microsoft.Extensions.DependencyInjection;
using Modules.Ordering.Domain;

[Collection(nameof(PresentationCollection))] // https://xunit.net/docs/shared-context#collection-fixture
[IntegrationTest("Infrastructure")]
[Module("Ordering")]
public class BuyerEntityRepositoryTests : IClassFixture<CustomWebApplicationFactoryFixture<Program>> // https://xunit.net/docs/shared-context#class-fixture
{
    private readonly CustomWebApplicationFactoryFixture<Program> fixture;
    private readonly IGenericRepository<Buyer> sut;

    public BuyerEntityRepositoryTests(ITestOutputHelper output, CustomWebApplicationFactoryFixture<Program> fixture)
    {
        this.fixture = fixture.WithOutput(output);
        this.sut = this.fixture.ServiceProvider.GetRequiredService<IGenericRepository<Buyer>>();
    }

    [Fact]
    public async Task InsertTest()
    {
        for (var i = 0; i < 5; i++)
        {
            var ticks = DateTime.UtcNow.Ticks;
            var entity = Buyer.ForUser(
                identity: GuidGenerator.Create(ticks.ToString()).ToString(),
                name: $"John Doe-{ticks}",
                email: $"John.Doe-{ticks}@test.com");
            entity.VerifyOrAddPaymentMethod(CardType.MasterCard.Id, "MC", ticks.ToString(), "123", "John Doe", DateTime.Now.AddYears(3), Guid.NewGuid());

            var result = await this.sut.InsertAsync(entity).AnyContext();

            result.ShouldNotBeNull();
            result.Id.ShouldNotBe(Guid.Empty);
        }
    }

    // TODO: more tests ....

    [Fact]
    public async Task FindOneByEmailSpecificationTest()
    {
        var ticks = DateTime.UtcNow.Ticks;
        var entity = Buyer.ForUser(
                identity: GuidGenerator.Create(ticks.ToString()).ToString(),
                name: $"John Doe-{ticks}",
                email: $"John.Doe-{ticks}@test-{ticks}.com");
        entity.VerifyOrAddPaymentMethod(CardType.MasterCard.Id, "MC", ticks.ToString(), "123", "John Doe", DateTime.Now.AddYears(3), Guid.NewGuid());

        await this.sut.InsertAsync(entity).AnyContext();
        entity.Id.ShouldNotBe(Guid.Empty);

        // assert
        using var scope = this.fixture.Services.CreateScope();
        var scopedSut = scope.ServiceProvider.GetRequiredService<IGenericRepository<Buyer>>();
        var result = await scopedSut.FindOneAsync(
            new Specification<Buyer>(e => ((string)e.Email).EndsWith($"test-{ticks}.com"))).AnyContext(); // https://khalidabuhakmeh.com/entity-framework-core-conversions-for-logical-domain-types
                                                                                                          //                                     ^^ this is possible due to the 'operator string' overload on the EmailAddress ValueObject
        result.ShouldNotBeNull();
        result.Id.ShouldBe(entity.Id);
    }
}