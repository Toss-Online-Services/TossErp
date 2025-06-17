using FluentAssertions;
using POS.Domain.Common.ValueObjects;
using POS.Domain.Tests.Common;
using Xunit;

namespace POS.Domain.Tests.Common.ValueObjects;

public class MoneyTests
{
    [Fact]
    public void Create_WithValidData_ShouldCreateMoney()
    {
        // Arrange & Act
        var money = TestDataFactory.ValueObjects.CreateValidMoney();

        // Assert
        money.Should().NotBeNull();
        money.Amount.Should().BeGreaterThan(0);
        money.Currency.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void Create_WithNegativeAmount_ShouldThrowException()
    {
        // Arrange
        var amount = -100.00m;
        var currency = "USD";

        // Act & Assert
        var action = () => new Money(amount, currency);
        action.Should().Throw<ArgumentException>()
            .WithMessage("Amount cannot be negative");
    }

    [Fact]
    public void Create_WithEmptyCurrency_ShouldThrowException()
    {
        // Arrange
        var amount = 100.00m;
        var currency = string.Empty;

        // Act & Assert
        var action = () => new Money(amount, currency);
        action.Should().Throw<ArgumentException>()
            .WithMessage("Currency cannot be empty");
    }

    [Fact]
    public void Add_WithSameCurrency_ShouldAddAmounts()
    {
        // Arrange
        var money1 = new Money(100.00m, "USD");
        var money2 = new Money(200.00m, "USD");

        // Act
        var result = money1.Add(money2);

        // Assert
        result.Amount.Should().Be(300.00m);
        result.Currency.Should().Be("USD");
    }

    [Fact]
    public void Add_WithDifferentCurrencies_ShouldThrowException()
    {
        // Arrange
        var money1 = new Money(100.00m, "USD");
        var money2 = new Money(200.00m, "EUR");

        // Act & Assert
        var action = () => money1.Add(money2);
        action.Should().Throw<InvalidOperationException>()
            .WithMessage("Cannot add money with different currencies");
    }

    [Fact]
    public void Subtract_WithSameCurrency_ShouldSubtractAmounts()
    {
        // Arrange
        var money1 = new Money(300.00m, "USD");
        var money2 = new Money(100.00m, "USD");

        // Act
        var result = money1.Subtract(money2);

        // Assert
        result.Amount.Should().Be(200.00m);
        result.Currency.Should().Be("USD");
    }

    [Fact]
    public void Subtract_WithDifferentCurrencies_ShouldThrowException()
    {
        // Arrange
        var money1 = new Money(300.00m, "USD");
        var money2 = new Money(100.00m, "EUR");

        // Act & Assert
        var action = () => money1.Subtract(money2);
        action.Should().Throw<InvalidOperationException>()
            .WithMessage("Cannot subtract money with different currencies");
    }

    [Fact]
    public void Multiply_WithValidMultiplier_ShouldMultiplyAmount()
    {
        // Arrange
        var money = new Money(100.00m, "USD");
        var multiplier = 2.5m;

        // Act
        var result = money.Multiply(multiplier);

        // Assert
        result.Amount.Should().Be(250.00m);
        result.Currency.Should().Be("USD");
    }

    [Fact]
    public void Multiply_WithNegativeMultiplier_ShouldThrowException()
    {
        // Arrange
        var money = new Money(100.00m, "USD");
        var multiplier = -2.5m;

        // Act & Assert
        var action = () => money.Multiply(multiplier);
        action.Should().Throw<ArgumentException>()
            .WithMessage("Multiplier cannot be negative");
    }

    [Fact]
    public void Equals_WithSameValues_ShouldReturnTrue()
    {
        // Arrange
        var money1 = new Money(100.00m, "USD");
        var money2 = new Money(100.00m, "USD");

        // Act & Assert
        money1.Equals(money2).Should().BeTrue();
        (money1 == money2).Should().BeTrue();
    }

    [Fact]
    public void Equals_WithDifferentValues_ShouldReturnFalse()
    {
        // Arrange
        var money1 = new Money(100.00m, "USD");
        var money2 = new Money(200.00m, "USD");
        var money3 = new Money(100.00m, "EUR");

        // Act & Assert
        money1.Equals(money2).Should().BeFalse();
        money1.Equals(money3).Should().BeFalse();
        (money1 != money2).Should().BeTrue();
    }

    [Fact]
    public void ToString_ShouldReturnFormattedString()
    {
        // Arrange
        var money = new Money(100.00m, "USD");

        // Act
        var result = money.ToString();

        // Assert
        result.Should().Be("$100.00");
    }
} 
