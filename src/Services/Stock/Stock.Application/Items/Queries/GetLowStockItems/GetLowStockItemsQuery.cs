using MediatR;
using TossErp.Stock.Application.Common.DTOs;

namespace TossErp.Stock.Application.Items.Queries.GetLowStockItems;

/// <summary>
/// Query for retrieving items that are below their reorder level
/// </summary>
public record GetLowStockItemsQuery : IRequest<GetLowStockItemsResponse>
{
    /// <summary>
    /// Filter by warehouse ID
    /// </summary>
    public Guid? WarehouseId { get; init; }

    /// <summary>
    /// Filter by item group
    /// </summary>
    public string? ItemGroup { get; init; }

    /// <summary>
    /// Filter by item type
    /// </summary>
    public string? ItemType { get; init; }

    /// <summary>
    /// If true, only return items that are completely out of stock
    /// </summary>
    public bool? OutOfStockOnly { get; init; }

    /// <summary>
    /// If true, only return items that are critical (very low stock)
    /// </summary>
    public bool? CriticalOnly { get; init; }

    /// <summary>
    /// Page number (1-based)
    /// </summary>
    public int? Page { get; init; }

    /// <summary>
    /// Number of items per page
    /// </summary>
    public int? PageSize { get; init; }

    /// <summary>
    /// Field to sort by (itemName, currentStock, reorderLevel, urgency)
    /// </summary>
    public string? SortBy { get; init; }

    /// <summary>
    /// Sort order (asc, desc)
    /// </summary>
    public string? SortOrder { get; init; } = "asc";
}
