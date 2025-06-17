using FluentAssertions;
using POS.Domain.AggregatesModel.SaleAggregate;
using POS.Domain.Common.ValueObjects;
using POS.Domain.Enums;
using POS.Domain.Exceptions;
using POS.Domain.Models;
using POS.Domain.Tests.Common;
using Xunit;

namespace POS.Domain.Tests.AggregatesModel.SaleAggregate;

public class SaleTests
{
    private readonly Guid _storeId = Guid.NewGuid();
    private readonly Guid _customerId = Guid.NewGuid();
    private readonly Guid _staffId = Guid.NewGuid();
    private const string SaleNumber = "SALE-001";

    [Fact]
    public void Constructor_WithValidParameters_CreatesSaleSuccessfully()
    {
        // Act
        var sale = new Sale(SaleNumber, _storeId, _customerId, _staffId);

        // Assert
        Assert.Equal(SaleNumber, sale.SaleNumber);
        Assert.Equal(_storeId, sale.StoreId);
        Assert.Equal(_customerId, sale.CustomerId);
        Assert.Equal(_staffId, sale.StaffId);
        Assert.Equal(POS.Domain.AggregatesModel.SaleAggregate.SaleStatus.Pending, sale.Status);
        Assert.True(sale.RequiresSync);
        Assert.Empty(sale.Items);
        Assert.Empty(sale.Payments);
        Assert.Empty(sale.Discounts);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Constructor_WithInvalidSaleNumber_ThrowsDomainException(string invalidSaleNumber)
    {
        // Act & Assert
        Assert.Throws<DomainException>(() => new Sale(invalidSaleNumber, _storeId, _customerId, _staffId));
    }

    [Fact]
    public void Constructor_WithEmptyStoreId_ThrowsDomainException()
    {
        // Act & Assert
        Assert.Throws<DomainException>(() => new Sale(SaleNumber, Guid.Empty, _customerId, _staffId));
    }

    [Fact]
    public void AddItem_WhenSaleIsPending_AddsItemSuccessfully()
    {
        // Arrange
        var sale = new Sale(SaleNumber, _storeId, _customerId, _staffId);
        var item = TestDataFactory.SaleFactory.CreateValidSaleItem(sale.Id);

        // Act
        sale.AddItem(item);

        // Assert
        Assert.Single(sale.Items);
        Assert.Equal(item.ProductId, sale.Items.First().ProductId);
        Assert.Equal(item.Total, sale.Subtotal);
    }

    [Fact]
    public void AddItem_WhenSaleIsNotPending_ThrowsDomainException()
    {
        // Arrange
        var sale = new Sale(SaleNumber, _storeId, _customerId, _staffId);
        var item = new SaleItem(Guid.NewGuid(), sale.Id, Guid.NewGuid(), "Test Product", 10.00m, 1, 0.10m);
        sale.StartProcessing();

        // Act & Assert
        Assert.Throws<DomainException>(() => sale.AddItem(item));
    }

    [Fact]
    public void UpdateItemQuantity_WhenItemExists_UpdatesQuantitySuccessfully()
    {
        // Arrange
        var sale = new Sale(SaleNumber, _storeId, _customerId, _staffId);
        var productId = Guid.NewGuid();
        var item = new SaleItem(Guid.NewGuid(), sale.Id, productId, "Test Product", 10.00m, 2, 0.10m);
        sale.AddItem(item);

        // Act
        sale.UpdateItemQuantity(productId, 3);

        // Assert
        Assert.Equal(3, sale.Items.First().Quantity);
        Assert.Equal(30.00m, sale.Subtotal);
    }

    [Fact]
    public void RemoveItem_WhenItemExists_RemovesItemSuccessfully()
    {
        // Arrange
        var sale = new Sale(SaleNumber, _storeId, _customerId, _staffId);
        var productId = Guid.NewGuid();
        var item = new SaleItem(Guid.NewGuid(), sale.Id, productId, "Test Product", 10.00m, 2, 0.10m);
        sale.AddItem(item);

        // Act
        sale.RemoveItem(productId);

        // Assert
        Assert.Empty(sale.Items);
        Assert.Equal(0m, sale.Subtotal);
    }

    [Fact]
    public void AddPayment_WhenSaleIsPending_AddsPaymentSuccessfully()
    {
        // Arrange
        var sale = new Sale(SaleNumber, _storeId, _customerId, _staffId);
        var payment = TestDataFactory.SaleFactory.CreateValidPayment(sale.Id);

        // Act
        sale.AddPayment(payment);

        // Assert
        Assert.Single(sale.Payments);
        Assert.Equal(payment.Amount, sale.Payments.First().Amount);
    }

    [Fact]
    public void Complete_WithValidConditions_CompletesSaleSuccessfully()
    {
        // Arrange
        var sale = new Sale(SaleNumber, _storeId, _customerId, _staffId);
        var item = new SaleItem(Guid.NewGuid(), sale.Id, Guid.NewGuid(), "Test Product", 10.00m, 2, 0.10m);
        sale.AddItem(item);
        // Get the total after adding the item
        var totalAfterItems = sale.Total;
        var payment = new Payment(Guid.NewGuid(), sale.Id, totalAfterItems, PaymentType.Cash, null, null, null);
        sale.AddPayment(payment);

        // Act
        sale.Complete();

        // Assert
        Assert.Equal(POS.Domain.AggregatesModel.SaleAggregate.SaleStatus.Completed, sale.Status);
        Assert.NotNull(sale.CompletedAt);
    }

    [Fact]
    public void Complete_WithoutItems_ThrowsDomainException()
    {
        // Arrange
        var sale = new Sale(SaleNumber, _storeId, _customerId, _staffId);

        // Act & Assert
        Assert.Throws<DomainException>(() => sale.Complete());
    }

    [Fact]
    public void Complete_WithoutPayments_ThrowsDomainException()
    {
        // Arrange
        var sale = new Sale(SaleNumber, _storeId, _customerId, _staffId);
        var item = new SaleItem(Guid.NewGuid(), sale.Id, Guid.NewGuid(), "Test Product", 10.00m, 2, 0.10m);
        sale.AddItem(item);

        // Act & Assert
        Assert.Throws<DomainException>(() => sale.Complete());
    }

    [Fact]
    public void Cancel_WithValidReason_CancelsSaleSuccessfully()
    {
        // Arrange
        var sale = new Sale(SaleNumber, _storeId, _customerId, _staffId);
        const string reason = "Customer cancelled";

        // Act
        sale.Cancel(reason);

        // Assert
        Assert.Equal(POS.Domain.AggregatesModel.SaleAggregate.SaleStatus.Cancelled, sale.Status);
        Assert.NotNull(sale.CancelledAt);
        Assert.Equal(reason, sale.CancellationReason);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Cancel_WithInvalidReason_ThrowsDomainException(string invalidReason)
    {
        // Arrange
        var sale = new Sale(SaleNumber, _storeId, _customerId, _staffId);

        // Act & Assert
        Assert.Throws<DomainException>(() => sale.Cancel(invalidReason));
    }

    [Fact]
    public void AddDiscount_WhenSaleIsPending_AddsDiscountSuccessfully()
    {
        // Arrange
        var sale = new Sale(SaleNumber, _storeId, _customerId, _staffId);
        var discount = TestDataFactory.SaleFactory.CreateValidSaleDiscount();

        // Act
        sale.AddDiscount(discount);

        // Assert
        Assert.Single(sale.Discounts);
        Assert.Equal(discount.Amount, sale.Discount);
    }

    [Fact]
    public void UpdateAddress_WhenSaleIsPending_UpdatesAddressSuccessfully()
    {
        // Arrange
        var sale = new Sale(SaleNumber, _storeId, _customerId, _staffId);
        var address = TestDataFactory.ValueObjects.CreateValidAddress();

        // Act
        sale.UpdateAddress(address);

        // Assert
        Assert.NotNull(sale.Address);
        Assert.Equal(address.Street, sale.Address.Street);
    }

    [Fact]
    public void MarkAsSynced_WithSuccess_UpdatesSyncStatusCorrectly()
    {
        // Arrange
        var sale = new Sale(SaleNumber, _storeId, _customerId, _staffId);

        // Act
        sale.MarkAsSynced(true);

        // Assert
        Assert.False(sale.RequiresSync);
        Assert.Null(sale.SyncError);
        Assert.NotNull(sale.LastSyncedAt);
    }

    [Fact]
    public void MarkAsSynced_WithFailure_UpdatesSyncStatusCorrectly()
    {
        // Arrange
        var sale = new Sale(SaleNumber, _storeId, _customerId, _staffId);
        const string errorMessage = "Sync failed";

        // Act
        sale.MarkAsSynced(false, errorMessage);

        // Assert
        Assert.True(sale.RequiresSync);
        Assert.Equal(errorMessage, sale.SyncError);
        Assert.NotNull(sale.LastSyncedAt);
    }
} 
