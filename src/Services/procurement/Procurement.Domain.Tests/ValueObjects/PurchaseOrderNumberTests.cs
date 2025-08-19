using Xunit;
using TossErp.Procurement.Domain.ValueObjects;

namespace TossErp.Procurement.Domain.Tests.ValueObjects;

public class PurchaseOrderNumberTests
{
    [Fact]
    public void Create_WithValidNumber_ShouldCreatePurchaseOrderNumber()
    {
        // Arrange
        var number = "PO-2024-001";

        // Act
        var purchaseOrderNumber = new PurchaseOrderNumber(number);

        // Assert
        Assert.Equal(number, purchaseOrderNumber.Value);
    }

    [Fact]
    public void Create_WithEmptyNumber_ShouldThrowException()
    {
        // Arrange
        var number = "";

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new PurchaseOrderNumber(number));
    }

    [Fact]
    public void Create_WithNullNumber_ShouldThrowException()
    {
        // Arrange
        string? number = null;

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new PurchaseOrderNumber(number!));
    }

    [Fact]
    public void Create_WithInvalidFormat_ShouldThrowException()
    {
        // Arrange
        var number = "INVALID-FORMAT";

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new PurchaseOrderNumber(number));
    }

    [Fact]
    public void Generate_ShouldCreateValidPurchaseOrderNumber()
    {
        // Act
        var purchaseOrderNumber = PurchaseOrderNumber.Generate();

        // Assert
        Assert.NotNull(purchaseOrderNumber);
        Assert.NotNull(purchaseOrderNumber.Value);
        Assert.Matches(@"^PO-\d{4}-\d{3}$", purchaseOrderNumber.Value);
    }

    [Fact]
    public void Generate_MultipleTimes_ShouldCreateUniqueNumbers()
    {
        // Act
        var number1 = PurchaseOrderNumber.Generate();
        var number2 = PurchaseOrderNumber.Generate();
        var number3 = PurchaseOrderNumber.Generate();

        // Assert
        Assert.NotEqual(number1.Value, number2.Value);
        Assert.NotEqual(number2.Value, number3.Value);
        Assert.NotEqual(number1.Value, number3.Value);
    }

    [Fact]
    public void ToString_ShouldReturnValue()
    {
        // Arrange
        var number = "PO-2024-001";
        var purchaseOrderNumber = new PurchaseOrderNumber(number);

        // Act
        var result = purchaseOrderNumber.ToString();

        // Assert
        Assert.Equal(number, result);
    }

    [Fact]
    public void Equals_SameValue_ShouldReturnTrue()
    {
        // Arrange
        var number = "PO-2024-001";
        var purchaseOrderNumber1 = new PurchaseOrderNumber(number);
        var purchaseOrderNumber2 = new PurchaseOrderNumber(number);

        // Act & Assert
        Assert.Equal(purchaseOrderNumber1, purchaseOrderNumber2);
        Assert.True(purchaseOrderNumber1.Equals(purchaseOrderNumber2));
    }

    [Fact]
    public void Equals_DifferentValue_ShouldReturnFalse()
    {
        // Arrange
        var purchaseOrderNumber1 = new PurchaseOrderNumber("PO-2024-001");
        var purchaseOrderNumber2 = new PurchaseOrderNumber("PO-2024-002");

        // Act & Assert
        Assert.NotEqual(purchaseOrderNumber1, purchaseOrderNumber2);
        Assert.False(purchaseOrderNumber1.Equals(purchaseOrderNumber2));
    }

    [Fact]
    public void GetHashCode_SameValue_ShouldReturnSameHashCode()
    {
        // Arrange
        var number = "PO-2024-001";
        var purchaseOrderNumber1 = new PurchaseOrderNumber(number);
        var purchaseOrderNumber2 = new PurchaseOrderNumber(number);

        // Act & Assert
        Assert.Equal(purchaseOrderNumber1.GetHashCode(), purchaseOrderNumber2.GetHashCode());
    }

    [Fact]
    public void GetHashCode_DifferentValue_ShouldReturnDifferentHashCode()
    {
        // Arrange
        var purchaseOrderNumber1 = new PurchaseOrderNumber("PO-2024-001");
        var purchaseOrderNumber2 = new PurchaseOrderNumber("PO-2024-002");

        // Act & Assert
        Assert.NotEqual(purchaseOrderNumber1.GetHashCode(), purchaseOrderNumber2.GetHashCode());
    }
}
