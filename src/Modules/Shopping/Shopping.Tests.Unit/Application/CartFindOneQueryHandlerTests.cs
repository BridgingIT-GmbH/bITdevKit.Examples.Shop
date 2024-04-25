namespace Modules.Shopping.Tests.Unit.Application;

using BridgingIT.DevKit.Domain.Repositories;
using BridgingIT.DevKit.Domain.Specifications;
using Microsoft.Extensions.Logging.Abstractions;
using Modules.Shopping.Application.Queries;
using Modules.Shopping.Domain;

[UnitTest("Application")]
[Module("Shopping")]
public class CartFindOneQueryHandlerTests
{
    [Fact]
    public async Task Process_NoExistingCartForUser_ReturnsNewCart() // [UnitOfWork_StateUnderTest_ExpectedBehavior] https://osherove.com/blog/2005/4/3/naming-standards-for-unit-tests.html
    {
        // Arrange
        var repository = Substitute.For<IGenericRepository<Cart>>();
        repository.FindAllAsync(Arg.Any<Specification<Cart>>())
            .Returns(Enumerable.Empty<Cart>());
        var query = new CartFindOneQuery("test");
        var sut = new CartFindOneQueryHandler(new NullLoggerFactory(), repository);

        // Act
        var response = await sut.Process(query, CancellationToken.None);

        // Assert
        await repository.Received().FindAllAsync(Arg.Any<Specification<Cart>>());
        await repository.Received().InsertAsync(Arg.Any<Cart>());
        response.ShouldNotBeNull();
        response.Result.ShouldNotBeNull();
        response.Result.Value.ShouldNotBeNull();
        response.Result.Value.Identity.ShouldBeSameAs(query.Identity);
    }

    [Fact]
    public async Task Process_ExistingCartForUser_ReturnsExistingCart() // [UnitOfWork_StateUnderTest_ExpectedBehavior] https://osherove.com/blog/2005/4/3/naming-standards-for-unit-tests.html
    {
        // Arrange
        var repository = Substitute.For<IGenericRepository<Cart>>();
        repository.FindAllAsync(Arg.Any<Specification<Cart>>())
            .Returns(new[] { Cart.ForUser("test") });
        var query = new CartFindOneQuery("test");
        var sut = new CartFindOneQueryHandler(new NullLoggerFactory(), repository);

        // Act
        var response = await sut.Process(query, CancellationToken.None);

        // Assert
        await repository.Received().FindAllAsync(Arg.Any<Specification<Cart>>());
        await repository.DidNotReceive().InsertAsync(Arg.Any<Cart>());
        response.ShouldNotBeNull();
        response.Result.ShouldNotBeNull();
        response.Result.Value.ShouldNotBeNull();
        response.Result.Value.Identity.ShouldBeSameAs(query.Identity);
    }
}