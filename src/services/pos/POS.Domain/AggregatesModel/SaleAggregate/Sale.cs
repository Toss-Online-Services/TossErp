#nullable enable
using POS.Domain.AggregatesModel.StaffAggregate;
using POS.Domain.SeedWork;
using POS.Domain.AggregatesModel.BuyerAggregate;
using POS.Domain.AggregatesModel.SaleAggregate.Events;
using POS.Domain.Exceptions;

namespace POS.Domain.AggregatesModel.SaleAggregate;

public class Sale : AggregateRoot
{
    private readonly List<SaleItem> _items = new();
    private readonly List<Payment> _payments = new();
    private readonly List<SaleDiscount> _discounts = new();

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
    public Guid? BuyerId { get; private set; }
    public Buyer? Buyer { get; private set; }
    public Guid? PaymentMethodId { get; private set; }
    public PaymentMethod? PaymentMethod { get; private set; }
    public Guid? AddressId { get; private set; }
    public Address? Address { get; private set; }
    public Guid? CardTypeId { get; private set; }
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

    public Sale(string invoiceNumber, DateTime saleDate, Guid? staffId = null, Guid? buyerId = null, 
        Guid? paymentMethodId = null, Guid? addressId = null, Guid? cardTypeId = null, string? notes = null)
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

    public void AddItem(Guid productId, string productName, decimal unitPrice, int quantity, decimal discount = 0)
    {
        var item = new SaleItem(productId, productName, quantity, unitPrice, 0.1m); // Assuming 10% tax rate
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
        CalculateTotalAmount();
        AddDomainEvent(new SaleDiscountAddedDomainEvent(this, discount));
    }

    public void SetCustomer(Guid customerId, string name, string? phone = null, string? email = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Customer name cannot be empty");

        BuyerId = customerId;
        var address = new Address("N/A", "N/A", "N/A", "N/A", "N/A"); // Create address with placeholder values
        Buyer = new Buyer(customerId, name, email ?? string.Empty, phone ?? string.Empty, address);
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
    }
} 

