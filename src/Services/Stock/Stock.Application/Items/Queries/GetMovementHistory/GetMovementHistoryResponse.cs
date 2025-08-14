using TossErp.Stock.Application.DTOs;

namespace TossErp.Stock.Application.Items.Queries.GetMovementHistory;

/// <summary>
/// Response containing paginated movement history
/// </summary>
public record GetMovementHistoryResponse
{
    /// <summary>
    /// List of movements
    /// </summary>
    public List<MovementDto> Movements { get; init; } = new();

    /// <summary>
    /// Total number of movements matching the filter
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
    public MovementHistorySummary Summary { get; init; } = new();
}

/// <summary>
/// Summary statistics for movement history
/// </summary>
public record MovementHistorySummary
{
    /// <summary>
    /// Total quantity received
    /// </summary>
    public decimal TotalReceived { get; init; }

    /// <summary>
    /// Total quantity issued
    /// </summary>
    public decimal TotalIssued { get; init; }

    /// <summary>
    /// Total quantity adjusted
    /// </summary>
    public decimal TotalAdjusted { get; init; }

    /// <summary>
    /// Total quantity transferred
    /// </summary>
    public decimal TotalTransferred { get; init; }

    /// <summary>
    /// Net movement (received - issued + adjusted + transferred)
    /// </summary>
    public decimal NetMovement { get; init; }
}
