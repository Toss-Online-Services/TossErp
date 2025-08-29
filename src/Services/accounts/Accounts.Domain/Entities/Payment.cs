using TossErp.Accounts.Domain.Enums;
using TossErp.Shared.SeedWork;
using TossErp.Accounts.Domain.ValueObjects;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TossErp.Accounts.Domain.Entities;

/// <summary>
/// Payment entity representing payments in the TOSS ERP system
/// Designed for South African township SMME context
/// </summary>
[Table("Payments")]
public class Payment : AggregateRoot
{
    public override Guid Id { get; protected set; }
    public override DateTime CreatedAt { get; protected set; }
    public override string CreatedBy { get; protected set; }
    [Required]
    [StringLength(50)]
    public string PaymentNumber { get; private set; } = string.Empty;

    public Guid? CustomerId { get; private set; }

    [StringLength(200)]
    public string? CustomerName { get; private set; }

    public Guid? InvoiceId { get; private set; }

    [StringLength(50)]
    public string? InvoiceNumber { get; private set; }

    public DateTime PaymentDate { get; private set; }

    public Money Amount { get; private set; } = Money.Zero(CurrencyCode.ZAR);

    public PaymentMethod PaymentMethod { get; private set; } = PaymentMethod.Cash;

    public PaymentStatus Status { get; private set; } = PaymentStatus.Pending;

    public PaymentType PaymentType { get; private set; } = PaymentType.CustomerPayment;

    [StringLength(3)]
    public string Currency { get; private set; } = "ZAR";

    [StringLength(100)]
    public string? ReferenceNumber { get; private set; }

    [StringLength(500)]
    public string? Description { get; private set; }

    [StringLength(1000)]
    public string? Notes { get; private set; }

    // Bank/transaction details
    [StringLength(100)]
    public string? BankReference { get; private set; }

    [StringLength(100)]
    public string? ChequeNumber { get; private set; }

    [StringLength(200)]
    public string? BankName { get; private set; }

    [StringLength(50)]
    public string? AccountNumber { get; private set; }

    // Digital payment details (for mobile money, EFT, etc.)
    [StringLength(100)]
    public string? TransactionId { get; private set; }

    [StringLength(100)]
    public string? PaymentProcessor { get; private set; }

    public DateTime? ProcessedDate { get; private set; }

    // Application layer compatibility properties
    public PaymentMethod Method => PaymentMethod;
    
    [StringLength(100)]
    public string? Reference 
    { 
        get => ReferenceNumber; 
        private set => ReferenceNumber = value; 
    }

    [StringLength(100)]
    public string? ExternalTransactionId 
    { 
        get => TransactionId; 
        private set => TransactionId = value; 
    }

    public Money? RefundAmount { get; private set; }

    [StringLength(500)]
    public string? RefundReason { get; private set; }

    // Township/SMME specific fields
    [StringLength(200)]
    public string? LocationOfPayment { get; private set; }

    public bool IsStoreCredit { get; private set; } = false;

    public bool IsCommunityPayment { get; private set; } = false;

    [StringLength(100)]
    public string? CommunityGroupName { get; private set; }

    // Fee information
    public Money? ProcessingFee { get; private set; }

    public Money? BankCharges { get; private set; }

    public Money NetAmount => Amount.Subtract(ProcessingFee ?? Money.Zero(CurrencyCode.ZAR))
                                   .Subtract(BankCharges ?? Money.Zero(CurrencyCode.ZAR));

    // Audit fields
    public DateTime ModifiedAt { get; private set; } = DateTime.UtcNow;

    [StringLength(100)]
    public string? ModifiedBy { get; private set; }

    // Application layer compatibility properties
    public DateTime LastModified => ModifiedAt;
    public string? LastModifiedBy => ModifiedBy;

    public DateTime? ApprovedDate { get; private set; }

    [StringLength(100)]
    public string? ApprovedBy { get; private set; }

    // Navigation properties
    public Customer? Customer { get; private set; }

    public Invoice? Invoice { get; private set; }

    private readonly List<PaymentAllocation> _allocations = new();
    public IReadOnlyList<PaymentAllocation> Allocations => _allocations.AsReadOnly();

    // Application layer compatibility property
    public IReadOnlyList<PaymentAllocation> PaymentAllocations => _allocations.AsReadOnly();

    private Payment() : base() { } // EF Core

    public Payment(
        Guid id,
        string tenantId,
        string paymentNumber,
        DateTime paymentDate,
        Money amount,
        PaymentMethod paymentMethod,
        PaymentType paymentType,
        string createdBy,
        Guid? customerId = null,
        string? customerName = null,
        Guid? invoiceId = null,
        string? invoiceNumber = null,
        string? description = null)
    {
        PaymentNumber = paymentNumber?.Trim() ?? throw new ArgumentException("Payment number cannot be empty");
        PaymentDate = paymentDate.Date;
        Amount = amount ?? throw new ArgumentNullException(nameof(amount));
        PaymentMethod = paymentMethod;
        PaymentType = paymentType;
        ModifiedBy = createdBy?.Trim() ?? throw new ArgumentException("CreatedBy cannot be empty");
        CustomerId = customerId;
        CustomerName = customerName?.Trim();
        InvoiceId = invoiceId;
        InvoiceNumber = invoiceNumber?.Trim();
        Description = description?.Trim();
        ModifiedAt = DateTime.UtcNow;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = createdBy;
        Currency = amount.Currency.ToString();
    }

    public static Payment Create(
        string tenantId,
        string paymentNumber,
        DateTime paymentDate,
        Money amount,
        PaymentMethod paymentMethod,
        PaymentType paymentType,
        string createdBy,
        Guid? customerId = null,
        string? customerName = null,
        Guid? invoiceId = null,
        string? invoiceNumber = null,
        string? description = null)
    {
        return new Payment(Guid.NewGuid(), tenantId, paymentNumber, paymentDate, amount, paymentMethod,
            paymentType, createdBy, customerId, customerName, invoiceId, invoiceNumber, description);
    }

    public void Submit(string modifiedBy)
    {
        if (Status != PaymentStatus.Pending)
            throw new InvalidOperationException("Only pending payments can be submitted");

        Status = PaymentStatus.Completed;
        ProcessedDate = DateTime.UtcNow;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy?.Trim() ?? throw new ArgumentException("ModifiedBy cannot be empty");
    }

    public void Approve(string approvedBy)
    {
        if (Status != PaymentStatus.Pending)
            throw new InvalidOperationException("Only pending payments can be approved");

        Status = PaymentStatus.Completed;
        ApprovedDate = DateTime.UtcNow;
        ApprovedBy = approvedBy?.Trim() ?? throw new ArgumentException("ApprovedBy cannot be empty");
        ProcessedDate = DateTime.UtcNow;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = approvedBy;
    }

    public void Fail(string reason, string modifiedBy)
    {
        if (Status == PaymentStatus.Completed)
            throw new InvalidOperationException("Cannot fail completed payment");

        Status = PaymentStatus.Failed;
        Notes = string.IsNullOrEmpty(Notes) ? reason : $"{Notes}; {reason}";
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy?.Trim() ?? throw new ArgumentException("ModifiedBy cannot be empty");
    }

    public void Cancel(string reason, string modifiedBy)
    {
        if (Status == PaymentStatus.Completed)
            throw new InvalidOperationException("Cannot cancel completed payment");

        Status = PaymentStatus.Cancelled;
        Notes = string.IsNullOrEmpty(Notes) ? reason : $"{Notes}; Cancelled: {reason}";
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy?.Trim() ?? throw new ArgumentException("ModifiedBy cannot be empty");
    }

    public Payment Refund(Money refundAmount, string refundReason, string createdBy)
    {
        if (Status != PaymentStatus.Completed)
            throw new InvalidOperationException("Only completed payments can be refunded");

        if (refundAmount.Amount <= 0 || refundAmount.Amount > Amount.Amount)
            throw new ArgumentException("Invalid refund amount");

        var refundPayment = new Payment(
            Guid.NewGuid(),
            TenantId,
            $"REF-{PaymentNumber}",
            DateTime.UtcNow,
            refundAmount,
            PaymentMethod,
            PaymentType.Refund,
            createdBy,
            CustomerId,
            CustomerName,
            InvoiceId,
            InvoiceNumber,
            $"Refund for {PaymentNumber}: {refundReason}")
        {
            Status = PaymentStatus.Completed,
            ProcessedDate = DateTime.UtcNow,
            RefundAmount = refundAmount,
            RefundReason = refundReason
        };

        return refundPayment;
    }

    public void SetBankDetails(
        string? bankReference,
        string? bankName,
        string? accountNumber,
        string? chequeNumber,
        string modifiedBy)
    {
        BankReference = bankReference?.Trim();
        BankName = bankName?.Trim();
        AccountNumber = accountNumber?.Trim();
        ChequeNumber = chequeNumber?.Trim();
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy?.Trim() ?? throw new ArgumentException("ModifiedBy cannot be empty");
    }

    public void SetDigitalPaymentDetails(
        string? transactionId,
        string? paymentProcessor,
        string modifiedBy)
    {
        TransactionId = transactionId?.Trim();
        PaymentProcessor = paymentProcessor?.Trim();
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy?.Trim() ?? throw new ArgumentException("ModifiedBy cannot be empty");
    }

    public void SetFees(Money? processingFee, Money? bankCharges, string modifiedBy)
    {
        ProcessingFee = processingFee;
        BankCharges = bankCharges;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy?.Trim() ?? throw new ArgumentException("ModifiedBy cannot be empty");
    }

    public void MarkAsStoreCredit(string modifiedBy)
    {
        IsStoreCredit = true;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy?.Trim() ?? throw new ArgumentException("ModifiedBy cannot be empty");
    }

    public void MarkAsCommunityPayment(string communityGroupName, string modifiedBy)
    {
        IsCommunityPayment = true;
        CommunityGroupName = communityGroupName?.Trim();
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy?.Trim() ?? throw new ArgumentException("ModifiedBy cannot be empty");
    }

    public void SetLocation(string? location, string modifiedBy)
    {
        LocationOfPayment = location?.Trim();
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy?.Trim() ?? throw new ArgumentException("ModifiedBy cannot be empty");
    }

    public void UpdateReferenceNumber(string? referenceNumber, string modifiedBy)
    {
        ReferenceNumber = referenceNumber?.Trim();
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy?.Trim() ?? throw new ArgumentException("ModifiedBy cannot be empty");
    }

    public void AddAllocation(PaymentAllocation allocation)
    {
        if (allocation == null)
            throw new ArgumentNullException(nameof(allocation));

        if (Status != PaymentStatus.Pending)
            throw new InvalidOperationException("Cannot modify allocations for non-pending payment");

        _allocations.Add(allocation);
    }

    public void RemoveAllocation(Guid allocationId)
    {
        if (Status != PaymentStatus.Pending)
            throw new InvalidOperationException("Cannot modify allocations for non-pending payment");

        var allocation = _allocations.FirstOrDefault(x => x.Id == allocationId);
        if (allocation != null)
        {
            _allocations.Remove(allocation);
        }
    }

    public Money TotalAllocated => _allocations.Aggregate(
        Money.Zero(Enum.Parse<CurrencyCode>(Currency)),
        (sum, allocation) => sum.Add(allocation.Amount));

    public Money UnallocatedAmount => Amount.Subtract(TotalAllocated);

    public bool IsFullyAllocated => UnallocatedAmount.Amount == 0;

    public bool IsOverAllocated => UnallocatedAmount.Amount < 0;

    public bool IsCompleted => Status == PaymentStatus.Completed;

    public bool IsPending => Status == PaymentStatus.Pending;

    public bool IsFailed => Status == PaymentStatus.Failed;

    public bool IsCancelled => Status == PaymentStatus.Cancelled;

    public bool IsRefund => PaymentType == PaymentType.Refund;
}

/// <summary>
/// Payment Allocation entity for tracking how payments are allocated to invoices or accounts
/// </summary>
[Table("PaymentAllocations")]
public class PaymentAllocation : Entity
{
    public override Guid Id { get; protected set; }
    public Guid PaymentId { get; private set; }

    public Guid? InvoiceId { get; private set; }

    [StringLength(50)]
    public string? InvoiceNumber { get; private set; }

    public Guid? AccountId { get; private set; }

    [StringLength(200)]
    public string? AccountName { get; private set; }

    public Money Amount { get; private set; } = Money.Zero(CurrencyCode.ZAR);

    [StringLength(500)]
    public string? Description { get; private set; }

    public DateTime AllocationDate { get; private set; } = DateTime.UtcNow;

    [StringLength(100)]
    public string? AllocatedBy { get; private set; }

    // Navigation properties
    public Payment Payment { get; private set; } = null!;

    public Invoice? Invoice { get; private set; }

    private PaymentAllocation() : base() { } // EF Core

    public PaymentAllocation(
        Guid id,
        string tenantId,
        Guid paymentId,
        Money amount,
        string allocatedBy,
        Guid? invoiceId = null,
        string? invoiceNumber = null,
        Guid? accountId = null,
        string? accountName = null,
        string? description = null)
    {
        PaymentId = paymentId;
        Amount = amount ?? throw new ArgumentNullException(nameof(amount));
        AllocatedBy = allocatedBy?.Trim() ?? throw new ArgumentException("AllocatedBy cannot be empty");
        InvoiceId = invoiceId;
        InvoiceNumber = invoiceNumber?.Trim();
        AccountId = accountId;
        AccountName = accountName?.Trim();
        Description = description?.Trim();
        AllocationDate = DateTime.UtcNow;
    }

    public static PaymentAllocation Create(
        string tenantId,
        Guid paymentId,
        Money amount,
        string allocatedBy,
        Guid? invoiceId = null,
        string? invoiceNumber = null,
        Guid? accountId = null,
        string? accountName = null,
        string? description = null)
    {
        return new PaymentAllocation(Guid.NewGuid(), tenantId, paymentId, amount, allocatedBy,
            invoiceId, invoiceNumber, accountId, accountName, description);
    }

    public void UpdateAmount(Money amount, string modifiedBy)
    {
        Amount = amount ?? throw new ArgumentNullException(nameof(amount));
        AllocatedBy = modifiedBy?.Trim() ?? throw new ArgumentException("ModifiedBy cannot be empty");
        AllocationDate = DateTime.UtcNow;
    }

    public void UpdateDescription(string? description, string modifiedBy)
    {
        Description = description?.Trim();
        AllocatedBy = modifiedBy?.Trim() ?? throw new ArgumentException("ModifiedBy cannot be empty");
        AllocationDate = DateTime.UtcNow;
    }

    public bool IsInvoiceAllocation => InvoiceId.HasValue;

    public bool IsAccountAllocation => AccountId.HasValue;
}
