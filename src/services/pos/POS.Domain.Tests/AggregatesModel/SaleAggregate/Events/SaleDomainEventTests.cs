using FluentAssertions;
using POS.Domain.AggregatesModel.SaleAggregate;
using POS.Domain.AggregatesModel.SaleAggregate.Events;
using POS.Domain.Common.ValueObjects;
using POS.Domain.Enums;
using Xunit;

namespace POS.Domain.Tests.AggregatesModel.SaleAggregate.Events;

public class SaleDomainEventTests
{
    private readonly Guid _saleId = Guid.NewGuid();
    private readonly Guid _storeId = Guid.NewGuid();
    private readonly Guid _customerId = Guid.NewGuid();
    private readonly Guid _staffId = Guid.NewGuid();
    private readonly string _saleNumber = "SALE-001";
    private readonly Money _itemPrice = new(100.00m, "USD");
    private readonly int _itemQuantity = 2;

    [Fact]
    public void SaleCreatedDomainEvent_ShouldContainCorrectData()
    {
        // Arrange
        var sale = Sale.Create(_saleNumber, _storeId, _customerId, _staffId);
        sale.Id = _saleId;

        // Act
        var @event = new SaleCreatedDomainEvent(_saleId, _storeId, _customerId, _staffId, _saleNumber);

        // Assert
        @event.SaleId.Should().Be(_saleId);
        @event.StoreId.Should().Be(_storeId);
        @event.CustomerId.Should().Be(_customerId);
        @event.StaffId.Should().Be(_staffId);
        @event.SaleNumber.Should().Be(_saleNumber);
    }

    [Fact]
    public void SaleCompletedDomainEvent_ShouldContainCorrectData()
    {
        // Arrange
        var sale = Sale.Create(_saleNumber, _storeId, _customerId, _staffId);
        sale.Id = _saleId;
        var completedAt = DateTime.UtcNow;

        // Act
        var @event = new SaleCompletedDomainEvent(_saleId, completedAt);

        // Assert
        @event.SaleId.Should().Be(_saleId);
        @event.CompletedAt.Should().Be(completedAt);
    }

    [Fact]
    public void SaleVoidedDomainEvent_ShouldContainCorrectData()
    {
        // Arrange
        var sale = Sale.Create(_saleNumber, _storeId, _customerId, _staffId);
        sale.Id = _saleId;
        var reason = "Test void reason";
        var approvedBy = Guid.NewGuid();
        var voidedAt = DateTime.UtcNow;

        // Act
        var @event = new SaleVoidedDomainEvent(_saleId, reason, approvedBy, voidedAt);

        // Assert
        @event.SaleId.Should().Be(_saleId);
        @event.Reason.Should().Be(reason);
        @event.ApprovedBy.Should().Be(approvedBy);
        @event.VoidedAt.Should().Be(voidedAt);
    }

    [Fact]
    public void SaleCancelledDomainEvent_ShouldContainCorrectData()
    {
        // Arrange
        var sale = Sale.Create(_saleNumber, _storeId, _customerId, _staffId);
        sale.Id = _saleId;
        var reason = "Test cancel reason";
        var cancelledAt = DateTime.UtcNow;

        // Act
        var @event = new SaleCancelledDomainEvent(_saleId, reason, cancelledAt);

        // Assert
        @event.SaleId.Should().Be(_saleId);
        @event.Reason.Should().Be(reason);
        @event.CancelledAt.Should().Be(cancelledAt);
    }

    [Fact]
    public void SalePaymentAddedDomainEvent_ShouldContainCorrectData()
    {
        // Arrange
        var sale = Sale.Create(_saleNumber, _storeId, _customerId, _staffId);
        sale.Id = _saleId;
        var paymentAmount = new Money(100.00m, "USD");
        var paymentMethod = PaymentType.Cash;
        var addedAt = DateTime.UtcNow;

        // Act
        var @event = new SalePaymentAddedDomainEvent(_saleId, paymentMethod, paymentAmount, addedAt);

        // Assert
        @event.SaleId.Should().Be(_saleId);
        @event.Method.Should().Be(paymentMethod);
        @event.Amount.Should().Be(paymentAmount);
        @event.AddedAt.Should().Be(addedAt);
    }

    [Fact]
    public void SaleDiscountAddedDomainEvent_ShouldContainCorrectData()
    {
        // Arrange
        var sale = Sale.Create(_saleNumber, _storeId, _customerId, _staffId);
        sale.Id = _saleId;
        var discountAmount = new Money(10.00m, "USD");
        var discountType = DiscountType.Percentage;
        var reason = "Test discount";
        var addedAt = DateTime.UtcNow;

        // Act
        var @event = new SaleDiscountAddedDomainEvent(_saleId, discountType, discountAmount, reason, addedAt);

        // Assert
        @event.SaleId.Should().Be(_saleId);
        @event.Type.Should().Be(discountType);
        @event.Amount.Should().Be(discountAmount);
        @event.Reason.Should().Be(reason);
        @event.AddedAt.Should().Be(addedAt);
    }

    [Fact]
    public void SaleNotesUpdatedDomainEvent_ShouldContainCorrectData()
    {
        // Arrange
        var sale = Sale.Create(_saleNumber, _storeId, _customerId, _staffId);
        sale.Id = _saleId;
        var notes = "Test notes";
        var updatedAt = DateTime.UtcNow;

        // Act
        var @event = new SaleNotesUpdatedDomainEvent(_saleId, notes, updatedAt);

        // Assert
        @event.SaleId.Should().Be(_saleId);
        @event.Notes.Should().Be(notes);
        @event.UpdatedAt.Should().Be(updatedAt);
    }

    [Fact]
    public void SaleAddressUpdatedDomainEvent_ShouldContainCorrectData()
    {
        // Arrange
        var sale = Sale.Create(_saleNumber, _storeId, _customerId, _staffId);
        sale.Id = _saleId;
        var address = new Address("123 Test St", "Test City", "Test State", "12345", "Test Country");
        var updatedAt = DateTime.UtcNow;

        // Act
        var @event = new SaleAddressUpdatedDomainEvent(_saleId, address, updatedAt);

        // Assert
        @event.SaleId.Should().Be(_saleId);
        @event.Address.Should().Be(address);
        @event.UpdatedAt.Should().Be(updatedAt);
    }

    [Fact]
    public void SaleProcessingStartedDomainEvent_ShouldContainCorrectData()
    {
        // Arrange
        var sale = Sale.Create(_saleNumber, _storeId, _customerId, _staffId);
        sale.Id = _saleId;
        var startedAt = DateTime.UtcNow;

        // Act
        var @event = new SaleProcessingStartedDomainEvent(_saleId, startedAt);

        // Assert
        @event.SaleId.Should().Be(_saleId);
        @event.StartedAt.Should().Be(startedAt);
    }
} 
