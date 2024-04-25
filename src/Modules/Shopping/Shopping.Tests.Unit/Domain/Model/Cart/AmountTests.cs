namespace Modules.Shopping.Tests.Unit.Domain;

using BridgingIT.DevKit.Domain;
using Modules.Shopping.Domain;

[UnitTest("Domain")]
[Module("Shopping")]
public class AmountTests
{
    [Fact]
    public void For_WithPositiveAmount_CreatesNewAmount() // [UnitOfWork_StateUnderTest_ExpectedBehavior] https://osherove.com/blog/2005/4/3/naming-standards-for-unit-tests.html
    {
        // Arrange
        // Act
        var sut = Amount.For(10);

        // Assert
        sut.Amount.ShouldBe(10);
    }

    [Fact]
    public void For_WithNegativeAmount_ThrowsException() // [UnitOfWork_StateUnderTest_ExpectedBehavior] https://osherove.com/blog/2005/4/3/naming-standards-for-unit-tests.html
    {
        // Arrange
        // Act
        // Assert
        Should.Throw<BusinessRuleNotSatisfiedException>(() => Amount.For(-10));
    }

    [Fact]
    public void Operators_VariousAmounts_CompareAsExpected() // [UnitOfWork_StateUnderTest_ExpectedBehavior] https://osherove.com/blog/2005/4/3/naming-standards-for-unit-tests.html
    {
        // Arrange
        var sut00 = Amount.For(00);
        var sut10 = Amount.For(10);
        var sut99 = Amount.For(99);

        // Act
        // Assert
        (sut10 < sut99).ShouldBe(true);
        (sut99 > sut10).ShouldBe(true);
        (sut10 != sut99).ShouldBe(true);
        (sut10 == sut99).ShouldBe(false);
        (sut10 == Amount.For(10)).ShouldBe(true);
        (sut10 == 10).ShouldBe(true);
        (sut00 == 0).ShouldBe(true);
        (Amount.Zero == 0).ShouldBe(true);
        (sut10 + sut99).ShouldBe(109);
        (sut99 - sut10).ShouldBe(89);
    }

    [Fact]
    public void Comparable_VariousAmounts_OrderAsExpected() // [UnitOfWork_StateUnderTest_ExpectedBehavior] https://osherove.com/blog/2005/4/3/naming-standards-for-unit-tests.html
    {
        // Arrange
        var sut00 = Amount.For(00);
        var sut10 = Amount.For(10);
        var sut99 = Amount.For(99);

        // Act
        var result = new[] { sut99, sut00, sut10 }.OrderBy(s => s);

        // Assert
        result.First().ShouldBe(sut00);
        result.Last().ShouldBe(sut99);
    }
}
