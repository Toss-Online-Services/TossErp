using TossErp.Stock.Application.DTOs;

namespace TossErp.Stock.Application.Items.Queries.GetStockLevels;

/// <summary>
/// Response containing paginated stock levels
/// </summary>
public record GetStockLevelsResponse
{
    /// <summary>
    /// List of stock levels
    /// </summary>
    public List<StockLevelDto> StockLevels { get; init; } = new();

    /// <summary>
    /// Total number of stock levels matching the filter
    /// </summary>
    public int TotalCount { get; init; }

    /// <summary>
    /// Current page number
    /// </summary>
    public int Page { get; init; }

    /// <summary>
    /// Number of items per page
    /// </summary>
    public int PageSize { get; init; }

    /// <summary>
    /// Total number of pages
    /// </summary>
    public int TotalPages { get; init; }

    /// <summary>
    /// Summary statistics
    /// </summary>
    public StockLevelSummary Summary { get; init; } = new();
}

/// <summary>
/// Summary statistics for stock levels
/// </summary>
public record StockLevelSummary
{
    /// <summary>
    /// Total number of items in stock
    /// </summary>
    public int TotalItemsInStock { get; init; }

    /// <summary>
    /// Total number of items out of stock
    /// </summary>
    public int TotalItemsOutOfStock { get; init; }

    /// <summary>
    /// Total number of items below reorder level
    /// </summary>
    public int TotalItemsLowStock { get; init; }

    /// <summary>
    /// Total value of stock on hand
    /// </summary>
    public decimal TotalStockValue { get; init; }
}
