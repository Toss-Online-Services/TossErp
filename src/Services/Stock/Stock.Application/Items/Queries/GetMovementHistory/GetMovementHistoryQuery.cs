using MediatR;
using TossErp.Stock.Application.Common.DTOs;

namespace TossErp.Stock.Application.Items.Queries.GetMovementHistory;

/// <summary>
/// Query for retrieving stock movement history with filtering options
/// </summary>
public record GetMovementHistoryQuery : IRequest<GetMovementHistoryResponse>
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
    /// Filter by movement type (Receive, Issue, Adjust, Transfer)
    /// </summary>
    public string? MovementType { get; init; }

    /// <summary>
    /// Filter by batch ID
    /// </summary>
    public Guid? BatchId { get; init; }

    /// <summary>
    /// Filter by reference number
    /// </summary>
    public string? ReferenceNumber { get; init; }

    /// <summary>
    /// Filter by date range - start date
    /// </summary>
    public DateTime? FromDate { get; init; }

    /// <summary>
    /// Filter by date range - end date
    /// </summary>
    public DateTime? ToDate { get; init; }

    /// <summary>
    /// Page number (1-based)
    /// </summary>
    public int? Page { get; init; }

    /// <summary>
    /// Number of items per page
    /// </summary>
    public int? PageSize { get; init; }

    /// <summary>
    /// Field to sort by (date, itemName, warehouseName, quantity, movementType)
    /// </summary>
    public string? SortBy { get; init; }

    /// <summary>
    /// Sort order (asc, desc)
    /// </summary>
    public string? SortOrder { get; init; } = "desc";
}
