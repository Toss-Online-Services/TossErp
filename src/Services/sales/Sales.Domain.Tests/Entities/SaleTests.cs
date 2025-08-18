using Xunit;
using TossErp.Sales.Domain.Entities;
using TossErp.Sales.Domain.Enums;
using TossErp.Sales.Domain.ValueObjects;

namespace TossErp.Sales.Domain.Tests.Entities;

public class SaleTests
{
    [Fact]
    public void Create_WithValidData_ShouldCreateSale()
    {
        // Arrange
        var tillId = Guid.NewGuid();
        var customerId = Guid.NewGuid();
        var customerName = "John Doe";
        var tenantId = "test-tenant";

        // Act
        var sale = Sale.Create(tillId, customerId, customerName, tenantId);

        // Assert
        Assert.NotEqual(Guid.Empty, sale.Id);
        Assert.Equal(tillId, sale.TillId);
        Assert.Equal(customerId, sale.CustomerId);
        Assert.Equal(customerName, sale.CustomerName);
        Assert.Equal(tenantId, sale.TenantId);
        Assert.Equal(SaleStatus.Pending, sale.Status);
        Assert.NotNull(sale.ReceiptNumber);
        Assert.Empty(sale.Items);
        Assert.Empty(sale.Payments);
        Assert.Equal(0, sale.TotalAmount.Amount);
    }

    [Fact]
    public void AddItem_WithValidItem_ShouldAddToItems()
    {
        // Arrange
        var sale = Sale.Create(Guid.NewGuid(), Guid.NewGuid(), "Customer", "tenant");
        var itemId = Guid.NewGuid();
        var itemName = "Test Item";
        var itemSku = "SKU001";
        var quantity = 2;
        var unitPrice = new Money(10.50m, "ZAR");

        // Act
        sale.AddItem(itemId, itemName, itemSku, quantity, unitPrice);

        // Assert
        Assert.Single(sale.Items);
        var item = sale.Items.First();
        Assert.Equal(itemId, item.ItemId);
        Assert.Equal(itemName, item.ItemName);
        Assert.Equal(itemSku, item.ItemSku);
        Assert.Equal(quantity, item.Quantity);
        Assert.Equal(unitPrice.Amount, item.UnitPrice.Amount);
        Assert.Equal(21.00m, sale.TotalAmount.Amount); // 2 * 10.50
    }

    [Fact]
    public void AddItem_WithZeroQuantity_ShouldThrowException()
    {
        // Arrange
        var sale = Sale.Create(Guid.NewGuid(), Guid.NewGuid(), "Customer", "tenant");
        var unitPrice = new Money(10.50m, "ZAR");

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() =>
            sale.AddItem(Guid.NewGuid(), "Item", "SKU", 0, unitPrice));
        Assert.Contains("Quantity must be greater than zero", exception.Message);
    }

    [Fact]
    public void AddItem_WithNegativeQuantity_ShouldThrowException()
    {
        // Arrange
        var sale = Sale.Create(Guid.NewGuid(), Guid.NewGuid(), "Customer", "tenant");
        var unitPrice = new Money(10.50m, "ZAR");

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() =>
            sale.AddItem(Guid.NewGuid(), "Item", "SKU", -1, unitPrice));
        Assert.Contains("Quantity must be greater than zero", exception.Message);
    }

    [Fact]
    public void AddPayment_WithValidPayment_ShouldAddToPayments()
    {
        // Arrange
        var sale = Sale.Create(Guid.NewGuid(), Guid.NewGuid(), "Customer", "tenant");
        sale.AddItem(Guid.NewGuid(), "Item", "SKU", 1, new Money(10.00m, "ZAR"));
        var paymentAmount = new Money(10.00m, "ZAR");

        // Act
        sale.AddPayment(PaymentMethod.Cash, paymentAmount, "Payment reference");

        // Assert
        Assert.Single(sale.Payments);
        var payment = sale.Payments.First();
        Assert.Equal(PaymentMethod.Cash, payment.Method);
        Assert.Equal(paymentAmount.Amount, payment.Amount.Amount);
        Assert.Equal("Payment reference", payment.Reference);
        Assert.True(payment.IsSuccessful);
    }

    [Fact]
    public void Complete_WithValidSale_ShouldChangeStatusToCompleted()
    {
        // Arrange
        var sale = Sale.Create(Guid.NewGuid(), Guid.NewGuid(), "Customer", "tenant");
        sale.AddItem(Guid.NewGuid(), "Item", "SKU", 1, new Money(10.00m, "ZAR"));
        sale.AddPayment(PaymentMethod.Cash, new Money(10.00m, "ZAR"));

        // Act
        sale.Complete();

        // Assert
        Assert.Equal(SaleStatus.Completed, sale.Status);
    }

    [Fact]
    public void Complete_WithoutItems_ShouldThrowException()
    {
        // Arrange
        var sale = Sale.Create(Guid.NewGuid(), Guid.NewGuid(), "Customer", "tenant");

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => sale.Complete());
        Assert.Contains("Cannot complete sale without items", exception.Message);
    }

    [Fact]
    public void Complete_WithoutPayments_ShouldThrowException()
    {
        // Arrange
        var sale = Sale.Create(Guid.NewGuid(), Guid.NewGuid(), "Customer", "tenant");
        sale.AddItem(Guid.NewGuid(), "Item", "SKU", 1, new Money(10.00m, "ZAR"));

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => sale.Complete());
        Assert.Contains("Cannot complete sale without payments", exception.Message);
    }

    [Fact]
    public void Cancel_WithValidSale_ShouldChangeStatusToCancelled()
    {
        // Arrange
        var sale = Sale.Create(Guid.NewGuid(), Guid.NewGuid(), "Customer", "tenant");
        var reason = "Customer cancelled";

        // Act
        sale.Cancel(reason);

        // Assert
        Assert.Equal(SaleStatus.Cancelled, sale.Status);
    }

    [Fact]
    public void Cancel_WithEmptyReason_ShouldThrowException()
    {
        // Arrange
        var sale = Sale.Create(Guid.NewGuid(), Guid.NewGuid(), "Customer", "tenant");

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => sale.Cancel(""));
        Assert.Contains("Cancellation reason is required", exception.Message);
    }

    [Fact]
    public void CalculateTotal_WithMultipleItems_ShouldCalculateCorrectTotal()
    {
        // Arrange
        var sale = Sale.Create(Guid.NewGuid(), Guid.NewGuid(), "Customer", "tenant");
        sale.AddItem(Guid.NewGuid(), "Item 1", "SKU1", 2, new Money(10.00m, "ZAR"));
        sale.AddItem(Guid.NewGuid(), "Item 2", "SKU2", 1, new Money(15.00m, "ZAR"));

        // Act
        var total = sale.TotalAmount;

        // Assert
        Assert.Equal(35.00m, total.Amount); // (2 * 10) + (1 * 15)
    }

    [Fact]
    public void CalculateTotal_WithDiscount_ShouldApplyDiscount()
    {
        // Arrange
        var sale = Sale.Create(Guid.NewGuid(), Guid.NewGuid(), "Customer", "tenant");
        sale.AddItem(Guid.NewGuid(), "Item", "SKU", 1, new Money(100.00m, "ZAR"));
        sale.ApplyDiscount(10.00m, "10% discount");

        // Act
        var total = sale.TotalAmount;

        // Assert
        Assert.Equal(90.00m, total.Amount); // 100 - 10
        Assert.Equal(10.00m, sale.DiscountAmount);
        Assert.Equal("10% discount", sale.DiscountReason);
    }
}
