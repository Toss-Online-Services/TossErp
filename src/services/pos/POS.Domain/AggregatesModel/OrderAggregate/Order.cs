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
    
    public required string OrderNumber { get; set; }
    public Guid CustomerId { get; private set; }
    public required OrderStatus Status { get; set; }
    public required Money TotalAmount { get; set; }
    public required Money TaxAmount { get; set; }
    public required Money DiscountAmount { get; set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public string? Notes { get; private set; }

    private Order() { } // For EF Core

    public Order(string orderNumber, Guid customerId, string? notes = null)
    {
        OrderNumber = Guard.Against.NullOrWhiteSpace(orderNumber, nameof(orderNumber));
        CustomerId = Guard.Against.Default(customerId, nameof(customerId));
        Status = OrderStatus.Created;
        TotalAmount = Money.FromDecimal(0, "USD"); // Default currency
        TaxAmount = Money.FromDecimal(0, "USD");
        DiscountAmount = Money.FromDecimal(0, "USD");
        CreatedAt = DateTime.UtcNow;
        Notes = notes;

        AddDomainEvent(new OrderCreatedDomainEvent(this));
    }

    public void AddItem(Guid productId, string productName, string productSku, int quantity, Money unitPrice, string? notes = null)
    {
        var item = new OrderItem(productId, productName, productSku, quantity, unitPrice, notes);
        _orderItems.Add(item);
        RecalculateTotals();
        AddDomainEvent(new OrderItemAddedDomainEvent(this, item));
    }

    public void RemoveItem(Guid productId)
    {
        var item = _orderItems.FirstOrDefault(i => i.ProductId == productId);
        if (item == null) return;

        _orderItems.Remove(item);
        RecalculateTotals();
        AddDomainEvent(new OrderItemRemovedDomainEvent(this, productId));
    }

    public void UpdateItemQuantity(Guid productId, int quantity)
    {
        var item = _orderItems.FirstOrDefault(i => i.ProductId == productId);
        if (item == null) return;

        item.UpdateQuantity(quantity);
        RecalculateTotals();
        AddDomainEvent(new OrderItemQuantityUpdatedDomainEvent(this, productId, quantity));
    }

    public void Confirm()
    {
        if (Status != OrderStatus.Created)
            throw new DomainException("Only created orders can be confirmed");

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

    public void Cancel()
    {
        if (Status == OrderStatus.Completed || Status == OrderStatus.Refunded)
            throw new DomainException("Completed or refunded orders cannot be cancelled");

        Status = OrderStatus.Cancelled;
        AddDomainEvent(new OrderCancelledDomainEvent(this));
    }

    public void Refund()
    {
        if (Status != OrderStatus.Completed)
            throw new DomainException("Only completed orders can be refunded");

        Status = OrderStatus.Refunded;
        AddDomainEvent(new OrderRefundedDomainEvent(this));
    }

    public void UpdateNotes(string notes)
    {
        Notes = notes;
        AddDomainEvent(new OrderNotesUpdatedDomainEvent(this));
    }

    private void RecalculateTotals()
    {
        var subtotal = _orderItems.Sum(item => item.TotalPrice.Amount);
        var tax = subtotal * 0.1m; // 10% tax rate
        var discount = 0m; // Calculate discount based on business rules

        TotalAmount = Money.FromDecimal(subtotal + tax - discount, "USD");
        TaxAmount = Money.FromDecimal(tax, "USD");
        DiscountAmount = Money.FromDecimal(discount, "USD");
    }
} 
