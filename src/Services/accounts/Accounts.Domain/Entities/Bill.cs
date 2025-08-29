using TossErp.Accounts.Domain.Enums;
using TossErp.Shared.SeedWork;
using TossErp.Accounts.Domain.ValueObjects;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TossErp.Accounts.Domain.Entities;

/// <summary>
/// Bill entity representing supplier/vendor bills in the TOSS ERP system
/// Designed for South African township SMME context
/// </summary>
[Table("Bills")]
public class Bill : AggregateRoot
{
    public override Guid Id { get; protected set; }
    public override DateTime CreatedAt { get; protected set; }
    public override string CreatedBy { get; protected set; }
    
    [Required]
    [StringLength(50)]
    public string BillNumber { get; private set; } = string.Empty;

    public Guid SupplierId { get; private set; }

    [StringLength(200)]
    public string? SupplierName { get; private set; }

    [StringLength(50)]
    public string? SupplierInvoiceNumber { get; private set; }

    public DateTime BillDate { get; private set; }

    public DateTime DueDate { get; private set; }

    public BillStatus Status { get; private set; } = BillStatus.Draft;

    public BillType BillType { get; private set; } = BillType.Standard;

    public Money SubtotalAmount { get; private set; } = Money.Zero(CurrencyCode.ZAR);

    public Money TaxAmount { get; private set; } = Money.Zero(CurrencyCode.ZAR);

    public Money TotalAmount { get; private set; } = Money.Zero(CurrencyCode.ZAR);

    public Money PaidAmount { get; private set; } = Money.Zero(CurrencyCode.ZAR);

    public Money OutstandingAmount { get; private set; } = Money.Zero(CurrencyCode.ZAR);

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

    public bool IsRecurring { get; private set; } = false;

    [StringLength(50)]
    public string? RecurringPattern { get; private set; }

    public DateTime? PaymentDate { get; private set; }

    [StringLength(200)]
    public string? DeliveryLocation { get; private set; }

    [StringLength(200)]
    public string? CommunityBenefit { get; private set; } // How this purchase benefits the community

    public bool IsCommunityPurchase { get; private set; } = false;

    // Terms and conditions
    public PaymentTerms PaymentTerms { get; private set; } = PaymentTerms.Net30;

    [StringLength(1000)]
    public string? TermsAndConditions { get; private set; }

    // Approval workflow
    public bool RequiresApproval { get; private set; } = false;

    public DateTime? SubmittedDate { get; private set; }

    [StringLength(100)]
    public string? SubmittedBy { get; private set; }

    public DateTime? ApprovedDate { get; private set; }

    [StringLength(100)]
    public string? ApprovedBy { get; private set; }

    // Audit fields
    public DateTime ModifiedAt { get; private set; } = DateTime.UtcNow;

    [StringLength(100)]
    public string? ModifiedBy { get; private set; }

    // Navigation properties
    private readonly List<BillLineItem> _lineItems = new();
    public IReadOnlyList<BillLineItem> LineItems => _lineItems.AsReadOnly();

    private readonly List<Payment> _payments = new();
    public IReadOnlyList<Payment> Payments => _payments.AsReadOnly();

    private Bill() : base() { } // EF Core

    public Bill(
        Guid id,
        string tenantId,
        string billNumber,
        Guid supplierId,
        string? supplierName,
        DateTime billDate,
        DateTime dueDate,
        string createdBy,
        BillType billType = BillType.Standard,
        string? description = null,
        string? supplierInvoiceNumber = null)
    {
        Id = id;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = createdBy?.Trim() ?? throw new ArgumentException("CreatedBy cannot be empty");
        BillNumber = billNumber?.Trim() ?? throw new ArgumentException("Bill number cannot be empty");
        SupplierId = supplierId;
        SupplierName = supplierName?.Trim();
        SupplierInvoiceNumber = supplierInvoiceNumber?.Trim();
        BillDate = billDate.Date;
        DueDate = dueDate.Date;
        BillType = billType;
        Description = description?.Trim();
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = createdBy;
        OutstandingAmount = TotalAmount;
    }

    public static Bill Create(
        string tenantId,
        string billNumber,
        Guid supplierId,
        string? supplierName,
        DateTime billDate,
        DateTime dueDate,
        string createdBy,
        BillType billType = BillType.Standard,
        string? description = null,
        string? supplierInvoiceNumber = null)
    {
        return new Bill(Guid.NewGuid(), tenantId, billNumber, supplierId, supplierName,
            billDate, dueDate, createdBy, billType, description, supplierInvoiceNumber);
    }

    public void AddLineItem(BillLineItem lineItem)
    {
        if (lineItem == null)
            throw new ArgumentNullException(nameof(lineItem));

        if (Status != BillStatus.Draft)
            throw new InvalidOperationException("Cannot modify non-draft bill");

        _lineItems.Add(lineItem);
        RecalculateAmounts();
    }

    public void RemoveLineItem(Guid lineItemId)
    {
        if (Status != BillStatus.Draft)
            throw new InvalidOperationException("Cannot modify non-draft bill");

        var item = _lineItems.FirstOrDefault(x => x.Id == lineItemId);
        if (item != null)
        {
            _lineItems.Remove(item);
            RecalculateAmounts();
        }
    }

    public void UpdateLineItem(Guid lineItemId, int quantity, Money unitPrice)
    {
        if (Status != BillStatus.Draft)
            throw new InvalidOperationException("Cannot modify non-draft bill");

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
        if (Status != BillStatus.Draft)
            throw new InvalidOperationException("Only draft bills can be submitted");

        if (!_lineItems.Any())
            throw new InvalidOperationException("Bill must have at least one line item");

        Status = BillStatus.Pending;
        SubmittedDate = DateTime.UtcNow;
        SubmittedBy = modifiedBy?.Trim() ?? throw new ArgumentException("SubmittedBy cannot be empty");
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy;
    }

    public void Approve(string modifiedBy)
    {
        if (Status != BillStatus.Pending)
            throw new InvalidOperationException("Only pending bills can be approved");

        Status = BillStatus.Approved;
        ApprovedDate = DateTime.UtcNow;
        ApprovedBy = modifiedBy?.Trim() ?? throw new ArgumentException("ApprovedBy cannot be empty");
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy;
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
            Status = BillStatus.Paid;
            PaymentDate = paymentDate;
        }
        else if (PaidAmount.Amount > 0)
        {
            Status = BillStatus.PartiallyPaid;
        }

        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy?.Trim() ?? throw new ArgumentException("ModifiedBy cannot be empty");
    }

    public void Cancel(string modifiedBy)
    {
        if (Status == BillStatus.Paid)
            throw new InvalidOperationException("Cannot cancel paid bill");

        Status = BillStatus.Cancelled;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy?.Trim() ?? throw new ArgumentException("ModifiedBy cannot be empty");
    }

    public void MarkAsOverdue()
    {
        if (Status == BillStatus.Approved && DateTime.UtcNow.Date > DueDate.Date)
        {
            Status = BillStatus.Overdue;
        }
    }

    public void UpdateSupplierInfo(string? supplierName, string modifiedBy)
    {
        SupplierName = supplierName?.Trim();
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy?.Trim() ?? throw new ArgumentException("ModifiedBy cannot be empty");
    }

    public void UpdateDueDate(DateTime dueDate, string modifiedBy)
    {
        if (Status == BillStatus.Paid)
            throw new InvalidOperationException("Cannot modify paid bill");

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

    public void SetCommunityPurchase(string communityBenefit, string modifiedBy)
    {
        IsCommunityPurchase = true;
        CommunityBenefit = communityBenefit?.Trim();
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy?.Trim() ?? throw new ArgumentException("ModifiedBy cannot be empty");
    }

    public void SetRecurringPattern(string recurringPattern, string modifiedBy)
    {
        IsRecurring = true;
        RecurringPattern = recurringPattern?.Trim();
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

    public bool IsOverdue => Status == BillStatus.Approved && DateTime.UtcNow.Date > DueDate.Date;
    public bool IsPaid => Status == BillStatus.Paid;
    public bool IsPartiallyPaid => Status == BillStatus.PartiallyPaid;
    public int DaysOverdue => IsOverdue ? (DateTime.UtcNow.Date - DueDate.Date).Days : 0;
    public bool IsDraft => Status == BillStatus.Draft;
    public bool IsPending => Status == BillStatus.Pending;
    public bool IsApproved => Status == BillStatus.Approved;
    public bool IsCancelled => Status == BillStatus.Cancelled;
}

/// <summary>
/// Bill Line Item entity representing individual items on a bill
/// </summary>
[Table("BillLineItems")]
public class BillLineItem : Entity
{
    public override Guid Id { get; protected set; }
    public Guid BillId { get; private set; }

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

    // Cost center and project tracking
    public Guid? CostCenterId { get; private set; }

    [StringLength(200)]
    public string? CostCenterName { get; private set; }

    public Guid? ProjectId { get; private set; }

    [StringLength(200)]
    public string? ProjectName { get; private set; }

    // Navigation properties
    public Bill Bill { get; private set; } = null!;

    private BillLineItem() : base() { } // EF Core

    public BillLineItem(
        Guid id,
        string tenantId,
        Guid billId,
        string itemName,
        int quantity,
        Money unitPrice,
        string? description = null,
        TaxRate? taxRate = null,
        Guid? productId = null,
        string? productCode = null,
        string? unit = null)
    {
        BillId = billId;
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

    public static BillLineItem Create(
        string tenantId,
        Guid billId,
        string itemName,
        int quantity,
        Money unitPrice,
        string? description = null,
        TaxRate? taxRate = null,
        Guid? productId = null,
        string? productCode = null,
        string? unit = null)
    {
        return new BillLineItem(Guid.NewGuid(), tenantId, billId, itemName, quantity, unitPrice,
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

    public void SetCostCenter(Guid? costCenterId, string? costCenterName)
    {
        CostCenterId = costCenterId;
        CostCenterName = costCenterName?.Trim();
    }

    public void SetProject(Guid? projectId, string? projectName)
    {
        ProjectId = projectId;
        ProjectName = projectName?.Trim();
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

/// <summary>
/// Bill status enumeration
/// </summary>
public enum BillStatus
{
    Draft = 0,
    Pending = 1,
    Approved = 2,
    PartiallyPaid = 3,
    Paid = 4,
    Overdue = 5,
    Cancelled = 6
}

/// <summary>
/// Bill type enumeration
/// </summary>
public enum BillType
{
    Standard = 0,
    Recurring = 1,
    Adjustment = 2,
    Credit = 3,
    Prepayment = 4
}
