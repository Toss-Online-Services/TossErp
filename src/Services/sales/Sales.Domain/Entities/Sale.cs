using TossErp.Sales.Domain.Common;
using TossErp.Sales.Domain.Enums;
using TossErp.Sales.Domain.ValueObjects;
using TossErp.Sales.Domain.Events;

namespace TossErp.Sales.Domain.Entities;

/// <summary>
/// Sale aggregate root representing a complete sales transaction
/// </summary>
public class Sale : Entity<Guid>
{
    private readonly List<SaleItem> _items = new();
    private readonly List<Payment> _payments = new();

    public ReceiptNumber ReceiptNumber { get; private set; } = null!;
    public Guid TillId { get; private set; }
    public Guid? CustomerId { get; private set; }
    public string CustomerName { get; private set; } = string.Empty;
    public SaleStatus Status { get; private set; }
    public Money SubTotal { get; private set; } = Money.Zero();
    public Money TaxAmount { get; private set; } = Money.Zero();
    public Money DiscountAmount { get; private set; } = Money.Zero();
    public Money Total { get; private set; } = Money.Zero();
    public Money PaidAmount { get; private set; } = Money.Zero();
    public Money ChangeAmount { get; private set; } = Money.Zero();
    public string? Notes { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public DateTime? CancelledAt { get; private set; }
    public string? CancellationReason { get; private set; }

    // Navigation properties
    public IReadOnlyList<SaleItem> Items => _items.AsReadOnly();
    public IReadOnlyList<Payment> Payments => _payments.AsReadOnly();

    // Domain events
    private readonly List<IDomainEvent> _domainEvents = new();
    public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected Sale() : base() { } // For EF Core

    public Sale(Guid id, ReceiptNumber receiptNumber, Guid tillId, string tenantId, string createdBy) : base(id, tenantId)
    {
        ReceiptNumber = receiptNumber ?? throw new ArgumentNullException(nameof(receiptNumber));
        TillId = tillId;
        Status = SaleStatus.Pending;
        CreatedBy = createdBy ?? throw new ArgumentNullException(nameof(createdBy));

        AddDomainEvent(new SaleCreatedEvent(Id, ReceiptNumber.Value, TillId, tenantId));
    }

    public static Sale Create(Guid tillId, Guid? customerId, string customerName, string tenantId, ReceiptNumber? receiptNumber = null, string createdBy = "system")
    {
        var id = Guid.NewGuid();
        var rn = receiptNumber ?? new ReceiptNumber("RC-00000001");
        var sale = new Sale(id, rn, tillId, tenantId, createdBy);
        sale.SetCustomer(customerId, customerName);
        return sale;
    }

    /// <summary>
    /// Set customer information for the sale
    /// </summary>
    public void SetCustomer(Guid? customerId, string customerName)
    {
        if (Status != SaleStatus.Pending)
            throw new InvalidOperationException("Cannot modify customer information for a non-pending sale");

        CustomerId = customerId;
        CustomerName = customerName ?? string.Empty;
    }

    /// <summary>
    /// Add an item to the sale
    /// </summary>
    public void AddItem(Guid itemId, string itemName, string itemSku, decimal quantity, Money unitPrice, decimal taxRate = 0.15m)
    {
        if (Status != SaleStatus.Pending)
            throw new InvalidOperationException("Cannot add items to a non-pending sale");

        if (quantity <= 0)
            throw new InvalidOperationException("Quantity must be greater than zero");

        if (unitPrice.Amount < 0)
            throw new ArgumentException("Unit price cannot be negative", nameof(unitPrice));

        var existingItem = _items.FirstOrDefault(i => i.ItemId == itemId);
        if (existingItem != null)
        {
            // Update existing item quantity
            existingItem.UpdateQuantity(existingItem.Quantity + quantity);
        }
        else
        {
            // Add new item
            var saleItem = new SaleItem(Guid.NewGuid(), itemId, itemName, itemSku, quantity, unitPrice, taxRate);
            saleItem.SetTenantId(TenantId);
            _items.Add(saleItem);
        }

        RecalculateTotals();
        AddDomainEvent(new SaleItemAddedEvent(Id, itemId, quantity, unitPrice));
    }

    /// <summary>
    /// Remove an item from the sale
    /// </summary>
    public void RemoveItem(Guid itemId)
    {
        if (Status != SaleStatus.Pending)
            throw new InvalidOperationException("Cannot remove items from a non-pending sale");

        var item = _items.FirstOrDefault(i => i.ItemId == itemId);
        if (item != null)
        {
            _items.Remove(item);
            RecalculateTotals();
            AddDomainEvent(new SaleItemRemovedEvent(Id, itemId));
        }
    }

    /// <summary>
    /// Update item quantity
    /// </summary>
    public void UpdateItemQuantity(Guid itemId, decimal newQuantity)
    {
        if (Status != SaleStatus.Pending)
            throw new InvalidOperationException("Cannot update item quantity for a non-pending sale");

        if (newQuantity <= 0)
            throw new ArgumentException("Quantity must be positive", nameof(newQuantity));

        var item = _items.FirstOrDefault(i => i.ItemId == itemId);
        if (item != null)
        {
            item.UpdateQuantity(newQuantity);
            RecalculateTotals();
            AddDomainEvent(new SaleItemQuantityUpdatedEvent(Id, itemId, newQuantity));
        }
    }

    /// <summary>
    /// Apply discount to the sale
    /// </summary>
    public void ApplyDiscount(Money discountAmount, string reason = "")
    {
        if (Status != SaleStatus.Pending)
            throw new InvalidOperationException("Cannot apply discount to a non-pending sale");

        if (discountAmount.Amount < 0)
            throw new ArgumentException("Discount amount cannot be negative", nameof(discountAmount));

        if (discountAmount > SubTotal)
            throw new ArgumentException("Discount cannot be greater than subtotal", nameof(discountAmount));

        DiscountAmount = discountAmount;
        DiscountReason = reason;
        RecalculateTotals();
        AddDomainEvent(new SaleDiscountAppliedEvent(Id, discountAmount, reason));
    }

    /// <summary>
    /// Add payment to the sale
    /// </summary>
    public void AddPayment(PaymentMethod method, Money amount, string? reference = null)
    {
        if (Status != SaleStatus.Pending && Status != SaleStatus.PartiallyPaid)
            throw new InvalidOperationException($"Cannot add payment to sale with status {Status}");

        if (amount.Amount <= 0)
            throw new ArgumentException("Payment amount must be positive", nameof(amount));

        var payment = new Payment(Guid.NewGuid(), method, amount, reference);
        payment.SetTenantId(TenantId);
        _payments.Add(payment);

        PaidAmount += amount;

        // Update sale status based on payment
        if (PaidAmount >= Total)
        {
            ChangeAmount = PaidAmount - Total;
            Status = SaleStatus.Completed;
            CompletedAt = DateTime.UtcNow;
            AddDomainEvent(new SaleCompletedEvent(Id, Total, PaidAmount, ChangeAmount));
        }
        else
        {
            Status = SaleStatus.PartiallyPaid;
        }

        AddDomainEvent(new PaymentAddedEvent(Id, payment.Id, method, amount));
    }

    /// <summary>
    /// Complete the sale (if fully paid)
    /// </summary>
    public void Complete(string completedBy)
    {
        if (Status != SaleStatus.Completed)
            throw new InvalidOperationException($"Cannot complete sale with status {Status}");

        if (!_items.Any())
            throw new InvalidOperationException("Cannot complete sale without items");

        MarkAsUpdated(completedBy);
        AddDomainEvent(new SaleCompletedEvent(Id, Total, PaidAmount, ChangeAmount));
    }

    /// <summary>
    /// Cancel the sale
    /// </summary>
    public void Cancel(string reason, string cancelledBy)
    {
        if (Status == SaleStatus.Cancelled)
            throw new InvalidOperationException("Sale is already cancelled");

        if (Status == SaleStatus.Completed)
            throw new InvalidOperationException("Cannot cancel a completed sale");

        Status = SaleStatus.Cancelled;
        CancelledAt = DateTime.UtcNow;
        CancellationReason = reason;
        MarkAsUpdated(cancelledBy);

        AddDomainEvent(new SaleCancelledEvent(Id, reason));
    }

    /// <summary>
    /// Put sale on hold
    /// </summary>
    public void PutOnHold(string reason, string updatedBy)
    {
        if (Status != SaleStatus.Pending)
            throw new InvalidOperationException("Can only put pending sales on hold");

        Status = SaleStatus.OnHold;
        Notes = reason;
        MarkAsUpdated(updatedBy);

        AddDomainEvent(new SalePutOnHoldEvent(Id, reason));
    }

    /// <summary>
    /// Resume sale from hold
    /// </summary>
    public void ResumeFromHold(string updatedBy)
    {
        if (Status != SaleStatus.OnHold)
            throw new InvalidOperationException("Sale is not on hold");

        Status = SaleStatus.Pending;
        MarkAsUpdated(updatedBy);

        AddDomainEvent(new SaleResumedFromHoldEvent(Id));
    }

    /// <summary>
    /// Add notes to the sale
    /// </summary>
    public void AddNotes(string notes, string updatedBy)
    {
        Notes = notes;
        MarkAsUpdated(updatedBy);
    }

    /// <summary>
    /// Recalculate sale totals based on items, tax, and discount
    /// </summary>
    private void RecalculateTotals()
    {
        SubTotal = _items.Aggregate(Money.Zero(), (sum, item) => sum + item.LineTotal);
        TaxAmount = _items.Aggregate(Money.Zero(), (sum, item) => sum + item.TaxAmount);
        Total = SubTotal + TaxAmount - DiscountAmount;
    }

    /// <summary>
    /// Add domain event
    /// </summary>
    private void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    /// <summary>
    /// Clear domain events (typically called after publishing)
    /// </summary>
    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    public Money TotalAmount => Total;
    public string? DiscountReason { get; private set; }

    /// <summary>
    /// Apply a discount amount to the sale
    /// </summary>
    public void ApplyDiscount(decimal amount, string reason)
    {
        ApplyDiscount(new Money(amount), reason);
    }

    /// <summary>
    /// Complete the sale (parameterless overload for tests)
    /// </summary>
    public void Complete()
    {
        if (!_items.Any())
            throw new InvalidOperationException("Cannot complete sale without items");
        if (!_payments.Any())
            throw new InvalidOperationException("Cannot complete sale without payments");
        Status = SaleStatus.Completed;
        CompletedAt = DateTime.UtcNow;
        AddDomainEvent(new SaleCompletedEvent(Id, Total, PaidAmount, ChangeAmount));
    }

    /// <summary>
    /// Cancel the sale (parameterless user overload)
    /// </summary>
    public void Cancel(string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
            throw new InvalidOperationException("Cancellation reason is required");
        Cancel(reason, "system");
    }
}
