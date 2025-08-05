using TossErp.Stock.Application.Common.DTOs;
using TossErp.Stock.Domain.Enums;

namespace TossErp.Stock.Application.Common.Services;

/// <summary>
/// Item service interface - migrated from nopCommerce IProductService
/// Provides comprehensive item management functionality for TOSS ERP
/// </summary>
public interface IItemService
{
    #region Core Item Operations

    /// <summary>
    /// Get item by ID
    /// </summary>
    Task<ItemDto?> GetItemByIdAsync(Guid itemId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get items by IDs
    /// </summary>
    Task<IList<ItemDto>> GetItemsByIdsAsync(Guid[] itemIds, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get item by item code
    /// </summary>
    Task<ItemDto?> GetItemByCodeAsync(string itemCode, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get items by item codes
    /// </summary>
    Task<IList<ItemDto>> GetItemsByCodesAsync(string[] itemCodes, CancellationToken cancellationToken = default);

    /// <summary>
    /// Create new item
    /// </summary>
    Task<ItemDto> CreateItemAsync(CreateItemRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update existing item
    /// </summary>
    Task<ItemDto> UpdateItemAsync(Guid itemId, UpdateItemRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete item (soft delete)
    /// </summary>
    Task DeleteItemAsync(Guid itemId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Permanently delete item
    /// </summary>
    Task PermanentlyDeleteItemAsync(Guid itemId, CancellationToken cancellationToken = default);

    #endregion

    #region Search and Filtering

    /// <summary>
    /// Search items with comprehensive filtering
    /// </summary>
    Task<SearchResult<ItemDto>> SearchItemsAsync(SearchItemsRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get items by item group
    /// </summary>
    Task<IList<ItemDto>> GetItemsByGroupAsync(string itemGroup, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get items by manufacturer
    /// </summary>
    Task<IList<ItemDto>> GetItemsByManufacturerAsync(Guid manufacturerId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get items by item type
    /// </summary>
    Task<IList<ItemDto>> GetItemsByTypeAsync(ItemType itemType, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get items by valuation method
    /// </summary>
    Task<IList<ItemDto>> GetItemsByValuationMethodAsync(ValuationMethod valuationMethod, CancellationToken cancellationToken = default);

    #endregion

    #region Stock Management

    /// <summary>
    /// Get low stock items
    /// </summary>
    Task<IList<ItemDto>> GetLowStockItemsAsync(Guid? warehouseId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get out of stock items
    /// </summary>
    Task<IList<ItemDto>> GetOutOfStockItemsAsync(Guid? warehouseId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get items with stock below reorder level
    /// </summary>
    Task<IList<ItemDto>> GetItemsBelowReorderLevelAsync(Guid? warehouseId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get total stock quantity for item
    /// </summary>
    Task<decimal> GetTotalStockQuantityAsync(Guid itemId, Guid? warehouseId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get available stock quantity for item
    /// </summary>
    Task<decimal> GetAvailableStockQuantityAsync(Guid itemId, Guid? warehouseId = null, CancellationToken cancellationToken = default);

    #endregion

    #region Pricing

    /// <summary>
    /// Get item pricing information
    /// </summary>
    Task<ItemPricingDto> GetItemPricingAsync(Guid itemId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update item pricing
    /// </summary>
    Task UpdateItemPricingAsync(Guid itemId, UpdateItemPricingRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get tier prices for item
    /// </summary>
    Task<IList<ItemTierPriceDto>> GetTierPricesAsync(Guid itemId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Add tier price to item
    /// </summary>
    Task AddTierPriceAsync(Guid itemId, AddTierPriceRequest request, CancellationToken cancellationToken = default);

    #endregion

    #region Related Items

    /// <summary>
    /// Get related items
    /// </summary>
    Task<IList<ItemDto>> GetRelatedItemsAsync(Guid itemId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Add related item
    /// </summary>
    Task AddRelatedItemAsync(Guid itemId, Guid relatedItemId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Remove related item
    /// </summary>
    Task RemoveRelatedItemAsync(Guid itemId, Guid relatedItemId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get cross-sell items
    /// </summary>
    Task<IList<ItemDto>> GetCrossSellItemsAsync(Guid itemId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Add cross-sell item
    /// </summary>
    Task AddCrossSellItemAsync(Guid itemId, Guid crossSellItemId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Remove cross-sell item
    /// </summary>
    Task RemoveCrossSellItemAsync(Guid itemId, Guid crossSellItemId, CancellationToken cancellationToken = default);

    #endregion

    #region Inventory Management

    /// <summary>
    /// Adjust inventory for item
    /// </summary>
    Task AdjustInventoryAsync(Guid itemId, decimal quantityChange, string? message = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Book reserved inventory
    /// </summary>
    Task BookReservedInventoryAsync(Guid itemId, Guid warehouseId, decimal quantity, string? message = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Reverse booked inventory
    /// </summary>
    Task<decimal> ReverseBookedInventoryAsync(Guid itemId, Guid warehouseId, decimal quantity, string? message = null, CancellationToken cancellationToken = default);

    #endregion

    #region Status Management

    /// <summary>
    /// Enable item
    /// </summary>
    Task EnableItemAsync(Guid itemId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Disable item
    /// </summary>
    Task DisableItemAsync(Guid itemId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Activate item
    /// </summary>
    Task ActivateItemAsync(Guid itemId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deactivate item
    /// </summary>
    Task DeactivateItemAsync(Guid itemId, CancellationToken cancellationToken = default);

    #endregion

    #region Bulk Operations

    /// <summary>
    /// Bulk update items
    /// </summary>
    Task BulkUpdateItemsAsync(BulkUpdateItemsRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Bulk delete items
    /// </summary>
    Task BulkDeleteItemsAsync(Guid[] itemIds, CancellationToken cancellationToken = default);

    /// <summary>
    /// Bulk enable items
    /// </summary>
    Task BulkEnableItemsAsync(Guid[] itemIds, CancellationToken cancellationToken = default);

    /// <summary>
    /// Bulk disable items
    /// </summary>
    Task BulkDisableItemsAsync(Guid[] itemIds, CancellationToken cancellationToken = default);

    #endregion

    #region Analytics and Reporting

    /// <summary>
    /// Get item statistics
    /// </summary>
    Task<ItemStatisticsDto> GetItemStatisticsAsync(Guid itemId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get items with applied discounts
    /// </summary>
    Task<IList<ItemDto>> GetItemsWithDiscountsAsync(Guid? discountId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get recently viewed items
    /// </summary>
    Task<IList<ItemDto>> GetRecentlyViewedItemsAsync(Guid customerId, int count = 10, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get popular items
    /// </summary>
    Task<IList<ItemDto>> GetPopularItemsAsync(int count = 10, CancellationToken cancellationToken = default);

    #endregion
}

#region Request/Response DTOs

public record CreateItemRequest
{
    public string ItemCode { get; init; } = string.Empty;
    public string ItemName { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string ItemGroup { get; init; } = string.Empty;
    public string? Brand { get; init; }
    public string StockUOM { get; init; } = string.Empty;
    public string ItemType { get; init; } = string.Empty;
    public string ValuationMethod { get; init; } = string.Empty;
    public string Company { get; init; } = string.Empty;
    
    // Optional properties
    public decimal? StandardRate { get; init; }
    public decimal? MinimumPrice { get; init; }
    public decimal? WeightPerUnit { get; init; }
    public decimal? WeightUOM { get; init; }
    public decimal? Length { get; init; }
    public decimal? Width { get; init; }
    public decimal? Height { get; init; }
    
    // Settings
    public bool ShowInWebsite { get; init; } = true;
    public bool IsPurchaseItem { get; init; } = true;
    public bool IsSalesItem { get; init; } = true;
    public bool IsManufacturingItem { get; init; } = false;
    public bool IsQualityInspectionRequired { get; init; } = false;
    public bool IsAsset { get; init; } = false;
    public bool IsSubscriptionProduct { get; init; } = false;
    public bool HasWarranty { get; init; } = false;
    public bool AllowCustomerReviews { get; init; } = true;
}

public record UpdateItemRequest
{
    public string? ItemName { get; init; }
    public string? Description { get; init; }
    public string? ItemGroup { get; init; }
    public string? Brand { get; init; }
    public string? StockUOM { get; init; }
    public string? ItemType { get; init; }
    public string? ValuationMethod { get; init; }
    public string? Company { get; init; }
    
    // Optional properties
    public decimal? StandardRate { get; init; }
    public decimal? MinimumPrice { get; init; }
    public decimal? WeightPerUnit { get; init; }
    public decimal? WeightUOM { get; init; }
    public decimal? Length { get; init; }
    public decimal? Width { get; init; }
    public decimal? Height { get; init; }
    
    // Settings
    public bool? ShowInWebsite { get; init; }
    public bool? IsPurchaseItem { get; init; }
    public bool? IsSalesItem { get; init; }
    public bool? IsManufacturingItem { get; init; }
    public bool? IsQualityInspectionRequired { get; init; }
    public bool? IsAsset { get; init; }
    public bool? IsSubscriptionProduct { get; init; }
    public bool? HasWarranty { get; init; }
    public bool? AllowCustomerReviews { get; init; }
}

public record SearchItemsRequest
{
    public string? Keywords { get; init; }
    public string? ItemGroup { get; init; }
    public string? Brand { get; init; }
    public ItemType? ItemType { get; init; }
    public ValuationMethod? ValuationMethod { get; init; }
    public bool? IsStockItem { get; init; }
    public bool? IsPurchaseItem { get; init; }
    public bool? IsSalesItem { get; init; }
    public bool? IsManufacturingItem { get; init; }
    public bool? Disabled { get; init; }
    public decimal? MinPrice { get; init; }
    public decimal? MaxPrice { get; init; }
    public Guid? WarehouseId { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
    public string? SortBy { get; init; }
    public bool SortDescending { get; init; } = false;
}

public record SearchResult<T>
{
    public IList<T> Items { get; init; } = new List<T>();
    public int TotalCount { get; init; }
    public int Page { get; init; }
    public int PageSize { get; init; }
    public int TotalPages { get; init; }
}

public record ItemPricingDto
{
    public Guid ItemId { get; init; }
    public decimal? StandardRate { get; init; }
    public decimal? LastPurchaseRate { get; init; }
    public decimal? BaseRate { get; init; }
    public decimal? BaseAmount { get; init; }
    public decimal? BasePriceListRate { get; init; }
    public decimal? MinimumPrice { get; init; }
    public decimal? LastPrice { get; init; }
    public IList<ItemTierPriceDto> TierPrices { get; init; } = new List<ItemTierPriceDto>();
}

public record UpdateItemPricingRequest
{
    public decimal? StandardRate { get; init; }
    public decimal? MinimumPrice { get; init; }
    public decimal? BaseRate { get; init; }
    public decimal? BaseAmount { get; init; }
    public decimal? BasePriceListRate { get; init; }
}

public record ItemTierPriceDto
{
    public Guid Id { get; init; }
    public Guid ItemId { get; init; }
    public decimal Quantity { get; init; }
    public decimal Price { get; init; }
    public string? CustomerGroup { get; init; }
    public DateTime? ValidFrom { get; init; }
    public DateTime? ValidTo { get; init; }
}

public record AddTierPriceRequest
{
    public decimal Quantity { get; init; }
    public decimal Price { get; init; }
    public string? CustomerGroup { get; init; }
    public DateTime? ValidFrom { get; init; }
    public DateTime? ValidTo { get; init; }
}

public record BulkUpdateItemsRequest
{
    public Guid[] ItemIds { get; init; } = Array.Empty<Guid>();
    public UpdateItemRequest Updates { get; init; } = new();
}

public record ItemStatisticsDto
{
    public Guid ItemId { get; init; }
    public decimal TotalStockQuantity { get; init; }
    public decimal AvailableStockQuantity { get; init; }
    public decimal ReservedStockQuantity { get; init; }
    public int TotalOrders { get; init; }
    public decimal TotalSalesAmount { get; init; }
    public decimal AverageRating { get; init; }
    public int ReviewCount { get; init; }
    public DateTime LastPurchaseDate { get; init; }
    public DateTime LastSaleDate { get; init; }
}

#endregion 
