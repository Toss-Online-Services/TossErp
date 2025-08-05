using MediatR;
using TossErp.Stock.Application.Common.DTOs;

namespace TossErp.Stock.Application.Items.Queries.GetItems;

/// <summary>
/// Query for retrieving items with filtering and pagination
/// </summary>
public record GetItemsQuery : IRequest<GetItemsResponse>
{
    /// <summary>
    /// Search term to filter items by name, code, description, or brand
    /// </summary>
    public string? SearchTerm { get; init; }

    /// <summary>
    /// Filter by item group
    /// </summary>
    public string? ItemGroup { get; init; }

    /// <summary>
    /// Filter by item type (Stock, Service, etc.)
    /// </summary>
    public string? ItemType { get; init; }

    /// <summary>
    /// Filter by disabled status
    /// </summary>
    public bool? IsDisabled { get; init; }

    /// <summary>
    /// Filter by stock item status
    /// </summary>
    public bool? IsStockItem { get; init; }

    /// <summary>
    /// If true, only return items with stock below reorder level
    /// </summary>
    public bool? LowStockOnly { get; init; }

    /// <summary>
    /// If true, only return items that are out of stock
    /// </summary>
    public bool? OutOfStockOnly { get; init; }

    /// <summary>
    /// If true, only return items that are stock items (exclude services)
    /// </summary>
    public bool? StockItemsOnly { get; init; }

    /// <summary>
    /// Page number (1-based)
    /// </summary>
    public int? Page { get; init; }

    /// <summary>
    /// Number of items per page
    /// </summary>
    public int? PageSize { get; init; }

    /// <summary>
    /// Field to sort by (name, code, group, created)
    /// </summary>
    public string? SortBy { get; init; }

    /// <summary>
    /// Sort order (asc, desc)
    /// </summary>
    public string? SortOrder { get; init; } = "asc";
} 
