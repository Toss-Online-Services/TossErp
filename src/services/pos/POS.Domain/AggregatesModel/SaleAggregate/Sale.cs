#nullable enable
using eShop.POS.Domain.Common;
using eShop.POS.Domain.AggregatesModel.SaleAggregate.Events;
using POS.Domain.SeedWork;

namespace eShop.POS.Domain.AggregatesModel.SaleAggregate;

public class Sale : Entity, IAggregateRoot
{
    private readonly List<SaleItem> _items;
    private readonly List<Payment> _payments;
    private readonly List<SaleDiscount> _discounts;
    private readonly List<DomainEvent> _domainEvents = new();

    public string StoreId { get; private set; }
    public string StaffId { get; private set; }
    public string StaffName { get; private set; }
    public DateTime SaleDate { get; private set; }
    public SaleStatus Status { get; private set; }
    public bool IsOffline { get; private set; }
    public DateTime? SyncedAt { get; private set; }
    public string? CustomerId { get; private set; }
    public string CustomerName { get; private set; }
    public string CustomerPhone { get; private set; }
    public string CustomerEmail { get; private set; }
    public string? Note { get; private set; }
    public bool IsRefund { get; private set; }
    public int? RefundedSaleId { get; private set; }
    public string RefundReason { get; private set; }
    public decimal Total { get; private set; }
    public decimal TipAmount { get; private set; }
    public string? TipStaffId { get; private set; }
    public bool IsSynced { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public DateTime? RefundedAt { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public string Notes { get; private set; }
    public decimal TotalAmount { get; private set; }

    public IReadOnlyCollection<SaleItem> Items => _items.AsReadOnly();
    public IReadOnlyCollection<Payment> Payments => _payments.AsReadOnly();
    public IReadOnlyCollection<SaleDiscount> Discounts => _discounts.AsReadOnly();

    protected Sale()
    {
        _items = new List<SaleItem>();
        _payments = new List<Payment>();
        _discounts = new List<SaleDiscount>();
        Status = SaleStatus.Draft;
        SaleDate = DateTime.UtcNow;
        CreatedAt = DateTime.UtcNow;
        StoreId = string.Empty;
        StaffId = string.Empty;
        StaffName = string.Empty;
        CustomerName = string.Empty;
        CustomerPhone = string.Empty;
        CustomerEmail = string.Empty;
        RefundReason = string.Empty;
        Notes = string.Empty;
    }

    public Sale(string storeId, string staffId, string staffName) : this()
    {
        StoreId = storeId;
        StaffId = staffId;
        StaffName = staffName;
    }

    public void AddItem(string productId, string productName, decimal unitPrice, int quantity)
    {
        var item = new SaleItem(productId, productName, unitPrice, quantity);
        _items.Add(item);
        RecalculateTotal();
    }

    public void AddPayment(PaymentMethod method, decimal amount, string reference = "", 
        string cardLast4 = "", string cardType = "")
    {
        var payment = new Payment(method, amount, reference, cardLast4, cardType);
        _payments.Add(payment);
        _domainEvents.Add(new SalePaymentAddedDomainEvent(this, payment));
    }

    public void AddDiscount(decimal amount, string reason, DiscountType type = DiscountType.Manual, string staffId = "")
    {
        var discount = new SaleDiscount(amount, reason, type, staffId);
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

    public void MarkAsCompleted()
    {
        if (Status != SaleStatus.Draft)
            throw new POSDomainException("Only draft sales can be completed.");

        Status = SaleStatus.Completed;
        CompletedAt = DateTime.UtcNow;
        _domainEvents.Add(new SaleCompletedDomainEvent(this));
    }

    public void MarkAsRefunded(int originalSaleId, string? reason)
    {
        if (Status != SaleStatus.Completed)
            throw new POSDomainException("Only completed sales can be refunded.");

        IsRefund = true;
        RefundedSaleId = originalSaleId;
        RefundReason = reason ?? string.Empty;
        Status = SaleStatus.Refunded;
        RefundedAt = DateTime.UtcNow;
        _domainEvents.Add(new SaleRefundedDomainEvent(this));
    }

    public void MarkAsVoided(string? reason)
    {
        if (Status != SaleStatus.Draft)
            throw new POSDomainException("Only draft sales can be voided.");

        Status = SaleStatus.Voided;
        Note = reason;
        _domainEvents.Add(new SaleVoidedDomainEvent(this));
    }

    public decimal GetSubtotal() => _items.Sum(i => i.Total);
    public decimal GetDiscountTotal() => _discounts.Where(d => d.Type != DiscountType.Tip).Sum(d => d.Amount);
    public decimal GetTipTotal() => _discounts.Where(d => d.Type == DiscountType.Tip).Sum(d => d.Amount);
    public decimal GetTotal() => GetSubtotal() - GetDiscountTotal() + GetTipTotal();
    public decimal GetPaymentTotal() => _payments.Sum(p => p.Amount);
    public decimal GetBalance() => GetTotal() - GetPaymentTotal();

    private void RecalculateTotal()
    {
        Total = GetSubtotal() - GetDiscountTotal() + GetTipTotal();
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
