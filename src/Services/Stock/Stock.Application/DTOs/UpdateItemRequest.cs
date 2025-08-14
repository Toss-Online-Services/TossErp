namespace TossErp.Stock.Application.DTOs;

/// <summary>
/// Request DTO for updating an existing item
/// </summary>
public record UpdateItemRequest
{
    /// <summary>
    /// Item ID to update
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Item barcode
    /// </summary>
    public string? Barcode { get; init; }

    /// <summary>
    /// Item name
    /// </summary>
    public string? Name { get; init; }

    /// <summary>
    /// Item description
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// Item category/group
    /// </summary>
    public string? Category { get; init; }

    /// <summary>
    /// Unit of measurement
    /// </summary>
    public string? Unit { get; init; }

    /// <summary>
    /// Selling price
    /// </summary>
    public decimal? SellingPrice { get; init; }

    /// <summary>
    /// Cost price
    /// </summary>
    public decimal? CostPrice { get; init; }

    /// <summary>
    /// Reorder level
    /// </summary>
    public decimal? ReorderLevel { get; init; }

    /// <summary>
    /// Reorder quantity
    /// </summary>
    public decimal? ReorderQty { get; init; }

    /// <summary>
    /// Maximum stock level
    /// </summary>
    public decimal? MaxStock { get; init; }

    /// <summary>
    /// Whether the item is active
    /// </summary>
    public bool? IsActive { get; init; }

    /// <summary>
    /// Item type (Stock, Service, etc.)
    /// </summary>
    public string? ItemType { get; init; }

    /// <summary>
    /// Whether this is a stock item
    /// </summary>
    public bool? IsStockItem { get; init; }

    /// <summary>
    /// Brand name
    /// </summary>
    public string? Brand { get; init; }

    /// <summary>
    /// Model number
    /// </summary>
    public string? Model { get; init; }

    /// <summary>
    /// Color
    /// </summary>
    public string? Color { get; init; }

    /// <summary>
    /// Size
    /// </summary>
    public string? Size { get; init; }

    /// <summary>
    /// Weight
    /// </summary>
    public decimal? Weight { get; init; }

    /// <summary>
    /// Dimensions
    /// </summary>
    public string? Dimensions { get; init; }
}
