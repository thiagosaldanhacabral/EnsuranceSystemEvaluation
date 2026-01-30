using FluentAssertions;
using SharedKernel.Domain;
using SharedKernel.Exceptions;

namespace ProposalService.UnitTests.Domain.ValueObjects;

public class MoneyTests
{
    [Fact]
    public void Create_WithValidAmount_ShouldCreateMoney()
    {
        // Arrange
        var amount = 1000.50m;

        // Act
        var money = Money.Create(amount);

        // Assert
        money.Should().NotBeNull();
        money.Amount.Should().Be(amount);
        money.Currency.Should().Be("BRL");
    }

    [Fact]
    public void Create_WithCustomCurrency_ShouldCreateMoneyWithCurrency()
    {
        // Arrange
        var amount = 1000.50m;
        var currency = "USD";

        // Act
        var money = Money.Create(amount, currency);

        // Assert
        money.Amount.Should().Be(amount);
        money.Currency.Should().Be(currency);
    }

    [Fact]
    public void Create_WithNegativeAmount_ShouldThrowDomainException()
    {
        // Act
        var act = () => Money.Create(-100m);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Money amount cannot be negative");
    }

    [Fact]
    public void Create_WithZeroAmount_ShouldCreateMoney()
    {
        // Act
        var money = Money.Create(0m);

        // Assert
        money.Amount.Should().Be(0m);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptyCurrency_ShouldThrowDomainException(string currency)
    {
        // Act
        var act = () => Money.Create(100m, currency);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Currency is required");
    }

    [Fact]
    public void Add_WithSameCurrency_ShouldReturnSum()
    {
        // Arrange
        var money1 = Money.Create(100m);
        var money2 = Money.Create(50m);

        // Act
        var result = money1.Add(money2);

        // Assert
        result.Amount.Should().Be(150m);
        result.Currency.Should().Be("BRL");
    }

    [Fact]
    public void Add_WithDifferentCurrency_ShouldThrowDomainException()
    {
        // Arrange
        var money1 = Money.Create(100m, "BRL");
        var money2 = Money.Create(50m, "USD");

        // Act
        var act = () => money1.Add(money2);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Cannot add money with different currencies");
    }

    [Fact]
    public void Subtract_WithSameCurrency_ShouldReturnDifference()
    {
        // Arrange
        var money1 = Money.Create(100m);
        var money2 = Money.Create(30m);

        // Act
        var result = money1.Subtract(money2);

        // Assert
        result.Amount.Should().Be(70m);
    }

    [Fact]
    public void Subtract_ResultingInNegative_ShouldThrowDomainException()
    {
        // Arrange
        var money1 = Money.Create(50m);
        var money2 = Money.Create(100m);

        // Act
        var act = () => money1.Subtract(money2);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Cannot subtract more than current amount");
    }

    [Fact]
    public void Multiply_WithPositiveFactor_ShouldReturnProduct()
    {
        // Arrange
        var money = Money.Create(100m);

        // Act
        var result = money.Multiply(2.5m);

        // Assert
        result.Amount.Should().Be(250m);
        result.Currency.Should().Be("BRL");
    }

    [Fact]
    public void Multiply_WithNegativeFactor_ShouldThrowDomainException()
    {
        // Arrange
        var money = Money.Create(100m);

        // Act
        var act = () => money.Multiply(-2m);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Multiplier cannot be negative");
    }

    [Fact]
    public void Equals_WithSameAmountAndCurrency_ShouldReturnTrue()
    {
        // Arrange
        var money1 = Money.Create(100m, "BRL");
        var money2 = Money.Create(100m, "BRL");

        // Act & Assert
        money1.Should().Be(money2);
        (money1 == money2).Should().BeTrue();
    }

    [Fact]
    public void Equals_WithDifferentAmount_ShouldReturnFalse()
    {
        // Arrange
        var money1 = Money.Create(100m);
        var money2 = Money.Create(50m);

        // Act & Assert
        money1.Should().NotBe(money2);
    }

    [Fact]
    public void Equals_WithDifferentCurrency_ShouldReturnFalse()
    {
        // Arrange
        var money1 = Money.Create(100m, "BRL");
        var money2 = Money.Create(100m, "USD");

        // Act & Assert
        money1.Should().NotBe(money2);
    }
}
