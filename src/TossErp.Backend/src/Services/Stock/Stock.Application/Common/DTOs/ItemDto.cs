namespace TossErp.Stock.Application.Common.DTOs;

/// <summary>
/// Data Transfer Object for Item entity
/// </summary>
public class ItemDto
{
    public Guid Id { get; set; }
    public string ItemCode { get; set; } = string.Empty;
    public string ItemName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string ItemGroup { get; set; } = string.Empty;
    public string? Brand { get; set; }
    public string StockUOM { get; set; } = string.Empty;
    public string ItemType { get; set; } = string.Empty;
    public string ValuationMethod { get; set; } = string.Empty;
    public string ItemStatus { get; set; } = string.Empty;
    public string PriorityLevel { get; set; } = string.Empty;
    public string Company { get; set; } = string.Empty;
    
    // Pricing
    public decimal? StandardRate { get; set; }
    public decimal? LastPurchaseRate { get; set; }
    public decimal? BaseRate { get; set; }
    public decimal? BaseAmount { get; set; }
    public decimal? BasePriceListRate { get; set; }
    public decimal? MinimumPrice { get; set; }
    public decimal? LastPrice { get; set; }
    
    // Inventory
    public decimal? WeightPerUnit { get; set; }
    public decimal? WeightUOM { get; set; }
    public decimal? ReOrderLevel { get; set; }
    public decimal? ReOrderQty { get; set; }
    public decimal? MaxQty { get; set; }
    public decimal? MinQty { get; set; }
    
    // Physical Dimensions
    public decimal? Length { get; set; }
    public decimal? Width { get; set; }
    public decimal? Height { get; set; }
    
    // Flags
    public bool IsStockItem { get; set; }
    public bool Disabled { get; set; }
    public bool Deleted { get; set; }
    
    // SEO and Meta
    public string? MetaTitle { get; set; }
    public string? MetaDescription { get; set; }
    public string? MetaKeywords { get; set; }
    
    // Website Settings
    public bool ShowInWebsite { get; set; }
    public bool ShowVariantInWebsite { get; set; }
    public bool AllowInWebsite { get; set; }
    
    // Purchase Settings
    public bool IsPurchaseItem { get; set; }
    public bool IncludeInPurchase { get; set; }
    public bool IncludeInSubcontracting { get; set; }
    
    // Sales Settings
    public bool IsSalesItem { get; set; }
    public bool IncludeInSales { get; set; }
    public bool IncludeInPOS { get; set; }
    
    // Manufacturing Settings
    public bool IsManufacturingItem { get; set; }
    public bool IncludeInManufacturing { get; set; }
    public bool IncludeInBOM { get; set; }
    
    // Quality Settings
    public bool IsQualityInspectionRequired { get; set; }
    public bool IncludeInQualityInspection { get; set; }
    
    // Asset Settings
    public bool IsAsset { get; set; }
    public bool AutoCreateAsset { get; set; }
    public string? AssetCategory { get; set; }
    
    // Subscription Settings
    public bool IsSubscriptionProduct { get; set; }
    public int? SubscriptionPeriod { get; set; }
    public string? SubscriptionPeriodType { get; set; }
    
    // Warranty Settings
    public bool HasWarranty { get; set; }
    public int? WarrantyPeriod { get; set; }
    public string? WarrantyPeriodType { get; set; }
    
    // Customer Settings
    public bool AllowCustomerReviews { get; set; }
    public bool AllowCustomerRating { get; set; }
    public bool AllowCustomerFeedback { get; set; }
    
    // Audit
    public DateTime Created { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime? LastModified { get; set; }
    public string? LastModifiedBy { get; set; }
    
    // Navigation Properties
    public List<ItemVariantDto> Variants { get; set; } = new();
    public List<ItemPriceDto> Prices { get; set; } = new();
    public List<ItemSupplierDto> Suppliers { get; set; } = new();
    public List<ItemCustomerDto> Customers { get; set; } = new();
    public List<ItemBarcodeDto> Barcodes { get; set; } = new();
    public List<ItemTaxDto> Taxes { get; set; } = new();
    public List<ItemReorderDto> ReorderLevels { get; set; } = new();
    public List<ItemAttributeDto> Attributes { get; set; } = new();
    public List<ItemAlternativeDto> Alternatives { get; set; } = new();
    public List<ItemManufacturerDto> Manufacturers { get; set; } = new();
    public List<ItemWebsiteSpecificationDto> WebsiteSpecifications { get; set; } = new();
    public List<ItemQualityInspectionParameterDto> QualityInspectionParameters { get; set; } = new();
    public List<UOMConversionDetailDto> UOMConversions { get; set; } = new();
    public List<ProductWarehouseInventoryDto> WarehouseInventories { get; set; } = new();
} 
