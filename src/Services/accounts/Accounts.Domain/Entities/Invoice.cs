using TossErp.Accounts.Domain.Enums;
using TossErp.Shared.SeedWork;
using TossErp.Accounts.Domain.ValueObjects;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TossErp.Accounts.Domain.Entities;

/// <summary>
/// Invoice entity representing invoices in the TOSS ERP system
/// Designed for South African township SMME context
/// </summary>
[Table("Invoices")]
public class Invoice : AggregateRoot
{
    public override Guid Id { get; protected set; }
    public override DateTime CreatedAt { get; protected set; }
    public override string CreatedBy { get; protected set; }
    [Required]
    [StringLength(50)]
    public string InvoiceNumber { get; private set; } = string.Empty;

    public Guid CustomerId { get; private set; }

    [StringLength(200)]
    public string? CustomerName { get; private set; }

    public DateTime InvoiceDate { get; private set; }

    public DateTime DueDate { get; private set; }

    public InvoiceStatus Status { get; private set; } = InvoiceStatus.Draft;

    public InvoiceType InvoiceType { get; private set; } = InvoiceType.Standard;

    public Money SubtotalAmount { get; private set; } = Money.Zero(CurrencyCode.ZAR);

    public Money TaxAmount { get; private set; } = Money.Zero(CurrencyCode.ZAR);

    public Money TotalAmount { get; private set; } = Money.Zero(CurrencyCode.ZAR);

    public Money PaidAmount { get; private set; } = Money.Zero(CurrencyCode.ZAR);

    public Money OutstandingAmount { get; private set; } = Money.Zero(CurrencyCode.ZAR);

    // Application layer compatibility properties
    public Money BalanceAmount => OutstandingAmount;
    public string? Terms => TermsAndConditions;
    public string? InternalNotes => Notes;
    public string? Reference => ReferenceNumber;
    public DateTime LastModified => ModifiedAt;
    public string? LastModifiedBy => ModifiedBy;
    public DateTime IssueDate => InvoiceDate;
    public DateTime? PaidDate => PaymentDate;
    public Money DiscountAmount => _lineItems.Aggregate(
        Money.Zero(CurrencyCode.ZAR),
        (sum, item) => sum.Add(item.DiscountAmount ?? Money.Zero(CurrencyCode.ZAR)));

    [StringLength(3)]
    public string Currency { get; private set; } = "ZAR";

    [StringLength(1000)]
    public string? Description { get; private set; }

    [StringLength(500)]
    public string? Notes { get; private set; }

    [StringLength(100)]
    public string? ReferenceNumber { get; private set; }

    [StringLength(100)]
    public string? PurchaseOrderNumber { get; private set; }

    // Township/SMME specific fields
    [StringLength(20)]
    public string? PaymentMethod { get; private set; }

    public bool AllowStoreCredit { get; private set; } = false;

    public bool AllowPartialPayment { get; private set; } = true;

    public DateTime? PaymentDate { get; private set; }

    [StringLength(200)]
    public string? DeliveryLocation { get; private set; }

    // Address fields for application layer compatibility
    public Address? BillingAddress { get; private set; }
    
    public Address? ShippingAddress { get; private set; }

    // Terms and conditions
    public PaymentTerms PaymentTerms { get; private set; } = PaymentTerms.Net30;

    [StringLength(1000)]
    public string? TermsAndConditions { get; private set; }

    // Audit fields
    public DateTime ModifiedAt { get; private set; } = DateTime.UtcNow;

    [StringLength(100)]
    public string? ModifiedBy { get; private set; }

    public DateTime? SentDate { get; private set; }

    [StringLength(100)]
    public string? SentBy { get; private set; }

    // Navigation properties
    public Customer Customer { get; private set; } = null!;

    private readonly List<InvoiceLineItem> _lineItems = new();
    public IReadOnlyList<InvoiceLineItem> LineItems => _lineItems.AsReadOnly();

    private readonly List<Payment> _payments = new();
    public IReadOnlyList<Payment> Payments => _payments.AsReadOnly();

    private Invoice() : base() { } // EF Core

    public Invoice(
        Guid id,
        string tenantId,
        string invoiceNumber,
        Guid customerId,
        string? customerName,
        DateTime invoiceDate,
        DateTime dueDate,
        string createdBy,
        InvoiceType invoiceType = InvoiceType.Standard,
        string? description = null)
    {
        InvoiceNumber = invoiceNumber?.Trim() ?? throw new ArgumentException("Invoice number cannot be empty");
        CustomerId = customerId;
        CustomerName = customerName?.Trim();
        InvoiceDate = invoiceDate.Date;
        DueDate = dueDate.Date;
        ModifiedBy = createdBy?.Trim() ?? throw new ArgumentException("CreatedBy cannot be empty");
        InvoiceType = invoiceType;
        Description = description?.Trim();
        ModifiedAt = DateTime.UtcNow;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = createdBy;
        OutstandingAmount = TotalAmount;
    }

    public static Invoice Create(
        string tenantId,
        Guid customerId,
        DateOnly issueDate,
        DateOnly dueDate,
        IEnumerable<InvoiceLineItem> lineItems,
        Address? billingAddress,
        Address? shippingAddress,
        string? terms,
        string? notes,
        string? internalNotes,
        string? reference,
        string? purchaseOrderNumber,
        string currency,
        string createdBy)
    {
        var invoice = new Invoice(
            Guid.NewGuid(),
            tenantId,
            $"INV-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..8].ToUpper()}",
            customerId,
            null,
            issueDate.ToDateTime(TimeOnly.MinValue),
            dueDate.ToDateTime(TimeOnly.MinValue),
            createdBy,
            InvoiceType.Standard,
            notes);
        invoice.BillingAddress = billingAddress;
        invoice.ShippingAddress = shippingAddress;
        invoice.TermsAndConditions = terms;
        invoice.Notes = notes;
        invoice.ReferenceNumber = reference;
        invoice.PurchaseOrderNumber = purchaseOrderNumber;
        invoice.Currency = currency;
        foreach (var item in lineItems) invoice.AddLineItem(item);
        return invoice;
    }

    public void AddLineItem(InvoiceLineItem lineItem)
    {
        if (lineItem == null)
            throw new ArgumentNullException(nameof(lineItem));

        if (Status != InvoiceStatus.Draft)
            throw new InvalidOperationException("Cannot modify non-draft invoice");

        _lineItems.Add(lineItem);
        RecalculateAmounts();
    }

    public void RemoveLineItem(Guid lineItemId)
    {
        if (Status != InvoiceStatus.Draft)
            throw new InvalidOperationException("Cannot modify non-draft invoice");

        var item = _lineItems.FirstOrDefault(x => x.Id == lineItemId);
        if (item != null)
        {
            _lineItems.Remove(item);
            RecalculateAmounts();
        }
    }

    public void UpdateLineItem(Guid lineItemId, int quantity, Money unitPrice)
    {
        if (Status != InvoiceStatus.Draft)
            throw new InvalidOperationException("Cannot modify non-draft invoice");

        var item = _lineItems.FirstOrDefault(x => x.Id == lineItemId);
        if (item != null)
        {
            item.UpdateQuantity(quantity);
            item.UpdateUnitPrice(unitPrice);
            RecalculateAmounts();
        }
    }

    public void Submit(string modifiedBy)
    {
        if (Status != InvoiceStatus.Draft)
            throw new InvalidOperationException("Only draft invoices can be submitted");

        if (!_lineItems.Any())
            throw new InvalidOperationException("Invoice must have at least one line item");

        Status = InvoiceStatus.Pending;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy?.Trim() ?? throw new ArgumentException("ModifiedBy cannot be empty");
    }

    public void Approve(string modifiedBy)
    {
        if (Status != InvoiceStatus.Pending)
            throw new InvalidOperationException("Only pending invoices can be approved");

        Status = InvoiceStatus.Approved;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy?.Trim() ?? throw new ArgumentException("ModifiedBy cannot be empty");
    }

    public void Send(string sentBy)
    {
        if (Status != InvoiceStatus.Approved && Status != InvoiceStatus.Draft)
            throw new InvalidOperationException("Only approved or draft invoices can be sent");

        Status = InvoiceStatus.Sent;
        SentDate = DateTime.UtcNow;
        SentBy = sentBy?.Trim() ?? throw new ArgumentException("SentBy cannot be empty");
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = sentBy;
    }

    public void RecordPayment(Money amount, DateTime paymentDate, string modifiedBy)
    {
        if (amount.Amount <= 0)
            throw new ArgumentException("Payment amount must be positive");

        if (amount.Amount > OutstandingAmount.Amount)
            throw new ArgumentException("Payment amount cannot exceed outstanding amount");

        PaidAmount = PaidAmount.Add(amount);
        OutstandingAmount = OutstandingAmount.Subtract(amount);

        if (OutstandingAmount.Amount == 0)
        {
            Status = InvoiceStatus.Paid;
            PaymentDate = paymentDate;
        }
        else if (PaidAmount.Amount > 0)
        {
            Status = InvoiceStatus.PartiallyPaid;
        }

        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy?.Trim() ?? throw new ArgumentException("ModifiedBy cannot be empty");
    }

    public void ApplyPayment(Payment payment)
    {
        if (payment == null)
            throw new ArgumentNullException(nameof(payment));

        RecordPayment(payment.Amount, payment.PaymentDate, payment.CreatedBy ?? "System");
        _payments.Add(payment);
    }

    public void Cancel(string modifiedBy)
    {
        if (Status == InvoiceStatus.Paid)
            throw new InvalidOperationException("Cannot cancel paid invoice");

        Status = InvoiceStatus.Cancelled;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy?.Trim() ?? throw new ArgumentException("ModifiedBy cannot be empty");
    }

    public void MarkAsOverdue()
    {
        if (Status == InvoiceStatus.Sent && DateTime.UtcNow.Date > DueDate.Date)
        {
            Status = InvoiceStatus.Overdue;
        }
    }

    public void MarkOverdue()
    {
        MarkAsOverdue();
    }

    public void UpdateCustomerInfo(string? customerName, string modifiedBy)
    {
        CustomerName = customerName?.Trim();
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy?.Trim() ?? throw new ArgumentException("ModifiedBy cannot be empty");
    }

    public void UpdateDueDate(DateTime dueDate, string modifiedBy)
    {
        if (Status == InvoiceStatus.Paid)
            throw new InvalidOperationException("Cannot modify paid invoice");

        DueDate = dueDate.Date;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy?.Trim() ?? throw new ArgumentException("ModifiedBy cannot be empty");
    }

    public void UpdatePaymentTerms(PaymentTerms paymentTerms, string modifiedBy)
    {
        PaymentTerms = paymentTerms;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy?.Trim() ?? throw new ArgumentException("ModifiedBy cannot be empty");
    }

    public void SetDeliveryLocation(string? deliveryLocation, string modifiedBy)
    {
        DeliveryLocation = deliveryLocation?.Trim();
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy?.Trim() ?? throw new ArgumentException("ModifiedBy cannot be empty");
    }

    public void SetBillingAddress(Address? billingAddress, string modifiedBy)
    {
        BillingAddress = billingAddress;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy?.Trim() ?? throw new ArgumentException("ModifiedBy cannot be empty");
    }

    public void SetShippingAddress(Address? shippingAddress, string modifiedBy)
    {
        ShippingAddress = shippingAddress;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy?.Trim() ?? throw new ArgumentException("ModifiedBy cannot be empty");
    }

    public void UpdateDetails(
        DateOnly dueDate,
        Address? billingAddress,
        Address? shippingAddress,
        string? terms,
        string? notes,
        string? internalNotes,
        string? reference,
        string? purchaseOrderNumber,
        string updatedBy)
    {
        DueDate = dueDate.ToDateTime(TimeOnly.MinValue);
        BillingAddress = billingAddress;
        ShippingAddress = shippingAddress;
        TermsAndConditions = terms;
        Notes = notes;
        ReferenceNumber = reference;
        PurchaseOrderNumber = purchaseOrderNumber;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = updatedBy?.Trim() ?? throw new ArgumentException("ModifiedBy cannot be empty");
    }

    public void UpdateLineItems(IEnumerable<InvoiceLineItem> lineItems, string modifiedBy)
    {
        if (Status != InvoiceStatus.Draft)
            throw new InvalidOperationException("Cannot modify non-draft invoice");

        _lineItems.Clear();
        foreach (var item in lineItems)
        {
            _lineItems.Add(item);
        }
        RecalculateAmounts();
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy?.Trim() ?? throw new ArgumentException("ModifiedBy cannot be empty");
    }

    private void RecalculateAmounts()
    {
        SubtotalAmount = _lineItems.Aggregate(
            Money.Zero(CurrencyCode.ZAR),
            (sum, item) => sum.Add(item.LineTotal ?? Money.Zero(CurrencyCode.ZAR)));

        TaxAmount = _lineItems.Aggregate(
            Money.Zero(CurrencyCode.ZAR),
            (sum, item) => sum.Add(item.TaxAmount ?? Money.Zero(CurrencyCode.ZAR)));

        TotalAmount = SubtotalAmount.Add(TaxAmount);
        OutstandingAmount = TotalAmount.Subtract(PaidAmount);
    }

    public bool IsOverdue => Status == InvoiceStatus.Sent && DateTime.UtcNow.Date > DueDate.Date;
    public bool IsPaid => Status == InvoiceStatus.Paid;
    public bool IsPartiallyPaid => Status == InvoiceStatus.PartiallyPaid;
    public int DaysOverdue => IsOverdue ? (DateTime.UtcNow.Date - DueDate.Date).Days : 0;
}

/// <summary>
/// Invoice Line Item entity representing individual items on an invoice
/// </summary>
[Table("InvoiceLineItems")]
public class InvoiceLineItem : Entity
{
    public override Guid Id { get; protected set; }
    public Guid InvoiceId { get; private set; }

    [Required]
    [StringLength(200)]
    public string ItemName { get; private set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; private set; }

    public int Quantity { get; private set; }

    public Money UnitPrice { get; private set; } = Money.Zero(CurrencyCode.ZAR);

    public Money? LineTotal { get; private set; }

    public TaxRate? TaxRate { get; private set; }

    public Money? TaxAmount { get; private set; }

    [StringLength(50)]
    public string? ProductCode { get; private set; }

    public Guid? ProductId { get; private set; }

    [StringLength(20)]
    public string? Unit { get; private set; } // e.g., "kg", "pieces", "litres"

    public decimal? DiscountPercentage { get; private set; }

    public Money? DiscountAmount { get; private set; }

    // Navigation properties
    public Invoice Invoice { get; private set; } = null!;

    private InvoiceLineItem() : base() { } // EF Core

    public InvoiceLineItem(
        Guid id,
        string tenantId,
        Guid invoiceId,
        string itemName,
        int quantity,
        Money unitPrice,
        string? description = null,
        TaxRate? taxRate = null,
        Guid? productId = null,
        string? productCode = null,
        string? unit = null)
    {
        InvoiceId = invoiceId;
        ItemName = itemName?.Trim() ?? throw new ArgumentException("Item name cannot be empty");
        Description = description?.Trim();
        Quantity = quantity > 0 ? quantity : throw new ArgumentException("Quantity must be positive");
        UnitPrice = unitPrice ?? throw new ArgumentNullException(nameof(unitPrice));
        TaxRate = taxRate;
        ProductId = productId;
        ProductCode = productCode?.Trim();
        Unit = unit?.Trim();

        CalculateAmounts();
    }

    public static InvoiceLineItem Create(
        string tenantId,
        Guid invoiceId,
        string itemName,
        int quantity,
        Money unitPrice,
        string? description = null,
        TaxRate? taxRate = null,
        Guid? productId = null,
        string? productCode = null,
        string? unit = null)
    {
        return new InvoiceLineItem(Guid.NewGuid(), tenantId, invoiceId, itemName, quantity, unitPrice,
            description, taxRate, productId, productCode, unit);
    }

    public void UpdateQuantity(int quantity)
    {
        Quantity = quantity > 0 ? quantity : throw new ArgumentException("Quantity must be positive");
        CalculateAmounts();
    }

    public void UpdateUnitPrice(Money unitPrice)
    {
        UnitPrice = unitPrice ?? throw new ArgumentNullException(nameof(unitPrice));
        CalculateAmounts();
    }

    public void UpdateTaxRate(TaxRate? taxRate)
    {
        TaxRate = taxRate;
        CalculateAmounts();
    }

    public void SetDiscount(decimal discountPercentage)
    {
        if (discountPercentage < 0 || discountPercentage > 100)
            throw new ArgumentException("Discount percentage must be between 0 and 100");

        DiscountPercentage = discountPercentage;
        DiscountAmount = LineTotal?.Multiply(discountPercentage / 100);
        CalculateAmounts();
    }

    public void SetDiscountAmount(Money discountAmount)
    {
        if (discountAmount.Amount < 0)
            throw new ArgumentException("Discount amount cannot be negative");

        DiscountAmount = discountAmount;
        DiscountPercentage = LineTotal?.Amount > 0 ? (discountAmount.Amount / LineTotal.Amount) * 100 : 0;
        CalculateAmounts();
    }

    private void CalculateAmounts()
    {
        LineTotal = UnitPrice.Multiply(Quantity);
        
        if (DiscountAmount != null && DiscountAmount.Amount > 0)
        {
            LineTotal = LineTotal.Subtract(DiscountAmount);
        }

        TaxAmount = TaxRate?.CalculateTax(LineTotal);
    }

    public Money TotalWithTax => LineTotal != null 
        ? (TaxAmount != null ? LineTotal.Add(TaxAmount) : LineTotal) 
        : Money.Zero(CurrencyCode.ZAR);

    public Money NetAmount => LineTotal ?? Money.Zero(CurrencyCode.ZAR);
}
