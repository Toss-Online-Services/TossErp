namespace Collaboration.Application.DTOs;

/// <summary>
/// Data Transfer Object for supplier quotations
/// </summary>
public record SupplierQuotationDto
{
    public Guid Id { get; init; }
    public Guid CampaignId { get; init; }
    public Guid SupplierId { get; init; }
    public string SupplierName { get; init; } = string.Empty;
    public string SupplierEmail { get; init; } = string.Empty;
    public string SupplierPhone { get; init; } = string.Empty;
    public QuotationStatus Status { get; init; }
    public decimal UnitPrice { get; init; }
    public decimal BulkDiscountPercentage { get; init; }
    public int MinQuantity { get; init; }
    public int MaxQuantity { get; init; }
    public string ProductDescription { get; init; } = string.Empty;
    public string? ProductSpecifications { get; init; }
    public string? TermsAndConditions { get; init; }
    public DateTime ValidUntil { get; init; }
    public DateTime? AcceptedAt { get; init; }
    public Guid? AcceptedBy { get; init; }
    public string? RejectionReason { get; init; }
    public DateTime? RejectedAt { get; init; }
    public Guid? RejectedBy { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
    public bool IsExpired { get; init; }
    public decimal DiscountedPrice { get; init; }
}

/// <summary>
/// DTO for creating a new supplier quotation
/// </summary>
public record CreateSupplierQuotationDto
{
    public Guid CampaignId { get; init; }
    public Guid SupplierId { get; init; }
    public string SupplierName { get; init; } = string.Empty;
    public string SupplierEmail { get; init; } = string.Empty;
    public string SupplierPhone { get; init; } = string.Empty;
    public decimal UnitPrice { get; init; }
    public decimal BulkDiscountPercentage { get; init; }
    public int MinQuantity { get; init; }
    public int MaxQuantity { get; init; }
    public string ProductDescription { get; init; } = string.Empty;
    public string? ProductSpecifications { get; init; }
    public string? TermsAndConditions { get; init; }
    public DateTime ValidUntil { get; init; }
}

/// <summary>
/// DTO for updating an existing supplier quotation
/// </summary>
public record UpdateSupplierQuotationDto
{
    public Guid Id { get; init; }
    public decimal UnitPrice { get; init; }
    public decimal BulkDiscountPercentage { get; init; }
    public int MinQuantity { get; init; }
    public int MaxQuantity { get; init; }
    public string ProductDescription { get; init; } = string.Empty;
    public string? ProductSpecifications { get; init; }
    public string? TermsAndConditions { get; init; }
    public DateTime ValidUntil { get; init; }
}

/// <summary>
/// DTO for supplier quotation comparison
/// </summary>
public record QuotationComparisonDto
{
    public Guid CampaignId { get; init; }
    public int RequestedQuantity { get; init; }
    public List<SupplierQuotationDto> Quotations { get; init; } = new();
    public SupplierQuotationDto? BestPriceQuotation { get; init; }
    public decimal TotalBestPrice { get; init; }
    public decimal TotalSavings { get; init; }
    public decimal AverageUnitPrice { get; init; }
    public decimal HighestDiscount { get; init; }
}

/// <summary>
/// DTO for supplier quotation response
/// </summary>
public record QuotationResponseDto
{
    public bool Success { get; init; }
    public string Message { get; init; } = string.Empty;
    public SupplierQuotationDto? Quotation { get; init; }
    public List<string> Errors { get; init; } = new();
}
