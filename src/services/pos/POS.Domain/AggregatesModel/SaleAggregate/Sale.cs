#nullable enable
using POS.Domain.AggregatesModel.StaffAggregate;
using POS.Domain.SeedWork;
using POS.Domain.AggregatesModel.SaleAggregate.Events;
using POS.Domain.Exceptions;
using POS.Domain.AggregatesModel.StoreAggregate;
using POS.Domain.Enums;
using POS.Domain.Models;
using PaymentMethodModel = POS.Domain.Models.PaymentMethod;
using POS.Domain.Common.ValueObjects;
using POS.Domain.Events;
using POS.Domain.Common;
using POS.Domain.Common.Events;
using POS.Domain.Exceptions;
using POS.Domain.SeedWork;

namespace POS.Domain.AggregatesModel.SaleAggregate;

public class Sale : AggregateRoot
{
    public string SaleNumber { get; private set; }
    public Guid StoreId { get; private set; }
    public Guid? CustomerId { get; private set; }
    public Guid? StaffId { get; private set; }
    public decimal Subtotal { get; private set; }
    public decimal Tax { get; private set; }
    public decimal Discount { get; private set; }
    public decimal Total { get; private set; }
    public string Status { get; private set; }
    public string? Notes { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public DateTime? CancelledAt { get; private set; }
    public string? CancellationReason { get; private set; }
    public Common.ValueObjects.Address? Address { get; private set; }

    private readonly List<SaleItem> _items = new();
    public IReadOnlyCollection<SaleItem> Items => _items.AsReadOnly();

    private readonly List<Payment> _payments = new();
    public IReadOnlyCollection<Payment> Payments => _payments.AsReadOnly();

    private readonly List<SaleDiscount> _discounts = new();
    public IReadOnlyCollection<SaleDiscount> Discounts => _discounts.AsReadOnly();

    private Sale()
    {
        SaleNumber = string.Empty;
        Status = string.Empty;
    }

    public Sale(string saleNumber, Guid storeId, Guid? customerId = null, Guid? staffId = null)
    {
        if (string.IsNullOrWhiteSpace(saleNumber))
            throw new DomainException("Sale number cannot be empty");
        if (storeId == Guid.Empty)
            throw new DomainException("Store ID cannot be empty");

        SaleNumber = saleNumber;
        StoreId = storeId;
        CustomerId = customerId;
        StaffId = staffId;
        Status = "Pending";
        CreatedAt = DateTime.UtcNow;
    }

    public void AddItem(SaleItem item)
    {
        if (Status != "Pending")
            throw new DomainException("Cannot add items to a non-pending sale");

        var existingItem = _items.FirstOrDefault(i => i.ProductId == item.ProductId);
        if (existingItem != null)
        {
            existingItem.UpdateQuantity(existingItem.Quantity + item.Quantity);
        }
        else
        {
            _items.Add(item);
        }

        RecalculateTotals();
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateItemQuantity(Guid productId, int quantity)
    {
        if (Status != "Pending")
            throw new DomainException("Cannot update items in a non-pending sale");

        var item = _items.FirstOrDefault(i => i.ProductId == productId);
        if (item == null)
            throw new DomainException($"Item with product ID {productId} not found");

        item.UpdateQuantity(quantity);
        RecalculateTotals();
        UpdatedAt = DateTime.UtcNow;
    }

    public void RemoveItem(Guid productId)
    {
        if (Status != "Pending")
            throw new DomainException("Cannot remove items from a non-pending sale");

        var item = _items.FirstOrDefault(i => i.ProductId == productId);
        if (item == null)
            throw new DomainException($"Item with product ID {productId} not found");

        _items.Remove(item);
        RecalculateTotals();
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddPayment(Payment payment)
    {
        if (Status != "Pending" && Status != "Processing")
            throw new DomainException("Cannot add payment to a completed or cancelled sale");

        _payments.Add(payment);
        RecalculateTotals();
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new PaymentCreatedDomainEvent(payment.Id, Id, payment.Amount, payment.Method.ToString()));
    }

    public void AddDiscount(SaleDiscount discount)
    {
        if (Status != "Pending")
            throw new DomainException("Cannot add discount to a non-pending sale");

        _discounts.Add(discount);
        RecalculateTotals();
        UpdatedAt = DateTime.UtcNow;
    }

    public void RemoveDiscount(Guid discountId)
    {
        if (Status != "Pending")
            throw new DomainException("Cannot remove discount from a non-pending sale");

        var discount = _discounts.FirstOrDefault(d => d.Id == discountId);
        if (discount == null)
            throw new DomainException($"Discount with ID {discountId} not found");

        _discounts.Remove(discount);
        RecalculateTotals();
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateCustomer(Guid? customerId)
    {
        if (Status != "Pending")
            throw new DomainException("Cannot update customer for a non-pending sale");

        CustomerId = customerId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateStaff(Guid? staffId)
    {
        if (Status != "Pending")
            throw new DomainException("Cannot update staff for a non-pending sale");

        StaffId = staffId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateAddress(Common.ValueObjects.Address? address)
    {
        if (Status != "Pending")
            throw new DomainException("Cannot update address for a non-pending sale");

        Address = address;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateNotes(string? notes)
    {
        if (Status != "Pending")
            throw new DomainException("Cannot update notes for a non-pending sale");

        Notes = notes;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Complete()
    {
        if (Status != "Pending" && Status != "Processing")
            throw new DomainException("Cannot complete a non-pending or non-processing sale");

        if (_items.Count == 0)
            throw new DomainException("Cannot complete a sale without items");

        if (_payments.Count == 0)
            throw new DomainException("Cannot complete a sale without payments");

        var totalPaid = _payments.Sum(p => p.Amount);
        if (totalPaid < Total)
            throw new DomainException("Total paid amount is less than the sale total");

        Status = "Completed";
        CompletedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new SaleCompletedDomainEvent(Id, Total, CompletedAt.Value));
    }

    public void Cancel(string reason)
    {
        if (Status != "Pending" && Status != "Processing")
            throw new DomainException("Cannot cancel a completed or already cancelled sale");

        if (string.IsNullOrWhiteSpace(reason))
            throw new DomainException("Cancellation reason cannot be empty");

        Status = "Cancelled";
        CancelledAt = DateTime.UtcNow;
        CancellationReason = reason;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new SaleCancelledDomainEvent(Id, reason, CancelledAt.Value));
    }

    private void RecalculateTotals()
    {
        Subtotal = _items.Sum(i => i.Total);
        Discount = _discounts.Sum(d => d.Amount);
        Tax = (Subtotal - Discount) * 0.1m; // 10% tax rate
        Total = Subtotal - Discount + Tax;

        if (Total < 0)
            Total = 0;
    }
}

public enum SaleStatus
{
    Pending,
    Completed,
    Voided,
    Refunded
}

