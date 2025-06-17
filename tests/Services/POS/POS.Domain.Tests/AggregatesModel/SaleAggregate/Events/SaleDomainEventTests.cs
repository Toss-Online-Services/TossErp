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
        var saleId = Guid.NewGuid();
        var storeId = Guid.NewGuid();
        var createdAt = DateTime.UtcNow;

        // Act
        var @event = new SaleCreatedDomainEvent(saleId, storeId, createdAt);

        // Assert
        @event.SaleId.Should().Be(saleId);
        @event.StoreId.Should().Be(storeId);
        @event.CreatedAt.Should().BeCloseTo(createdAt, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void SaleCompletedDomainEvent_ShouldContainCorrectData()
    {
        // Arrange
        var saleId = Guid.NewGuid();
        var totalAmount = new Money(100.00m, "USD");
        var completedAt = DateTime.UtcNow;

        // Act
        var @event = new SaleCompletedDomainEvent(saleId, totalAmount, completedAt);

        // Assert
        @event.SaleId.Should().Be(saleId);
        @event.TotalAmount.Should().Be(totalAmount);
        @event.CompletedAt.Should().BeCloseTo(completedAt, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void SaleVoidedDomainEvent_ShouldContainCorrectData()
    {
        // Arrange
        var saleId = Guid.NewGuid();
        var reason = "Test void reason";
        var approvedBy = Guid.NewGuid();
        var voidedAt = DateTime.UtcNow;

        // Act
        var @event = new SaleVoidedDomainEvent(saleId, reason, voidedAt, approvedBy);

        // Assert
        @event.SaleId.Should().Be(saleId);
        @event.Reason.Should().Be(reason);
        @event.ApprovedBy.Should().Be(approvedBy);
        @event.VoidedAt.Should().BeCloseTo(voidedAt, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void SaleCancelledDomainEvent_ShouldContainCorrectData()
    {
        // Arrange
        var saleId = Guid.NewGuid();
        var reason = "Test cancel reason";
        var cancelledAt = DateTime.UtcNow;

        // Act
        var @event = new SaleCancelledDomainEvent(saleId, reason, cancelledAt);

        // Assert
        @event.SaleId.Should().Be(saleId);
        @event.Reason.Should().Be(reason);
        @event.CancelledAt.Should().BeCloseTo(cancelledAt, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void SalePaymentAddedDomainEvent_ShouldContainCorrectData()
    {
        // Arrange
        var sale = new Sale(_saleNumber, _storeId, _customerId, _staffId);
        var paymentAmount = new Money(100.00m, "USD");
        var paymentMethod = PaymentType.Cash;
        var addedAt = DateTime.UtcNow;

        // Act
        var @event = new SalePaymentAddedDomainEvent(sale.Id, paymentMethod, paymentAmount, addedAt);

        // Assert
        @event.SaleId.Should().Be(sale.Id);
        @event.PaymentMethod.Should().Be(paymentMethod);
        @event.Amount.Should().Be(paymentAmount);
        @event.AddedAt.Should().Be(addedAt);
    }

    [Fact]
    public void SaleDiscountAddedDomainEvent_ShouldContainCorrectData()
    {
        // Arrange
        var sale = new Sale(_saleNumber, _storeId, _customerId, _staffId);
        var discountAmount = new Money(10.00m, "USD");
        var discountType = DiscountType.Percentage;
        var reason = "Test discount";
        var addedAt = DateTime.UtcNow;

        // Act
        var @event = new SaleDiscountAddedDomainEvent(sale.Id, discountType, discountAmount, reason, addedAt);

        // Assert
        @event.SaleId.Should().Be(sale.Id);
        @event.DiscountType.Should().Be(discountType);
        @event.Amount.Should().Be(discountAmount);
        @event.Reason.Should().Be(reason);
        @event.AddedAt.Should().Be(addedAt);
    }

    [Fact]
    public void SaleNotesUpdatedDomainEvent_ShouldContainCorrectData()
    {
        // Arrange
        var saleId = Guid.NewGuid();
        var notes = "Test notes";
        var updatedAt = DateTime.UtcNow;

        // Act
        var @event = new SaleNotesUpdatedDomainEvent(saleId, notes, updatedAt);

        // Assert
        @event.SaleId.Should().Be(saleId);
        @event.Notes.Should().Be(notes);
        @event.UpdatedAt.Should().BeCloseTo(updatedAt, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void SaleAddressUpdatedDomainEvent_ShouldContainCorrectData()
    {
        // Arrange
        var saleId = Guid.NewGuid();
        var address = new Address("123 Test St", "Test City", "Test State", "12345", "Test Country");
        var updatedAt = DateTime.UtcNow;

        // Act
        var @event = new SaleAddressUpdatedDomainEvent(saleId, address, updatedAt);

        // Assert
        @event.SaleId.Should().Be(saleId);
        @event.Address.Should().Be(address);
        @event.UpdatedAt.Should().BeCloseTo(updatedAt, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void SaleProcessingStartedDomainEvent_ShouldContainCorrectData()
    {
        // Arrange
        var saleId = Guid.NewGuid();
        var startedAt = DateTime.UtcNow;

        // Act
        var @event = new SaleProcessingStartedDomainEvent(saleId, startedAt);

        // Assert
        @event.SaleId.Should().Be(saleId);
        @event.StartedAt.Should().BeCloseTo(startedAt, TimeSpan.FromSeconds(1));
    }
} 
