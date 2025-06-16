#nullable enable
using POS.Domain.AggregatesModel.StaffAggregate;
using POS.Domain.SeedWork;
using POS.Domain.AggregatesModel.BuyerAggregate;
using POS.Domain.AggregatesModel.SaleAggregate.Events;
using POS.Domain.Exceptions;
using POS.Domain.AggregatesModel.StoreAggregate;
using POS.Domain.Enums;
using POS.Domain.Models;
using PaymentMethodModel = POS.Domain.Models.PaymentMethod;

namespace POS.Domain.AggregatesModel.SaleAggregate;

public class Sale : AggregateRoot
{
    private readonly List<SaleItem> _items = new();
    private readonly List<Payment> _payments = new();
    private readonly List<SaleDiscount> _discounts = new();

    public Guid StoreId { get; private set; }
    public Store Store { get; private set; } = null!;
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
    public Guid? StaffId { get; private set; }
    public Staff? Staff { get; private set; }
    public string? StaffName { get; private set; }
    public Guid? BuyerId { get; private set; }
    public Buyer? Buyer { get; private set; }
    public string? CustomerId { get; private set; }
    public string? CustomerName { get; private set; }
    public string? CustomerPhone { get; private set; }
    public string? CustomerEmail { get; private set; }
    public Guid? PaymentMethodId { get; private set; }
    public PaymentMethodModel? PaymentMethod { get; private set; }
    public Guid? AddressId { get; private set; }
    public Address? Address { get; private set; }
    public Guid? CardTypeId { get; private set; }
    public CardType? CardType { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public bool IsOffline { get; private set; }
    public string? RefundReason { get; private set; }

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
        CreatedAt = DateTime.UtcNow;
        DomainEvents = new List<DomainEvent>();
    }

    public Sale(Guid storeId, string invoiceNumber, DateTime saleDate, Guid? staffId = null, string? staffName = null, 
        Guid? buyerId = null, string? customerId = null, string? customerName = null, string? customerPhone = null, 
        string? customerEmail = null, Guid? paymentMethodId = null, Guid? addressId = null, Guid? cardTypeId = null, 
        string? notes = null, bool isOffline = false)
    {
        if (storeId == Guid.Empty)
            throw new DomainException("Store ID cannot be empty");
        if (string.IsNullOrWhiteSpace(invoiceNumber))
            throw new DomainException("Invoice number cannot be empty");

        _items = new List<SaleItem>();
        _payments = new List<Payment>();
        _discounts = new List<SaleDiscount>();

        StoreId = storeId;
        InvoiceNumber = invoiceNumber;
        SaleDate = saleDate;
        StaffId = staffId;
        StaffName = staffName;
        BuyerId = buyerId;
        CustomerId = customerId;
        CustomerName = customerName;
        CustomerPhone = customerPhone;
        CustomerEmail = customerEmail;
        PaymentMethodId = paymentMethodId;
        AddressId = addressId;
        CardTypeId = cardTypeId;
        Notes = notes;
        Status = SaleStatus.Pending;
        IsSynced = false;
        SyncedAt = null;
        CreatedAt = DateTime.UtcNow;
        IsOffline = isOffline;
        DomainEvents = new List<DomainEvent>();
    }

    public void AddItem(int productId, string? storeId, int quantity, decimal unitPrice, string? category)
    {
        var item = new SaleItem(productId, productId, storeId, quantity, unitPrice, category);
        _items.Add(item);
        CalculateTotalAmount();
        AddDomainEvent(new SaleItemAddedDomainEvent(this, item));
    }

    public void RemoveItem(Guid itemId)
    {
        var item = _items.Find(i => i.Id == itemId);
        if (item != null)
        {
            _items.Remove(item);
            CalculateTotalAmount();
            AddDomainEvent(new SaleItemRemovedDomainEvent(this, item));
        }
    }

    public void AddPayment(decimal amount, PaymentType type, string? reference = null, string? cardLast4 = null, string? cardType = null)
    {
        if (amount <= 0)
            throw new DomainException("Payment amount must be greater than zero");

        var payment = new Payment(Id, amount, type, reference, cardLast4, cardType);
        _payments.Add(payment);
        AmountPaid += amount;
        Balance = TotalAmount - AmountPaid;
        AddDomainEvent(new SalePaymentAddedDomainEvent(this, payment));

        if (GetBalance() <= 0)
        {
            Complete();
        }
    }

    public void AddDiscount(string? code, string? storeId, DiscountType type, decimal amount, DateTime startDate, DateTime endDate)
    {
        var discount = new SaleDiscount(code, storeId, type, amount, startDate, endDate);
        _discounts.Add(discount);
        CalculateTotalAmount();
        AddDomainEvent(new SaleDiscountAddedDomainEvent(this, discount));
    }

    public void SetCustomer(Guid customerId, string name, string? phone = null, string? email = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Customer name cannot be empty");

        BuyerId = customerId;
        CustomerId = customerId.ToString();
        CustomerName = name;
        CustomerPhone = phone;
        CustomerEmail = email;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetStaff(Guid staffId, string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Staff name cannot be empty");

        StaffId = staffId;
        StaffName = name;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddNotes(string? notes)
    {
        Notes = notes;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsOffline()
    {
        IsSynced = false;
        SyncedAt = null;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsSynced()
    {
        if (IsSynced)
            throw new DomainException("Sale is already synced");

        IsSynced = true;
        SyncedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
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
        UpdatedAt = DateTime.UtcNow;
        CompletedAt = DateTime.UtcNow;
        AddDomainEvent(new SaleCompletedDomainEvent(this));
    }

    public void Void()
    {
        if (Status != SaleStatus.Pending)
            throw new DomainException("Only pending sales can be voided");

        Status = SaleStatus.Voided;
        UpdatedAt = DateTime.UtcNow;
        AddDomainEvent(new SaleVoidedDomainEvent(this));
    }

    public void Refund(decimal amount, string reason)
    {
        if (Status != SaleStatus.Completed)
            throw new DomainException("Only completed sales can be refunded");

        if (amount <= 0)
            throw new DomainException("Refund amount must be greater than zero");

        if (amount > TotalAmount)
            throw new DomainException("Refund amount cannot exceed total amount");

        if (string.IsNullOrWhiteSpace(reason))
            throw new DomainException("Refund reason is required");

        Status = SaleStatus.Refunded;
        RefundReason = reason;
        UpdatedAt = DateTime.UtcNow;
        AddDomainEvent(new SaleRefundedDomainEvent(this, amount, reason));
    }

    public decimal GetSubtotal() => _items.Sum(i => i.UnitPrice * i.Quantity);
    public decimal GetDiscountTotal() => _discounts.Where(d => d.Type != DiscountType.Tip).Sum(d => d.Amount);
    public decimal GetTipTotal() => _discounts.Where(d => d.Type == DiscountType.Tip).Sum(d => d.Amount);
    public decimal GetTotal() => GetSubtotal() - GetDiscountTotal() + GetTipTotal();
    public decimal GetPaymentTotal() => _payments.Sum(p => p.Amount);
    public decimal GetBalance() => GetTotal() - GetPaymentTotal();

    private void CalculateTotalAmount()
    {
        SubTotal = GetSubtotal();
        DiscountAmount = GetDiscountTotal();
        TaxAmount = (SubTotal - DiscountAmount) * 0.1m; // Assuming 10% tax
        TotalAmount = SubTotal - DiscountAmount + TaxAmount;
        Balance = TotalAmount - AmountPaid;
        UpdatedAt = DateTime.UtcNow;
    }
}

public enum SaleStatus
{
    Pending,
    Completed,
    Voided,
    Refunded
}

