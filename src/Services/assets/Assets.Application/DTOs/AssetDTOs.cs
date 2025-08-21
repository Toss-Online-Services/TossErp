namespace TossErp.Assets.Application.DTOs;

/// <summary>
/// Asset data transfer object
/// </summary>
public class AssetDto
{
    public Guid Id { get; set; }
    public string TenantId { get; set; } = null!;
    public string AssetNumber { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public AssetCategory Category { get; set; }
    public AssetType Type { get; set; }
    public AssetStatus Status { get; set; }
    public string? SerialNumber { get; set; }
    public string? Model { get; set; }
    public string? Manufacturer { get; set; }
    public DateTime? PurchaseDate { get; set; }
    public decimal? PurchasePrice { get; set; }
    public string? PurchaseCurrency { get; set; }
    public decimal? CurrentValue { get; set; }
    public DateTime? WarrantyStartDate { get; set; }
    public DateTime? WarrantyEndDate { get; set; }
    public int? WarrantyPeriodMonths { get; set; }
    public string? WarrantyProvider { get; set; }
    public Guid? LocationId { get; set; }
    public string? LocationName { get; set; }
    public Guid? DepartmentId { get; set; }
    public string? DepartmentName { get; set; }
    public Guid? AssignedToUserId { get; set; }
    public string? AssignedToUserName { get; set; }
    public DateTime? LastMaintenanceDate { get; set; }
    public DateTime? NextMaintenanceDate { get; set; }
    public int MaintenanceIntervalDays { get; set; }
    public DepreciationMethod? DepreciationMethod { get; set; }
    public int? UsefulLifeYears { get; set; }
    public decimal? SalvageValue { get; set; }
    public decimal? AccumulatedDepreciation { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; } = null!;
    public DateTime? LastModified { get; set; }
    public string? LastModifiedBy { get; set; }
    public List<AssetDocumentDto> Documents { get; set; } = new();
    public List<MaintenanceRecordDto> MaintenanceRecords { get; set; } = new();
}

/// <summary>
/// Asset summary data transfer object for lists
/// </summary>
public class AssetSummaryDto
{
    public Guid Id { get; set; }
    public string AssetNumber { get; set; } = null!;
    public string Name { get; set; } = null!;
    public AssetCategory Category { get; set; }
    public AssetStatus Status { get; set; }
    public string? LocationName { get; set; }
    public string? AssignedToUserName { get; set; }
    public DateTime? NextMaintenanceDate { get; set; }
    public DateTime? WarrantyEndDate { get; set; }
    public decimal? CurrentValue { get; set; }
    public string? CurrentValueCurrency { get; set; }
}

/// <summary>
/// Asset document data transfer object
/// </summary>
public class AssetDocumentDto
{
    public Guid Id { get; set; }
    public string FileName { get; set; } = null!;
    public string? Description { get; set; }
    public DocumentType DocumentType { get; set; }
    public string FileUrl { get; set; } = null!;
    public string ContentType { get; set; } = null!;
    public long FileSize { get; set; }
    public DateTime UploadedAt { get; set; }
    public string UploadedBy { get; set; } = null!;
}

/// <summary>
/// Maintenance record data transfer object
/// </summary>
public class MaintenanceRecordDto
{
    public Guid Id { get; set; }
    public MaintenanceType Type { get; set; }
    public string? Description { get; set; }
    public DateTime ScheduledDate { get; set; }
    public DateTime? CompletedDate { get; set; }
    public MaintenanceStatus Status { get; set; }
    public MaintenancePriority Priority { get; set; }
    public string? TechniciansNotes { get; set; }
    public decimal? Cost { get; set; }
    public string? CostCurrency { get; set; }
    public string? ServiceProvider { get; set; }
    public string? ServiceProviderContact { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; } = null!;
    public List<AssetDocumentDto> Documents { get; set; } = new();
}

/// <summary>
/// Location data transfer object
/// </summary>
public class LocationDto
{
    public Guid Id { get; set; }
    public string TenantId { get; set; } = null!;
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public LocationType Type { get; set; }
    public Guid? ParentLocationId { get; set; }
    public string? ParentLocationName { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? PostalCode { get; set; }
    public string? Country { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public string? ContactPerson { get; set; }
    public string? ContactPhone { get; set; }
    public string? ContactEmail { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; } = null!;
    public int AssetCount { get; set; }
    public List<LocationDto> ChildLocations { get; set; } = new();
}

/// <summary>
/// Asset transfer data transfer object
/// </summary>
public class AssetTransferDto
{
    public Guid Id { get; set; }
    public Guid AssetId { get; set; }
    public string AssetNumber { get; set; } = null!;
    public string AssetName { get; set; } = null!;
    public TransferType TransferType { get; set; }
    public Guid? FromLocationId { get; set; }
    public string? FromLocationName { get; set; }
    public Guid? ToLocationId { get; set; }
    public string? ToLocationName { get; set; }
    public Guid? FromUserId { get; set; }
    public string? FromUserName { get; set; }
    public Guid? ToUserId { get; set; }
    public string? ToUserName { get; set; }
    public DateTime TransferDate { get; set; }
    public string? Reason { get; set; }
    public string? Notes { get; set; }
    public TransferStatus Status { get; set; }
    public DateTime? CompletedDate { get; set; }
    public string? CompletedBy { get; set; }
    public string InitiatedBy { get; set; } = null!;
    public DateTime InitiatedAt { get; set; }
}

/// <summary>
/// Asset valuation data transfer object
/// </summary>
public class AssetValuationDto
{
    public Guid Id { get; set; }
    public Guid AssetId { get; set; }
    public string AssetNumber { get; set; } = null!;
    public string AssetName { get; set; } = null!;
    public ValuationType ValuationType { get; set; }
    public DateTime ValuationDate { get; set; }
    public decimal FairValue { get; set; }
    public string Currency { get; set; } = null!;
    public string? ValuationMethod { get; set; }
    public string? Valuator { get; set; }
    public string? Notes { get; set; }
    public string? DocumentUrl { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; } = null!;
}

/// <summary>
/// Asset disposal data transfer object
/// </summary>
public class AssetDisposalDto
{
    public Guid Id { get; set; }
    public Guid AssetId { get; set; }
    public string AssetNumber { get; set; } = null!;
    public string AssetName { get; set; } = null!;
    public DisposalMethod Method { get; set; }
    public DateTime DisposalDate { get; set; }
    public string? Reason { get; set; }
    public decimal? SalePrice { get; set; }
    public string? SalePriceCurrency { get; set; }
    public string? Buyer { get; set; }
    public string? BuyerContact { get; set; }
    public string? Notes { get; set; }
    public DisposalStatus Status { get; set; }
    public DateTime? CompletedDate { get; set; }
    public string? CompletedBy { get; set; }
    public string InitiatedBy { get; set; } = null!;
    public DateTime InitiatedAt { get; set; }
    public List<AssetDocumentDto> Documents { get; set; } = new();
}

/// <summary>
/// Paged result wrapper
/// </summary>
public class PagedResult<T>
{
    public IEnumerable<T> Items { get; set; } = new List<T>();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;
}
