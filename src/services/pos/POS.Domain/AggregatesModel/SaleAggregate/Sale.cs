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
    public Money SubTotal { get; private set; }
    public Money TaxAmount { get; private set; }
    public Money DiscountAmount { get; private set; }
    public Money TotalAmount { get; private set; }
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
    public string? CustomerId { get; private set; }
    public string? CustomerName { get; private set; }
    public string? CustomerPhone { get; private set; }
    public string? CustomerEmail { get; private set; }
    public Guid? PaymentMethodId { get; private set; }
    public PaymentMethodModel? PaymentMethod { get; private set; }
    public Guid? AddressId { get; private set; }
    public POS.Domain.Common.ValueObjects.Address? Address { get; private set; }
    public Guid? CardTypeId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public bool IsOffline { get; private set; }
    public string? RefundReason { get; private set; }

    public IReadOnlyCollection<SaleItem> Items => _items.AsReadOnly();
    public IReadOnlyCollection<Payment> Payments => _payments.AsReadOnly();
    public IReadOnlyCollection<SaleDiscount> Discounts => _discounts.AsReadOnly();

    private Sale()
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
        AddDomainEvent(new SaleCreatedDomainEvent(Id, storeId, customerId));
    }

    public void AddItem(Guid productId, string productName, decimal unitPrice, int quantity, decimal taxRate)
    {
        if (Status != SaleStatus.Pending)
            throw new DomainException("Can only add items to a pending sale");

        if (quantity <= 0)
            throw new DomainException("Quantity must be greater than zero");

        if (unitPrice < 0)
            throw new DomainException("Unit price cannot be negative");

        if (taxRate < 0 || taxRate > 100)
            throw new DomainException("Tax rate must be between 0 and 100");

        var item = new SaleItem(productId, productName, unitPrice, quantity, taxRate, "USD");
        _items.Add(item);
        CalculateTotalAmount();
        AddDomainEvent(new SaleItemAddedDomainEvent(Id, item.Id));
    }

    public void UpdateItemQuantity(Guid itemId, int quantity)
    {
        if (Status != SaleStatus.Pending)
            throw new DomainException("Can only update items in a pending sale");

        if (quantity <= 0)
            throw new DomainException("Quantity must be greater than zero");

        var item = _items.Find(i => i.Id == itemId);
        if (item == null)
            throw new DomainException("Item not found");

        var oldQuantity = item.Quantity;
        item.UpdateQuantity(quantity);
        CalculateTotalAmount();
        AddDomainEvent(new SaleItemQuantityUpdatedDomainEvent(this, item, oldQuantity, quantity));
    }

    public void RemoveItem(Guid itemId)
    {
        var item = _items.Find(i => i.Id == itemId);
        if (item != null)
        {
            _items.Remove(item);
            CalculateTotalAmount();
            AddDomainEvent(new SaleItemRemovedDomainEvent(Id, itemId));
        }
    }

    public void AddPayment(decimal amount, PaymentType type, string? reference = null, string? cardLast4 = null, string? cardType = null)
    {
        if (Status == SaleStatus.Voided)
            throw new DomainException("Cannot add payment to a voided sale");

        if (Status == SaleStatus.Refunded)
            throw new DomainException("Cannot add payment to a refunded sale");

        if (amount <= 0)
            throw new DomainException("Payment amount must be greater than zero");

        if (amount > GetBalance())
            throw new DomainException("Payment amount cannot exceed balance");

        var payment = new Payment(Id, amount, type, reference, cardLast4, cardType);
        _payments.Add(payment);
        AmountPaid += amount;
        Balance = TotalAmount.Amount - AmountPaid;
        AddDomainEvent(new SalePaymentAddedDomainEvent(Id, payment.Id));

        if (GetBalance() <= 0)
        {
            Complete();
        }
    }

    public void AddDiscount(string? code, string? storeId, DiscountType type, decimal amount, DateTime startDate, DateTime endDate)
    {
        if (Status != SaleStatus.Pending)
            throw new DomainException("Can only add discounts to a pending sale");

        if (amount <= 0)
            throw new DomainException("Discount amount must be greater than zero");

        if (type == DiscountType.Percentage && amount > 100)
            throw new DomainException("Percentage discount cannot exceed 100%");

        if (startDate > endDate)
            throw new DomainException("Discount start date must be before end date");

        var discount = new SaleDiscount(code, storeId, type, amount, startDate, endDate);
        _discounts.Add(discount);
        CalculateTotalAmount();
        AddDomainEvent(new SaleDiscountAddedDomainEvent(this, discount));
    }

    public void RemoveDiscount(Guid discountId)
    {
        if (Status != SaleStatus.Pending)
            throw new DomainException("Can only remove discounts from a pending sale");

        var discount = _discounts.Find(d => d.Id == discountId);
        if (discount == null)
            throw new DomainException("Discount not found");

        _discounts.Remove(discount);
        CalculateTotalAmount();
        AddDomainEvent(new SaleDiscountRemovedDomainEvent(this, discount));
    }

    public void SetCustomer(Guid customerId, string name, string? phone = null, string? email = null)
    {
        if (Status != SaleStatus.Pending)
            throw new DomainException("Can only set customer for a pending sale");

        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Customer name cannot be empty");

        if (email != null && !IsValidEmail(email))
            throw new DomainException("Invalid email format");

        if (phone != null && !IsValidPhone(phone))
            throw new DomainException("Invalid phone format");

        BuyerId = customerId;
        CustomerId = customerId.ToString();
        CustomerName = name;
        CustomerPhone = phone;
        CustomerEmail = email;
        UpdatedAt = DateTime.UtcNow;
        AddDomainEvent(new SaleCustomerUpdatedDomainEvent(this, customerId, name));
    }

    public void SetStaff(Guid staffId, string name)
    {
        if (Status != SaleStatus.Pending)
            throw new DomainException("Can only set staff for a pending sale");

        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Staff name cannot be empty");

        StaffId = staffId;
        StaffName = name;
        UpdatedAt = DateTime.UtcNow;
        AddDomainEvent(new SaleStaffUpdatedDomainEvent(this, staffId, name));
    }

    public void AddNotes(string? notes)
    {
        if (Status != SaleStatus.Pending)
            throw new DomainException("Can only add notes to a pending sale");

        Notes = notes;
        UpdatedAt = DateTime.UtcNow;
        AddDomainEvent(new SaleNotesUpdatedDomainEvent(this, notes));
    }

    public void MarkAsOffline()
    {
        if (IsOffline)
            throw new DomainException("Sale is already offline");

        IsOffline = true;
        IsSynced = false;
        SyncedAt = null;
        UpdatedAt = DateTime.UtcNow;
        AddDomainEvent(new SaleMarkedAsOfflineDomainEvent(this));
    }

    public void MarkAsSynced()
    {
        if (IsSynced)
            throw new DomainException("Sale is already synced");

        if (!IsOffline)
            throw new DomainException("Only offline sales can be marked as synced");

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

        if (AmountPaid < TotalAmount.Amount)
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

        if (_payments.Any())
            throw new DomainException("Cannot void a sale with payments");

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

        if (amount > TotalAmount.Amount)
            throw new DomainException("Refund amount cannot exceed total amount");

        if (string.IsNullOrWhiteSpace(reason))
            throw new DomainException("Refund reason is required");

        Status = SaleStatus.Refunded;
        RefundReason = reason;
        UpdatedAt = DateTime.UtcNow;
        AddDomainEvent(new SaleRefundedDomainEvent(this, amount, reason));
    }

    private bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    private bool IsValidPhone(string phone)
    {
        return System.Text.RegularExpressions.Regex.IsMatch(phone, @"^\+?[0-9]{10,15}$");
    }

    public decimal GetSubtotal() => _items.Sum(i => i.UnitPrice * i.Quantity);
    public decimal GetDiscountTotal() => _discounts.Where(d => d.Type != DiscountType.Tip).Sum(d => d.Amount);
    public decimal GetTipTotal() => _discounts.Where(d => d.Type == DiscountType.Tip).Sum(d => d.Amount);
    public decimal GetTotal() => GetSubtotal() - GetDiscountTotal() + GetTipTotal();
    public decimal GetPaymentTotal() => _payments.Sum(p => p.Amount);
    public decimal GetBalance() => GetTotal() - GetPaymentTotal();

    private void CalculateTotalAmount()
    {
        SubTotal = new Money(GetSubtotal(), Currency);
        DiscountAmount = new Money(GetDiscountTotal(), Currency);
        TaxAmount = new Money((GetSubtotal() - GetDiscountTotal()) * 0.1m, Currency);
        TotalAmount = new Money(GetSubtotal() - GetDiscountTotal() + GetTipTotal(), Currency);
        Balance = GetTotal() - GetPaymentTotal();
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

