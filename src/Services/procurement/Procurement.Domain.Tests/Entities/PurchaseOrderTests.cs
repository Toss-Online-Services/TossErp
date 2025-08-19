using Xunit;
using TossErp.Procurement.Domain.Entities;
using TossErp.Procurement.Domain.Enums;
using TossErp.Procurement.Domain.ValueObjects;

namespace TossErp.Procurement.Domain.Tests.Entities;

public class PurchaseOrderTests
{
    [Fact]
    public void Create_WithValidData_ShouldCreatePurchaseOrder()
    {
        // Arrange
        var supplierId = Guid.NewGuid();
        var purchaseOrderNumber = PurchaseOrderNumber.Generate();
        var tenantId = "test-tenant";

        // Act
        var purchaseOrder = PurchaseOrder.Create(supplierId, purchaseOrderNumber, tenantId);

        // Assert
        Assert.Equal(supplierId, purchaseOrder.SupplierId);
        Assert.Equal(purchaseOrderNumber, purchaseOrder.PurchaseOrderNumber);
        Assert.Equal(PurchaseOrderStatus.Draft, purchaseOrder.Status);
        Assert.Equal(tenantId, purchaseOrder.TenantId);
        Assert.Empty(purchaseOrder.Items);
    }

    [Fact]
    public void AddItem_WithValidItem_ShouldAddToItems()
    {
        // Arrange
        var purchaseOrder = CreateSamplePurchaseOrder();
        var itemId = Guid.NewGuid();
        var itemName = "Test Item";
        var itemSku = "SKU001";
        var quantity = 10;
        var unitPrice = 25.50m;
        var taxRate = 0.15m;

        // Act
        purchaseOrder.AddItem(itemId, itemName, itemSku, quantity, unitPrice, taxRate);

        // Assert
        Assert.Single(purchaseOrder.Items);
        var item = purchaseOrder.Items.First();
        Assert.Equal(itemId, item.ItemId);
        Assert.Equal(itemName, item.ItemName);
        Assert.Equal(itemSku, item.ItemSku);
        Assert.Equal(quantity, item.QuantityOrdered);
        Assert.Equal(unitPrice, item.UnitPrice);
        Assert.Equal(taxRate, item.TaxRate);
    }

    [Fact]
    public void AddItem_WithInvalidQuantity_ShouldThrowException()
    {
        // Arrange
        var purchaseOrder = CreateSamplePurchaseOrder();
        var itemId = Guid.NewGuid();
        var itemName = "Test Item";
        var itemSku = "SKU001";
        var quantity = 0; // Invalid quantity
        var unitPrice = 25.50m;
        var taxRate = 0.15m;

        // Act & Assert
        Assert.Throws<ArgumentException>(() => 
            purchaseOrder.AddItem(itemId, itemName, itemSku, quantity, unitPrice, taxRate));
    }

    [Fact]
    public void AddItem_WithInvalidUnitPrice_ShouldThrowException()
    {
        // Arrange
        var purchaseOrder = CreateSamplePurchaseOrder();
        var itemId = Guid.NewGuid();
        var itemName = "Test Item";
        var itemSku = "SKU001";
        var quantity = 10;
        var unitPrice = -25.50m; // Invalid price
        var taxRate = 0.15m;

        // Act & Assert
        Assert.Throws<ArgumentException>(() => 
            purchaseOrder.AddItem(itemId, itemName, itemSku, quantity, unitPrice, taxRate));
    }

    [Fact]
    public void Submit_WithItems_ShouldChangeStatusToSubmitted()
    {
        // Arrange
        var purchaseOrder = CreateSamplePurchaseOrder();
        purchaseOrder.AddItem(Guid.NewGuid(), "Test Item", "SKU001", 10, 25.50m, 0.15m);

        // Act
        purchaseOrder.Submit();

        // Assert
        Assert.Equal(PurchaseOrderStatus.Submitted, purchaseOrder.Status);
        Assert.NotNull(purchaseOrder.SubmittedAt);
    }

    [Fact]
    public void Submit_WithoutItems_ShouldThrowException()
    {
        // Arrange
        var purchaseOrder = CreateSamplePurchaseOrder();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => purchaseOrder.Submit());
    }

    [Fact]
    public void Approve_WithSubmittedStatus_ShouldChangeStatusToApproved()
    {
        // Arrange
        var purchaseOrder = CreateSamplePurchaseOrder();
        purchaseOrder.AddItem(Guid.NewGuid(), "Test Item", "SKU001", 10, 25.50m, 0.15m);
        purchaseOrder.Submit();

        // Act
        purchaseOrder.Approve();

        // Assert
        Assert.Equal(PurchaseOrderStatus.Approved, purchaseOrder.Status);
        Assert.NotNull(purchaseOrder.ApprovedAt);
    }

    [Fact]
    public void Approve_WithNonSubmittedStatus_ShouldThrowException()
    {
        // Arrange
        var purchaseOrder = CreateSamplePurchaseOrder();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => purchaseOrder.Approve());
    }

    [Fact]
    public void Cancel_WithValidStatus_ShouldChangeStatusToCancelled()
    {
        // Arrange
        var purchaseOrder = CreateSamplePurchaseOrder();
        var reason = "Supplier cannot deliver";

        // Act
        purchaseOrder.Cancel(reason);

        // Assert
        Assert.Equal(PurchaseOrderStatus.Cancelled, purchaseOrder.Status);
        Assert.NotNull(purchaseOrder.CancelledAt);
        Assert.Contains(reason, purchaseOrder.Notes);
    }

    [Fact]
    public void CalculateTotal_WithItems_ShouldReturnCorrectTotal()
    {
        // Arrange
        var purchaseOrder = CreateSamplePurchaseOrder();
        purchaseOrder.AddItem(Guid.NewGuid(), "Item 1", "SKU001", 2, 10.00m, 0.15m);
        purchaseOrder.AddItem(Guid.NewGuid(), "Item 2", "SKU002", 1, 20.00m, 0.15m);

        // Act
        var total = purchaseOrder.CalculateTotal();

        // Assert
        // Item 1: 2 * 10.00 = 20.00 + 15% tax = 23.00
        // Item 2: 1 * 20.00 = 20.00 + 15% tax = 23.00
        // Total: 46.00
        Assert.Equal(46.00m, total);
    }

    [Fact]
    public void CalculateTotal_WithoutItems_ShouldReturnZero()
    {
        // Arrange
        var purchaseOrder = CreateSamplePurchaseOrder();

        // Act
        var total = purchaseOrder.CalculateTotal();

        // Assert
        Assert.Equal(0m, total);
    }

    private static PurchaseOrder CreateSamplePurchaseOrder()
    {
        var supplierId = Guid.NewGuid();
        var purchaseOrderNumber = PurchaseOrderNumber.Generate();
        var tenantId = "test-tenant";
        return PurchaseOrder.Create(supplierId, purchaseOrderNumber, tenantId);
    }
}
