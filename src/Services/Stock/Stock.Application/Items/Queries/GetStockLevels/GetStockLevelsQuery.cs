using MediatR;
using TossErp.Stock.Application.Common.DTOs;

namespace TossErp.Stock.Application.Items.Queries.GetStockLevels;

/// <summary>
/// Query for retrieving stock levels with filtering options
/// </summary>
public record GetStockLevelsQuery : IRequest<GetStockLevelsResponse>
{
    /// <summary>
    /// Filter by item ID
    /// </summary>
    public Guid? ItemId { get; init; }

    /// <summary>
    /// Filter by warehouse ID
    /// </summary>
    public Guid? WarehouseId { get; init; }

    /// <summary>
    /// Filter by bin ID
    /// </summary>
    public Guid? BinId { get; init; }

    /// <summary>
    /// If true, only return items with stock below reorder level
    /// </summary>
    public bool? LowStockOnly { get; init; }

    /// <summary>
    /// If true, only return items that are out of stock
    /// </summary>
    public bool? OutOfStockOnly { get; init; }

    /// <summary>
    /// If true, only return items with stock above zero
    /// </summary>
    public bool? InStockOnly { get; init; }

    /// <summary>
    /// Page number (1-based)
    /// </summary>
    public int? Page { get; init; }

    /// <summary>
    /// Number of items per page
    /// </summary>
    public int? PageSize { get; init; }

    /// <summary>
    /// Field to sort by (itemName, warehouseName, quantity, reorderLevel)
    /// </summary>
    public string? SortBy { get; init; }

    /// <summary>
    /// Sort order (asc, desc)
    /// </summary>
    public string? SortOrder { get; init; } = "asc";
}
