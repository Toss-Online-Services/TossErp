using MediatR;
using TossErp.Stock.Application.Common.DTOs;

namespace TossErp.Stock.Application.Items.Commands.CreateItem;

public record CreateItemCommand : IRequest<ItemDto>
{
    // Required properties for Item constructor
    public string ItemCode { get; init; } = string.Empty;
    public string ItemName { get; init; } = string.Empty;
    public string ItemGroup { get; init; } = string.Empty;
    public string StockUOM { get; init; } = string.Empty;
    public string ItemType { get; init; } = string.Empty;
    public string ValuationMethod { get; init; } = string.Empty;
    public string Company { get; init; } = string.Empty;
    
    // Optional properties
    public string? Description { get; init; }
    public string? Brand { get; init; }
    
    // Pricing (optional)
    public decimal? StandardRate { get; init; }
    public decimal? MinimumPrice { get; init; }
    
    // Inventory (optional)
    public decimal? WeightPerUnit { get; init; }
    public decimal? WeightUOM { get; init; }
    public decimal? ReOrderLevel { get; init; }
    public decimal? ReOrderQty { get; init; }
    public decimal? MaxQty { get; init; }
    public decimal? MinQty { get; init; }
    
    // Physical Dimensions (optional)
    public decimal? Length { get; init; }
    public decimal? Width { get; init; }
    public decimal? Height { get; init; }
    
    // SEO and Meta (optional)
    public string? MetaTitle { get; init; }
    public string? MetaDescription { get; init; }
    public string? MetaKeywords { get; init; }
    
    // Website Settings (optional)
    public bool ShowInWebsite { get; init; } = true;
    public bool ShowVariantInWebsite { get; init; } = true;
    public bool AllowInWebsite { get; init; } = true;
    
    // Purchase Settings (optional)
    public bool IsPurchaseItem { get; init; } = true;
    public bool IncludeInPurchase { get; init; } = true;
    public bool IncludeInSubcontracting { get; init; } = false;
    
    // Sales Settings (optional)
    public bool IsSalesItem { get; init; } = true;
    public bool IncludeInSales { get; init; } = true;
    public bool IncludeInPOS { get; init; } = true;
    
    // Manufacturing Settings (optional)
    public bool IsManufacturingItem { get; init; } = false;
    public bool IncludeInManufacturing { get; init; } = false;
    public bool IncludeInBOM { get; init; } = false;
    
    // Quality Settings (optional)
    public bool IsQualityInspectionRequired { get; init; } = false;
    public bool IncludeInQualityInspection { get; init; } = false;
    
    // Asset Settings (optional)
    public bool IsAsset { get; init; } = false;
    public bool AutoCreateAsset { get; init; } = false;
    public string? AssetCategory { get; init; }
    
    // Subscription Settings (optional)
    public bool IsSubscriptionProduct { get; init; } = false;
    public int? SubscriptionPeriod { get; init; }
    public string? SubscriptionPeriodType { get; init; }
    
    // Warranty Settings (optional)
    public bool HasWarranty { get; init; } = false;
    public int? WarrantyPeriod { get; init; }
    public string? WarrantyPeriodType { get; init; }
    
    // Customer Settings (optional)
    public bool AllowCustomerReviews { get; init; } = true;
    public bool AllowCustomerRating { get; init; } = true;
    public bool AllowCustomerFeedback { get; init; } = true;
    
    // Related entities (optional)
    public List<CreateItemVariantCommand> Variants { get; init; } = new();
    public List<CreateItemPriceCommand> Prices { get; init; } = new();
    public List<CreateItemSupplierCommand> Suppliers { get; init; } = new();
    public List<CreateItemCustomerCommand> Customers { get; init; } = new();
    public List<CreateItemBarcodeCommand> Barcodes { get; init; } = new();
    public List<CreateItemTaxCommand> Taxes { get; init; } = new();
    public List<CreateItemReorderCommand> ReorderLevels { get; init; } = new();
    public List<CreateItemAttributeCommand> Attributes { get; init; } = new();
    public List<CreateItemAlternativeCommand> Alternatives { get; init; } = new();
    public List<CreateItemManufacturerCommand> Manufacturers { get; init; } = new();
    public List<CreateItemWebsiteSpecificationCommand> WebsiteSpecifications { get; init; } = new();
    public List<CreateItemQualityInspectionParameterCommand> QualityInspectionParameters { get; init; } = new();
    public List<CreateUOMConversionDetailCommand> UOMConversions { get; init; } = new();
    public List<CreateProductWarehouseInventoryCommand> WarehouseInventories { get; init; } = new();
    public Guid WarehouseId { get; init; }
    public int DisplayOrder { get; init; }
}

public record CreateItemVariantCommand
{
    public string VariantName { get; init; } = string.Empty;
    public string VariantCode { get; init; } = string.Empty;
    public string? Description { get; init; }
    public bool Disabled { get; init; }
    public List<CreateItemVariantAttributeCommand> Attributes { get; init; } = new();
}

public record CreateItemVariantAttributeCommand
{
    public string Attribute { get; init; } = string.Empty;
    public string AttributeValue { get; init; } = string.Empty;
    public int DisplayOrder { get; init; }
}

public record CreateItemPriceCommand
{
    public string PriceList { get; init; } = string.Empty;
    public string Currency { get; init; } = string.Empty;
    public decimal Rate { get; init; }
    public decimal? MinimumQty { get; init; }
    public decimal? MaximumQty { get; init; }
    public DateTime? ValidFrom { get; init; }
    public DateTime? ValidUpto { get; init; }
    public bool Disabled { get; init; }
}

public record CreateItemSupplierCommand
{
    public string Supplier { get; init; } = string.Empty;
    public string? SupplierPartNo { get; init; }
    public decimal? LastPurchaseRate { get; init; }
    public decimal? MinimumQty { get; init; }
    public decimal? MaximumQty { get; init; }
    public decimal? LeadTimeDays { get; init; }
    public bool IsDefaultSupplier { get; init; }
    public bool Disabled { get; init; }
}

public record CreateItemCustomerCommand
{
    public string Customer { get; init; } = string.Empty;
    public string? CustomerItemCode { get; init; }
    public string? CustomerItemName { get; init; }
    public decimal? RefRate { get; init; }
    public string? RefCurrency { get; init; }
    public bool Disabled { get; init; }
}

public record CreateItemBarcodeCommand
{
    public string Barcode { get; init; } = string.Empty;
    public string BarcodeType { get; init; } = string.Empty;
    public bool IsPrimary { get; init; }
    public bool Disabled { get; init; }
}

public record CreateItemTaxCommand
{
    public string TaxType { get; init; } = string.Empty;
    public string TaxCategory { get; init; } = string.Empty;
    public string TaxCode { get; init; } = string.Empty;
    public decimal TaxRate { get; init; }
    public bool IsDefault { get; init; }
    public bool Disabled { get; init; }
}

public record CreateItemReorderCommand
{
    public string Warehouse { get; init; } = string.Empty;
    public decimal ReOrderLevel { get; init; }
    public decimal ReOrderQty { get; init; }
    public decimal MinQty { get; init; }
    public decimal MaxQty { get; init; }
    public bool Disabled { get; init; }
}

public record CreateItemAttributeCommand
{
    public string Attribute { get; init; } = string.Empty;
    public string AttributeValue { get; init; } = string.Empty;
    public int DisplayOrder { get; init; }
}

public record CreateItemAlternativeCommand
{
    public string AlternativeItem { get; init; } = string.Empty;
    public decimal ConversionFactor { get; init; }
    public bool Disabled { get; init; }
}

public record CreateItemManufacturerCommand
{
    public string Manufacturer { get; init; } = string.Empty;
    public string? ManufacturerPartNo { get; init; }
    public string? Comment { get; init; }
}

public record CreateItemWebsiteSpecificationCommand
{
    public string Label { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public int DisplayOrder { get; init; }
}

public record CreateItemQualityInspectionParameterCommand
{
    public string Parameter { get; init; } = string.Empty;
    public string Specification { get; init; } = string.Empty;
    public string AcceptanceCriteria { get; init; } = string.Empty;
}

public record CreateUOMConversionDetailCommand
{
    public string UOM { get; init; } = string.Empty;
    public decimal ConversionFactor { get; init; }
    public bool IsBaseUOM { get; init; }
    public bool Disabled { get; init; }
}

public record CreateProductWarehouseInventoryCommand
{
    public int WarehouseId { get; init; }
    public decimal StockQuantity { get; init; }
    public decimal ReservedQuantity { get; init; }
} 
