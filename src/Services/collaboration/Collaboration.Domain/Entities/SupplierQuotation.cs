using Collaboration.Domain.Common;
using Collaboration.Domain.Enums;

namespace Collaboration.Domain.Entities;

/// <summary>
/// Represents a supplier quotation for a group-buy campaign
/// </summary>
public class SupplierQuotation : Entity
{
    public Guid CampaignId { get; private set; }
    public Guid SupplierId { get; private set; }
    public string SupplierName { get; private set; }
    public string SupplierEmail { get; private set; }
    public string SupplierPhone { get; private set; }
    public QuotationStatus Status { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal BulkDiscountPercentage { get; private set; }
    public int MinQuantity { get; private set; }
    public int MaxQuantity { get; private set; }
    public string ProductDescription { get; private set; }
    public string? ProductSpecifications { get; private set; }
    public string? TermsAndConditions { get; private set; }
    public DateTime ValidUntil { get; private set; }
    public DateTime? AcceptedAt { get; private set; }
    public Guid? AcceptedBy { get; private set; }
    public string? RejectionReason { get; private set; }
    public DateTime? RejectedAt { get; private set; }
    public Guid? RejectedBy { get; private set; }
    public Guid TenantId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    // Navigation properties
    public Campaign Campaign { get; private set; } = null!;

    // Private constructor for EF Core
    private SupplierQuotation() 
    { 
        SupplierName = string.Empty;
        SupplierEmail = string.Empty;
        SupplierPhone = string.Empty;
        ProductDescription = string.Empty;
    }

    public SupplierQuotation(
        Guid campaignId,
        Guid supplierId,
        string supplierName,
        string supplierEmail,
        string supplierPhone,
        decimal unitPrice,
        decimal bulkDiscountPercentage,
        int minQuantity,
        int maxQuantity,
        string productDescription,
        string? productSpecifications,
        string? termsAndConditions,
        DateTime validUntil,
        Guid tenantId)
    {
        if (campaignId == Guid.Empty)
            throw new ArgumentException("Campaign ID cannot be empty", nameof(campaignId));

        if (supplierId == Guid.Empty)
            throw new ArgumentException("Supplier ID cannot be empty", nameof(supplierId));

        if (string.IsNullOrWhiteSpace(supplierName))
            throw new ArgumentException("Supplier name cannot be empty", nameof(supplierName));

        if (string.IsNullOrWhiteSpace(supplierEmail))
            throw new ArgumentException("Supplier email cannot be empty", nameof(supplierEmail));

        if (string.IsNullOrWhiteSpace(supplierPhone))
            throw new ArgumentException("Supplier phone cannot be empty", nameof(supplierPhone));

        if (unitPrice <= 0)
            throw new ArgumentException("Unit price must be greater than 0", nameof(unitPrice));

        if (bulkDiscountPercentage < 0 || bulkDiscountPercentage > 100)
            throw new ArgumentException("Bulk discount percentage must be between 0 and 100", nameof(bulkDiscountPercentage));

        if (minQuantity <= 0)
            throw new ArgumentException("Minimum quantity must be greater than 0", nameof(minQuantity));

        if (maxQuantity < minQuantity)
            throw new ArgumentException("Maximum quantity cannot be less than minimum quantity", nameof(maxQuantity));

        if (string.IsNullOrWhiteSpace(productDescription))
            throw new ArgumentException("Product description cannot be empty", nameof(productDescription));

        if (validUntil <= DateTime.UtcNow)
            throw new ArgumentException("Valid until date must be in the future", nameof(validUntil));

        CampaignId = campaignId;
        SupplierId = supplierId;
        SupplierName = supplierName;
        SupplierEmail = supplierEmail;
        SupplierPhone = supplierPhone;
        Status = QuotationStatus.Submitted;
        UnitPrice = unitPrice;
        BulkDiscountPercentage = bulkDiscountPercentage;
        MinQuantity = minQuantity;
        MaxQuantity = maxQuantity;
        ProductDescription = productDescription;
        ProductSpecifications = productSpecifications;
        TermsAndConditions = termsAndConditions;
        ValidUntil = validUntil;
        TenantId = tenantId;
        CreatedAt = DateTime.UtcNow;
    }

    public void UpdateQuotation(
        decimal unitPrice,
        decimal bulkDiscountPercentage,
        int minQuantity,
        int maxQuantity,
        string productDescription,
        string? productSpecifications,
        string? termsAndConditions,
        DateTime validUntil)
    {
        if (Status != QuotationStatus.Submitted)
            throw new InvalidOperationException("Cannot update quotation after it has been accepted or rejected");

        if (unitPrice <= 0)
            throw new ArgumentException("Unit price must be greater than 0", nameof(unitPrice));

        if (bulkDiscountPercentage < 0 || bulkDiscountPercentage > 100)
            throw new ArgumentException("Bulk discount percentage must be between 0 and 100", nameof(bulkDiscountPercentage));

        if (minQuantity <= 0)
            throw new ArgumentException("Minimum quantity must be greater than 0", nameof(minQuantity));

        if (maxQuantity < minQuantity)
            throw new ArgumentException("Maximum quantity cannot be less than minimum quantity", nameof(maxQuantity));

        if (string.IsNullOrWhiteSpace(productDescription))
            throw new ArgumentException("Product description cannot be empty", nameof(productDescription));

        if (validUntil <= DateTime.UtcNow)
            throw new ArgumentException("Valid until date must be in the future", nameof(validUntil));

        UnitPrice = unitPrice;
        BulkDiscountPercentage = bulkDiscountPercentage;
        MinQuantity = minQuantity;
        MaxQuantity = maxQuantity;
        ProductDescription = productDescription;
        ProductSpecifications = productSpecifications;
        TermsAndConditions = termsAndConditions;
        ValidUntil = validUntil;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Accept(Guid acceptedBy)
    {
        if (Status != QuotationStatus.Submitted)
            throw new InvalidOperationException("Only submitted quotations can be accepted");

        if (ValidUntil < DateTime.UtcNow)
            throw new InvalidOperationException("Cannot accept expired quotation");

        Status = QuotationStatus.Accepted;
        AcceptedAt = DateTime.UtcNow;
        AcceptedBy = acceptedBy;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Reject(string reason, Guid rejectedBy)
    {
        if (Status != QuotationStatus.Submitted)
            throw new InvalidOperationException("Only submitted quotations can be rejected");

        if (string.IsNullOrWhiteSpace(reason))
            throw new ArgumentException("Rejection reason is required", nameof(reason));

        Status = QuotationStatus.Rejected;
        RejectionReason = reason;
        RejectedAt = DateTime.UtcNow;
        RejectedBy = rejectedBy;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Withdraw()
    {
        if (Status != QuotationStatus.Submitted)
            throw new InvalidOperationException("Only submitted quotations can be withdrawn");

        Status = QuotationStatus.Withdrawn;
        UpdatedAt = DateTime.UtcNow;
    }

    public decimal CalculateDiscountedPrice(int quantity)
    {
        if (quantity < MinQuantity)
            throw new ArgumentException($"Quantity must be at least {MinQuantity}", nameof(quantity));

        if (quantity > MaxQuantity)
            throw new ArgumentException($"Quantity cannot exceed {MaxQuantity}", nameof(quantity));

        var discount = UnitPrice * (BulkDiscountPercentage / 100);
        return UnitPrice - discount;
    }

    public decimal CalculateTotalPrice(int quantity)
    {
        var discountedPrice = CalculateDiscountedPrice(quantity);
        return discountedPrice * quantity;
    }

    public bool IsExpired => DateTime.UtcNow > ValidUntil;
    public bool CanAccept => Status == QuotationStatus.Submitted && !IsExpired;
    public bool CanReject => Status == QuotationStatus.Submitted;
    public bool CanUpdate => Status == QuotationStatus.Submitted;
    public bool IsAccepted => Status == QuotationStatus.Accepted;
    public bool IsRejected => Status == QuotationStatus.Rejected;
}
