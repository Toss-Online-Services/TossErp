using POS.Domain.Events;
using POS.Domain.ValueObjects;
using POS.Domain.AggregatesModel.OrderAggregate.Events;
using POS.Domain.AggregatesModel.ProductAggregate;
using POS.Domain.SeedWork;

namespace POS.Domain.AggregatesModel.OrderAggregate;

/// <summary>
/// Represents an order in the POS system
/// </summary>
public class Order : Entity, IAggregateRoot
{
    private readonly List<OrderItem> _orderItems = new();
    public IReadOnlyCollection<OrderItem> OrderItems => _orderItems.AsReadOnly();
    
    public string OrderNumber { get; private set; }
    public Guid CustomerId { get; private set; }
    public OrderStatus Status { get; private set; }
    public Money TotalAmount { get; private set; }
    public Money TaxAmount { get; private set; }
    public Money DiscountAmount { get; private set; }
    public decimal TaxRate { get; private set; }
    public decimal DiscountPercentage { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public string? Notes { get; private set; }

    // Audit fields
    public DateTime? UpdatedAt { get; private set; }
    public DateTime? DeletedAt { get; private set; }
    public string? CreatedBy { get; private set; }
    public string? UpdatedBy { get; private set; }
    public string? DeletedBy { get; private set; }

    // For EF Core
    private Order() 
    { 
        OrderNumber = string.Empty;
        Status = OrderStatus.Created;
        TotalAmount = new Money(0, "USD");
        TaxAmount = new Money(0, "USD");
        DiscountAmount = new Money(0, "USD");
        TaxRate = 0.1m; // Default 10% tax rate
        DiscountPercentage = 0m; // Default no discount
        CreatedAt = DateTime.UtcNow;
    }

    public Order(string orderNumber, Guid customerId, decimal taxRate = 0.1m, decimal discountPercentage = 0m, string? notes = null, string? createdBy = null)
    {
        OrderNumber = Guard.Against.NullOrWhiteSpace(orderNumber, nameof(orderNumber));
        CustomerId = Guard.Against.Default(customerId, nameof(customerId));
        Status = OrderStatus.Created;
        TotalAmount = new Money(0, "USD"); // Default currency
        TaxAmount = new Money(0, "USD");
        DiscountAmount = new Money(0, "USD");
        TaxRate = Guard.Against.Negative(taxRate, nameof(taxRate));
        DiscountPercentage = Guard.Against.Negative(discountPercentage, nameof(discountPercentage));
        CreatedAt = DateTime.UtcNow;
        Notes = notes;
        CreatedBy = createdBy;

        AddDomainEvent(new OrderCreatedDomainEvent(this));
    }

    public void AddItem(Guid productId, string productName, string productSku, int quantity, Money unitPrice, string? notes = null)
    {
        Guard.Against.Default(productId, nameof(productId));
        Guard.Against.NullOrWhiteSpace(productName, nameof(productName));
        Guard.Against.NullOrWhiteSpace(productSku, nameof(productSku));
        Guard.Against.NegativeOrZero(quantity, nameof(quantity));
        Guard.Against.Null(unitPrice, nameof(unitPrice));

        if (Status != OrderStatus.Created && Status != OrderStatus.Draft)
            throw new DomainException("Can only add items to created or draft orders");

        var item = new OrderItem(productId, productName, productSku, quantity, unitPrice, notes);
        _orderItems.Add(item);
        RecalculateTotals();
        AddDomainEvent(new OrderItemAddedDomainEvent(this, item));
    }

    public void RemoveItem(Guid productId)
    {
        Guard.Against.Default(productId, nameof(productId));

        if (Status != OrderStatus.Created && Status != OrderStatus.Draft)
            throw new DomainException("Can only remove items from created or draft orders");

        var item = _orderItems.FirstOrDefault(i => i.ProductId == productId);
        if (item == null) return;

        _orderItems.Remove(item);
        RecalculateTotals();
        AddDomainEvent(new OrderItemRemovedDomainEvent(this, productId));
    }

    public void UpdateItemQuantity(Guid productId, int quantity)
    {
        Guard.Against.Default(productId, nameof(productId));
        Guard.Against.NegativeOrZero(quantity, nameof(quantity));

        if (Status != OrderStatus.Created && Status != OrderStatus.Draft)
            throw new DomainException("Can only update quantities in created or draft orders");

        var item = _orderItems.FirstOrDefault(i => i.ProductId == productId);
        if (item == null) return;

        item.UpdateQuantity(quantity);
        RecalculateTotals();
        AddDomainEvent(new OrderItemQuantityUpdatedDomainEvent(this, productId, quantity));
    }

    public void Confirm()
    {
        if (Status != OrderStatus.Created && Status != OrderStatus.Draft)
            throw new DomainException("Only created or draft orders can be confirmed");

        if (_orderItems.Count == 0)
            throw new DomainException("Cannot confirm an order with no items");

        Status = OrderStatus.Confirmed;
        AddDomainEvent(new OrderConfirmedDomainEvent(this));
    }

    public void Process()
    {
        if (Status != OrderStatus.Confirmed)
            throw new DomainException("Only confirmed orders can be processed");

        Status = OrderStatus.Processing;
        AddDomainEvent(new OrderProcessingDomainEvent(this));
    }

    public void Complete()
    {
        if (Status != OrderStatus.Processing)
            throw new DomainException("Only processing orders can be completed");

        Status = OrderStatus.Completed;
        CompletedAt = DateTime.UtcNow;
        AddDomainEvent(new OrderCompletedDomainEvent(this));
    }

    public void Cancel(string? reason = null, string? cancelledBy = null)
    {
        if (Status == OrderStatus.Completed || Status == OrderStatus.Refunded)
            throw new DomainException("Completed or refunded orders cannot be cancelled");

        Status = OrderStatus.Cancelled;
        Notes = reason;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = cancelledBy;
        AddDomainEvent(new OrderCancelledDomainEvent(this, reason));
    }

    public void Refund(string? reason = null, string? refundedBy = null)
    {
        if (Status != OrderStatus.Completed)
            throw new DomainException("Only completed orders can be refunded");

        Status = OrderStatus.Refunded;
        Notes = reason;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = refundedBy;
        AddDomainEvent(new OrderRefundedDomainEvent(this, reason));
    }

    public void UpdateNotes(string? notes, string? updatedBy = null)
    {
        Notes = notes;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
        AddDomainEvent(new OrderNotesUpdatedDomainEvent(this));
    }

    public void Delete(string? deletedBy = null)
    {
        if (Status != OrderStatus.Cancelled && Status != OrderStatus.Refunded)
            throw new DomainException("Only cancelled or refunded orders can be deleted");

        DeletedAt = DateTime.UtcNow;
        DeletedBy = deletedBy;
        AddDomainEvent(new OrderDeletedDomainEvent(this));
    }

    private void RecalculateTotals()
    {
        var subtotal = _orderItems.Sum(item => item.TotalPrice.Amount);
        
        // Calculate tax using the configured rate
        var tax = subtotal * TaxRate;
        
        // Calculate discount using the configured percentage
        var discount = subtotal * DiscountPercentage;
        
        // Ensure currency consistency across all amounts
        var currency = _orderItems.FirstOrDefault()?.UnitPrice.Currency ?? "USD";
        
        TotalAmount = new Money(subtotal + tax - discount, currency);
        TaxAmount = new Money(tax, currency);
        DiscountAmount = new Money(discount, currency);
    }

    public void UpdateTaxRate(decimal newTaxRate, string? updatedBy = null)
    {
        Guard.Against.Negative(newTaxRate, nameof(newTaxRate));
        
        if (Status != OrderStatus.Created && Status != OrderStatus.Draft)
            throw new DomainException("Can only update tax rate for created or draft orders");

        TaxRate = newTaxRate;
        RecalculateTotals();
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
        AddDomainEvent(new OrderTaxRateUpdatedDomainEvent(this, newTaxRate));
    }

    public void UpdateDiscountPercentage(decimal newDiscountPercentage, string? updatedBy = null)
    {
        Guard.Against.Negative(newDiscountPercentage, nameof(newDiscountPercentage));
        
        if (Status != OrderStatus.Created && Status != OrderStatus.Draft)
            throw new DomainException("Can only update discount percentage for created or draft orders");

        DiscountPercentage = newDiscountPercentage;
        RecalculateTotals();
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
        AddDomainEvent(new OrderDiscountPercentageUpdatedDomainEvent(this, newDiscountPercentage));
    }
} 
