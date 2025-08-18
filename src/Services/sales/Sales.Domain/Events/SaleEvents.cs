using TossErp.Sales.Domain.Enums;
using TossErp.Sales.Domain.ValueObjects;

namespace TossErp.Sales.Domain.Events;

/// <summary>
/// Base class for sale domain events
/// </summary>
public abstract record SaleDomainEvent : IDomainEvent
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public DateTime OccurredOn { get; init; } = DateTime.UtcNow;
    public Guid SaleId { get; init; }

    protected SaleDomainEvent(Guid saleId)
    {
        SaleId = saleId;
    }
}

/// <summary>
/// Event raised when a sale is created
/// </summary>
public record SaleCreatedEvent(Guid SaleId, string ReceiptNumber, Guid TillId, string TenantId) 
    : SaleDomainEvent(SaleId);

/// <summary>
/// Event raised when an item is added to a sale
/// </summary>
public record SaleItemAddedEvent(Guid SaleId, Guid ItemId, decimal Quantity, Money UnitPrice) 
    : SaleDomainEvent(SaleId);

/// <summary>
/// Event raised when an item is removed from a sale
/// </summary>
public record SaleItemRemovedEvent(Guid SaleId, Guid ItemId) 
    : SaleDomainEvent(SaleId);

/// <summary>
/// Event raised when an item quantity is updated
/// </summary>
public record SaleItemQuantityUpdatedEvent(Guid SaleId, Guid ItemId, decimal NewQuantity) 
    : SaleDomainEvent(SaleId);

/// <summary>
/// Event raised when a discount is applied to a sale
/// </summary>
public record SaleDiscountAppliedEvent(Guid SaleId, Money DiscountAmount, string Reason) 
    : SaleDomainEvent(SaleId);

/// <summary>
/// Event raised when a payment is added to a sale
/// </summary>
public record PaymentAddedEvent(Guid SaleId, Guid PaymentId, PaymentMethod Method, Money Amount) 
    : SaleDomainEvent(SaleId);

/// <summary>
/// Event raised when a sale is completed
/// </summary>
public record SaleCompletedEvent(Guid SaleId, Money Total, Money PaidAmount, Money ChangeAmount) 
    : SaleDomainEvent(SaleId);

/// <summary>
/// Event raised when a sale is cancelled
/// </summary>
public record SaleCancelledEvent(Guid SaleId, string Reason) 
    : SaleDomainEvent(SaleId);

/// <summary>
/// Event raised when a sale is put on hold
/// </summary>
public record SalePutOnHoldEvent(Guid SaleId, string Reason) 
    : SaleDomainEvent(SaleId);

/// <summary>
/// Event raised when a sale is resumed from hold
/// </summary>
public record SaleResumedFromHoldEvent(Guid SaleId) 
    : SaleDomainEvent(SaleId);
