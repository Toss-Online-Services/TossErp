#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using POS.Domain.Common;
using TossErp.POS.Domain.Events;
using System.Collections.ObjectModel;

namespace POS.Domain.AggregatesModel.SaleAggregate;

public class Sale : Entity, IAggregateRoot
{
    private readonly List<SaleItem> _items = new();
    private readonly List<Payment> _payments = new();
    private readonly List<SaleDiscount> _discounts = new();
    private readonly List<DomainEvent> _domainEvents = new();

    public string SaleNumber { get; private set; }
    public DateTime SaleDate { get; private set; }
    public decimal TotalAmount { get; private set; }
    public SaleStatus Status { get; private set; }
    public int StoreId { get; private set; }
    public Store Store { get; private set; }
    public int? BuyerId { get; private set; }
    public Buyer Buyer { get; private set; }
    public int StaffId { get; private set; }
    public Staff Staff { get; private set; }
    public string StaffName { get; private set; } = string.Empty;
    public bool IsOffline { get; private set; }
    public DateTime? SyncedAt { get; private set; }
    public string? CustomerId { get; private set; }
    public string CustomerName { get; private set; } = string.Empty;
    public string CustomerPhone { get; private set; } = string.Empty;
    public string CustomerEmail { get; private set; } = string.Empty;
    public string? Note { get; private set; }
    public bool IsRefund { get; private set; }
    public int? RefundedSaleId { get; private set; }
    public string? RefundReason { get; private set; }
    public decimal Total { get; private set; }
    public decimal TipAmount { get; private set; }
    public string? TipStaffId { get; private set; }
    public bool IsSynced { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public DateTime? RefundedAt { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; private set; }
    public string Notes { get; private set; } = string.Empty;
    public DateTime? VoidedAt { get; private set; }

    public IReadOnlyCollection<SaleItem> SaleItems => _items.AsReadOnly();
    public IReadOnlyCollection<Payment> Payments => _payments.AsReadOnly();
    public IReadOnlyCollection<SaleDiscount> Discounts => _discounts.AsReadOnly();

    protected Sale() { }

    public Sale(string saleNumber, DateTime saleDate, int storeId, int? buyerId, int staffId)
    {
        SaleNumber = saleNumber;
        SaleDate = saleDate;
        StoreId = storeId;
        BuyerId = buyerId;
        StaffId = staffId;
        Status = SaleStatus.Draft;
        _items = new List<SaleItem>();
    }

    public void AddSaleItem(int productId, int quantity, decimal unitPrice)
    {
        var saleItem = new SaleItem(Id, productId, quantity, unitPrice);
        _items.Add(saleItem);
        CalculateTotalAmount();
        _domainEvents.Add(new SaleItemAddedDomainEvent(this, saleItem));
    }

    public void RemoveSaleItem(int saleItemId)
    {
        var saleItem = _items.Find(x => x.Id == saleItemId);
        if (saleItem != null)
        {
            _items.Remove(saleItem);
            CalculateTotalAmount();
            _domainEvents.Add(new SaleItemRemovedDomainEvent(this, saleItem));
        }
    }

    public void UpdateStatus(SaleStatus newStatus)
    {
        Status = newStatus;
    }

    public void AddPayment(string method, decimal amount, string? reference = null)
    {
        var payment = new Payment(method, amount, reference);
        _payments.Add(payment);
        _domainEvents.Add(new SalePaymentAddedDomainEvent(this, payment));

        if (GetBalance() <= 0)
        {
            Complete();
        }
    }

    public void AddDiscount(string name, decimal amount, DiscountType type)
    {
        var discount = new SaleDiscount(name, amount, type);
        _discounts.Add(discount);
        _domainEvents.Add(new SaleDiscountAddedDomainEvent(this, discount));
    }

    public void AddTip(decimal amount, string staffId)
    {
        TipAmount = amount;
        TipStaffId = staffId;
    }

    public void SetCustomer(string customerId, string? name, string? phone = null, string? email = null)
    {
        CustomerId = customerId;
        CustomerName = name ?? string.Empty;
        CustomerPhone = phone ?? string.Empty;
        CustomerEmail = email ?? string.Empty;
    }

    public void AddNotes(string? notes)
    {
        Note = notes;
        Notes = notes ?? string.Empty;
    }

    public void MarkAsOffline()
    {
        IsOffline = true;
        SyncedAt = null;
    }

    public void MarkAsSynced()
    {
        IsOffline = false;
        SyncedAt = DateTime.UtcNow;
        _domainEvents.Add(new SaleSyncedDomainEvent(this, SyncedAt.Value));
    }

    public void Complete()
    {
        if (Status != SaleStatus.Pending)
            throw new POSDomainException("Only pending sales can be completed.");

        Status = SaleStatus.Completed;
        CompletedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        _domainEvents.Add(new SaleCompletedDomainEvent(this));
    }

    public void Void(string reason)
    {
        if (Status != SaleStatus.Pending)
            throw new POSDomainException("Only pending sales can be voided.");

        Status = SaleStatus.Voided;
        VoidedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        _domainEvents.Add(new SaleVoidedDomainEvent(this));
    }

    public void Refund(string reason)
    {
        if (Status != SaleStatus.Completed)
            throw new POSDomainException("Only completed sales can be refunded.");

        Status = SaleStatus.Refunded;
        RefundedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        RefundReason = reason;
        _domainEvents.Add(new SaleRefundedDomainEvent(this));
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
