using Xunit;
using TossErp.Sales.Domain.ValueObjects;

namespace TossErp.Sales.Domain.Tests.ValueObjects;

public class MoneyTests
{
    [Fact]
    public void Create_WithValidAmount_ShouldCreateMoney()
    {
        // Arrange & Act
        var money = new Money(100.50m, "ZAR");

        // Assert
        Assert.Equal(100.50m, money.Amount);
        Assert.Equal("ZAR", money.Currency);
    }

    [Fact]
    public void Zero_ShouldReturnZeroAmount()
    {
        // Act
        var zero = Money.Zero();

        // Assert
        Assert.Equal(0m, zero.Amount);
        Assert.Equal("ZAR", zero.Currency);
    }

    [Fact]
    public void Add_WithSameCurrency_ShouldAddAmounts()
    {
        // Arrange
        var money1 = new Money(100.00m, "ZAR");
        var money2 = new Money(50.00m, "ZAR");

        // Act
        var result = money1 + money2;

        // Assert
        Assert.Equal(150.00m, result.Amount);
        Assert.Equal("ZAR", result.Currency);
    }

    [Fact]
    public void Add_WithDifferentCurrencies_ShouldThrowException()
    {
        // Arrange
        var money1 = new Money(100.00m, "ZAR");
        var money2 = new Money(50.00m, "USD");

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => money1 + money2);
        Assert.Contains("Cannot add money with different currencies", exception.Message);
    }

    [Fact]
    public void Subtract_WithSameCurrency_ShouldSubtractAmounts()
    {
        // Arrange
        var money1 = new Money(100.00m, "ZAR");
        var money2 = new Money(30.00m, "ZAR");

        // Act
        var result = money1 - money2;

        // Assert
        Assert.Equal(70.00m, result.Amount);
        Assert.Equal("ZAR", result.Currency);
    }

    [Fact]
    public void Subtract_WithDifferentCurrencies_ShouldThrowException()
    {
        // Arrange
        var money1 = new Money(100.00m, "ZAR");
        var money2 = new Money(30.00m, "USD");

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => money1 - money2);
        Assert.Contains("Cannot subtract money with different currencies", exception.Message);
    }

    [Fact]
    public void Multiply_WithDecimal_ShouldMultiplyAmount()
    {
        // Arrange
        var money = new Money(10.00m, "ZAR");
        var multiplier = 2.5m;

        // Act
        var result = money * multiplier;

        // Assert
        Assert.Equal(25.00m, result.Amount);
        Assert.Equal("ZAR", result.Currency);
    }

    [Fact]
    public void ToString_ShouldReturnFormattedString()
    {
        // Arrange
        var money = new Money(123.45m, "ZAR");

        // Act
        var result = money.ToString();

        // Assert
        Assert.Equal("123.45 ZAR", result);
    }

    [Fact]
    public void Equals_WithSameValues_ShouldReturnTrue()
    {
        // Arrange
        var money1 = new Money(100.00m, "ZAR");
        var money2 = new Money(100.00m, "ZAR");

        // Act & Assert
        Assert.Equal(money1, money2);
        Assert.True(money1.Equals(money2));
    }

    [Fact]
    public void Equals_WithDifferentValues_ShouldReturnFalse()
    {
        // Arrange
        var money1 = new Money(100.00m, "ZAR");
        var money2 = new Money(200.00m, "ZAR");

        // Act & Assert
        Assert.NotEqual(money1, money2);
        Assert.False(money1.Equals(money2));
    }

    [Fact]
    public void Equals_WithDifferentCurrencies_ShouldReturnFalse()
    {
        // Arrange
        var money1 = new Money(100.00m, "ZAR");
        var money2 = new Money(100.00m, "USD");

        // Act & Assert
        Assert.NotEqual(money1, money2);
        Assert.False(money1.Equals(money2));
    }

    [Fact]
    public void GetHashCode_WithSameValues_ShouldReturnSameHashCode()
    {
        // Arrange
        var money1 = new Money(100.00m, "ZAR");
        var money2 = new Money(100.00m, "ZAR");

        // Act & Assert
        Assert.Equal(money1.GetHashCode(), money2.GetHashCode());
    }
}
