using MediatR;
using TossErp.Stock.Application.Common.DTOs;

namespace TossErp.Stock.Application.Items.Commands.UpdateItem;

/// <summary>
/// Command for updating an existing item
/// </summary>
public record UpdateItemCommand : IRequest<ItemDto>
{
    /// <summary>
    /// The unique identifier of the item to update
    /// </summary>
    public Guid Id { get; init; }

    // Basic Information
    public string? ItemName { get; init; }
    public string? Description { get; init; }
    public string? Brand { get; init; }
    public string? ItemGroup { get; init; }

    // Pricing
    public decimal? StandardRate { get; init; }
    public decimal? MinimumPrice { get; init; }

    // Physical Properties
    public decimal? WeightPerUnit { get; init; }
    public decimal? WeightUOM { get; init; }
    public decimal? Length { get; init; }
    public decimal? Width { get; init; }
    public decimal? Height { get; init; }

    // Settings
    public string? ItemType { get; init; }
    public string? ValuationMethod { get; init; }
    public bool? Disabled { get; init; }

    // Website Settings
    public bool? ShowInWebsite { get; init; }
    public bool? ShowVariantInWebsite { get; init; }
    public bool? AllowInWebsite { get; init; }

    // Purchase Settings
    public bool? IsPurchaseItem { get; init; }
    public bool? IncludeInPurchase { get; init; }
    public bool? IncludeInSubcontracting { get; init; }

    // Sales Settings
    public bool? IsSalesItem { get; init; }
    public bool? IncludeInSales { get; init; }
    public bool? IncludeInPOS { get; init; }

    // Manufacturing Settings
    public bool? IsManufacturingItem { get; init; }
    public bool? IncludeInManufacturing { get; init; }
    public bool? IncludeInBOM { get; init; }

    // Quality Settings
    public bool? IsQualityInspectionRequired { get; init; }
    public bool? IncludeInQualityInspection { get; init; }

    // Asset Settings
    public bool? IsAsset { get; init; }
    public bool? AutoCreateAsset { get; init; }
    public string? AssetCategory { get; init; }

    // Subscription Settings
    public bool? IsSubscriptionProduct { get; init; }
    public int? SubscriptionPeriod { get; init; }
    public string? SubscriptionPeriodType { get; init; }

    // Warranty Settings
    public bool? HasWarranty { get; init; }
    public int? WarrantyPeriod { get; init; }
    public string? WarrantyPeriodType { get; init; }

    // SEO Settings
    public string? MetaTitle { get; init; }
    public string? MetaDescription { get; init; }
    public string? MetaKeywords { get; init; }

    // Customer Settings
    public bool? AllowCustomerReviews { get; init; }
    public bool? AllowCustomerRating { get; init; }
    public bool? AllowCustomerFeedback { get; init; }
}

public record UpdateProductWarehouseInventoryCommand
{
    public int Id { get; init; }
    public int WarehouseId { get; init; }
    public decimal StockQuantity { get; init; }
    public decimal ReservedQuantity { get; init; }
} 
