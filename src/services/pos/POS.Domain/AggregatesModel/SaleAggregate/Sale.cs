#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using TossErp.POS.Domain.SeedWork;
using TossErp.POS.Domain.Events;
using System.Collections.ObjectModel;
using TossErp.POS.Domain.AggregatesModel.BuyerAggregate;
using TossErp.POS.Domain.AggregatesModel.SaleAggregate.Events;
using TossErp.POS.Domain.Exceptions;

namespace TossErp.POS.Domain.AggregatesModel.SaleAggregate;

public class Sale : AggregateRoot
{
    private readonly List<SaleItem> _items = new();
    private readonly List<Payment> _payments = new();
    private readonly List<SaleDiscount> _discounts = new();
    private readonly List<DomainEvent> _domainEvents = new();

    public string InvoiceNumber { get; private set; }
    public DateTime SaleDate { get; private set; }
    public decimal SubTotal { get; private set; }
    public decimal TaxAmount { get; private set; }
    public decimal DiscountAmount { get; private set; }
    public decimal TotalAmount { get; private set; }
    public decimal AmountPaid { get; private set; }
    public decimal Balance { get; private set; }
    public SaleStatus Status { get; private set; }
    public string? Notes { get; private set; }
    public bool IsSynced { get; private set; }
    public DateTime? SyncedAt { get; private set; }
    public int? StaffId { get; private set; }
    public Staff? Staff { get; private set; }
    public int? BuyerId { get; private set; }
    public Buyer? Buyer { get; private set; }
    public int? PaymentMethodId { get; private set; }
    public PaymentMethod? PaymentMethod { get; private set; }
    public int? AddressId { get; private set; }
    public Address? Address { get; private set; }
    public int? CardTypeId { get; private set; }
    public CardType? CardType { get; private set; }

    public IReadOnlyCollection<SaleItem> Items => _items.AsReadOnly();
    public IReadOnlyCollection<Payment> Payments => _payments.AsReadOnly();
    public IReadOnlyCollection<SaleDiscount> Discounts => _discounts.AsReadOnly();

    protected Sale()
    {
        InvoiceNumber = string.Empty;
        Status = SaleStatus.Pending;
        _items = new List<SaleItem>();
        _payments = new List<Payment>();
        _discounts = new List<SaleDiscount>();
    }

    public Sale(string invoiceNumber, DateTime saleDate, int? staffId = null, int? buyerId = null, 
        int? paymentMethodId = null, int? addressId = null, int? cardTypeId = null, string? notes = null)
    {
        if (string.IsNullOrWhiteSpace(invoiceNumber))
            throw new DomainException("Invoice number cannot be empty");

        _items = new List<SaleItem>();
        _payments = new List<Payment>();
        _discounts = new List<SaleDiscount>();

        InvoiceNumber = invoiceNumber;
        SaleDate = saleDate;
        StaffId = staffId;
        BuyerId = buyerId;
        PaymentMethodId = paymentMethodId;
        AddressId = addressId;
        CardTypeId = cardTypeId;
        Notes = notes;
        Status = SaleStatus.Pending;
        IsSynced = false;
        SyncedAt = null;
    }

    public void AddItem(int productId, string productName, decimal unitPrice, int quantity, decimal discount = 0)
    {
        var item = new SaleItem(Id, productId, productName, unitPrice, quantity, discount);
        _items.Add(item);
        CalculateTotalAmount();
        AddDomainEvent(new SaleItemAddedDomainEvent(this, item));
    }

    public void RemoveItem(int itemId)
    {
        var item = _items.Find(i => i.Id == itemId);
        if (item != null)
        {
            _items.Remove(item);
            CalculateTotalAmount();
            AddDomainEvent(new SaleItemRemovedDomainEvent(this, item));
        }
    }

    public void AddPayment(decimal amount, string? reference = null)
    {
        if (amount <= 0)
            throw new DomainException("Payment amount must be greater than zero");

        var payment = new Payment(Id, amount, reference);
        _payments.Add(payment);
        AmountPaid += amount;
        Balance = TotalAmount - AmountPaid;
        AddDomainEvent(new SalePaymentAddedDomainEvent(this, payment));

        if (GetBalance() <= 0)
        {
            Complete();
        }
    }

    public void AddDiscount(string name, decimal amount, DiscountType type)
    {
        var discount = new SaleDiscount(Id, name, amount, type);
        _discounts.Add(discount);
        AddDomainEvent(new SaleDiscountAddedDomainEvent(this, discount));
    }

    public void SetCustomer(string customerId, string? name, string? phone = null, string? email = null)
    {
        BuyerId = customerId;
        Buyer = new Buyer(customerId, name, phone, email);
    }

    public void AddNotes(string? notes)
    {
        Notes = notes;
    }

    public void MarkAsOffline()
    {
        IsSynced = false;
        SyncedAt = null;
    }

    public void MarkAsSynced()
    {
        if (IsSynced)
            throw new DomainException("Sale is already synced");

        IsSynced = true;
        SyncedAt = DateTime.UtcNow;
        AddDomainEvent(new SaleSyncedDomainEvent(this, SyncedAt.Value));
    }

    public void Complete()
    {
        if (Status != SaleStatus.Pending)
            throw new DomainException("Only pending sales can be completed");

        if (!_items.Any())
            throw new DomainException("Cannot complete sale without items");

        if (AmountPaid < TotalAmount)
            throw new DomainException("Cannot complete sale without full payment");

        Status = SaleStatus.Completed;
        AddDomainEvent(new SaleCompletedDomainEvent(this));
    }

    public void Void()
    {
        if (Status != SaleStatus.Pending)
            throw new DomainException("Only pending sales can be voided");

        Status = SaleStatus.Voided;
        AddDomainEvent(new SaleVoidedDomainEvent(this));
    }

    public void Refund(decimal amount)
    {
        if (Status != SaleStatus.Completed)
            throw new DomainException("Only completed sales can be refunded");

        if (amount <= 0)
            throw new DomainException("Refund amount must be greater than zero");

        if (amount > TotalAmount)
            throw new DomainException("Refund amount cannot exceed total amount");

        Status = SaleStatus.Refunded;
        AddDomainEvent(new SaleRefundedDomainEvent(this));
    }

    public decimal CalculateTotal()
    {
        decimal total = _items.Sum(item => item.TotalPrice);
        decimal discountTotal = _discounts.Sum(discount => discount.CalculateDiscountAmount(total));
        decimal taxTotal = (total - discountTotal) * 0.1m; // Assuming 10% tax
        return total - discountTotal + taxTotal;
    }

    public decimal GetSubtotal() => _items.Sum(i => i.UnitPrice * i.Quantity);
    public decimal GetDiscountTotal() => _discounts.Where(d => d.Type != DiscountType.Tip).Sum(d => d.Amount);
    public decimal GetTipTotal() => _discounts.Where(d => d.Type == DiscountType.Tip).Sum(d => d.Amount);
    public decimal GetTotal() => GetSubtotal() - GetDiscountTotal() + GetTipTotal();
    public decimal GetPaymentTotal() => _payments.Sum(p => p.Amount);
    public decimal GetBalance() => GetTotal() - GetPaymentTotal();

    private void CalculateTotalAmount()
    {
        TotalAmount = 0;
        foreach (var item in _items)
        {
            TotalAmount += item.TotalPrice;
        }
    }

    public IReadOnlyCollection<DomainEvent> GetDomainEvents()
    {
        return _domainEvents.AsReadOnly();
    }

    public new void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
} 

