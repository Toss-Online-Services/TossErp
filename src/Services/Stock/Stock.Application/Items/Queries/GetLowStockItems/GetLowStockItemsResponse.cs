using TossErp.Stock.Application.Common.DTOs;

namespace TossErp.Stock.Application.Items.Queries.GetLowStockItems;

/// <summary>
/// Response containing low stock items
/// </summary>
public record GetLowStockItemsResponse
{
    /// <summary>
    /// List of low stock items
    /// </summary>
    public List<LowStockItemDto> Items { get; init; } = new();

    /// <summary>
    /// Total number of low stock items
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
    public LowStockSummary Summary { get; init; } = new();
}

/// <summary>
/// DTO for low stock items with urgency information
/// </summary>
public record LowStockItemDto
{
    /// <summary>
    /// Item ID
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Item name
    /// </summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// Item code/SKU
    /// </summary>
    public string Code { get; init; } = string.Empty;

    /// <summary>
    /// Current stock level
    /// </summary>
    public decimal CurrentStock { get; init; }

    /// <summary>
    /// Reorder level
    /// </summary>
    public decimal ReorderLevel { get; init; }

    /// <summary>
    /// Maximum stock level
    /// </summary>
    public decimal MaxStock { get; init; }

    /// <summary>
    /// Stock deficit (reorder level - current stock)
    /// </summary>
    public decimal StockDeficit { get; init; }

    /// <summary>
    /// Urgency level (Critical, High, Medium, Low)
    /// </summary>
    public string UrgencyLevel { get; init; } = string.Empty;

    /// <summary>
    /// Warehouse name
    /// </summary>
    public string WarehouseName { get; init; } = string.Empty;

    /// <summary>
    /// Last movement date
    /// </summary>
    public DateTime? LastMovementDate { get; init; }
}

/// <summary>
/// Summary statistics for low stock items
/// </summary>
public record LowStockSummary
{
    /// <summary>
    /// Total number of critical items
    /// </summary>
    public int CriticalCount { get; init; }

    /// <summary>
    /// Total number of high urgency items
    /// </summary>
    public int HighCount { get; init; }

    /// <summary>
    /// Total number of medium urgency items
    /// </summary>
    public int MediumCount { get; init; }

    /// <summary>
    /// Total number of low urgency items
    /// </summary>
    public int LowCount { get; init; }

    /// <summary>
    /// Total number of out of stock items
    /// </summary>
    public int OutOfStockCount { get; init; }
}
