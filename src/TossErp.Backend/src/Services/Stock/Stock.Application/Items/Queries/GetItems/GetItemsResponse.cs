using TossErp.Stock.Application.Common.DTOs;

namespace TossErp.Stock.Application.Items.Queries.GetItems;

/// <summary>
/// Response containing paginated items
/// </summary>
public record GetItemsResponse
{
    /// <summary>
    /// List of items
    /// </summary>
    public List<ItemDto> Items { get; init; } = new();

    /// <summary>
    /// Total number of items matching the filter
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
} 
