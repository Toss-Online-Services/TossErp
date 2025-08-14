namespace TossErp.Stock.Application.DTOs;

/// <summary>
/// Request DTO for stock movement operations
/// </summary>
public record StockMovementRequest
{
    /// <summary>
    /// Item ID
    /// </summary>
    public Guid ItemId { get; init; }

    /// <summary>
    /// Warehouse ID
    /// </summary>
    public Guid WarehouseId { get; init; }

    /// <summary>
    /// Bin ID (optional)
    /// </summary>
    public Guid? BinId { get; init; }

    /// <summary>
    /// Quantity to move
    /// </summary>
    public decimal Quantity { get; init; }

    /// <summary>
    /// Movement type (Receive, Issue, Adjust, Transfer)
    /// </summary>
    public string MovementType { get; init; } = string.Empty;

    /// <summary>
    /// Reference number (PO, SO, etc.)
    /// </summary>
    public string? ReferenceNumber { get; init; }

    /// <summary>
    /// Movement reason/notes
    /// </summary>
    public string? Reason { get; init; }

    /// <summary>
    /// Unit cost (for receiving)
    /// </summary>
    public decimal? UnitCost { get; init; }

    /// <summary>
    /// Batch ID (for receiving)
    /// </summary>
    public Guid? BatchId { get; init; }

    /// <summary>
    /// Expiry date (for receiving)
    /// </summary>
    public DateTime? ExpiryDate { get; init; }

    /// <summary>
    /// Source warehouse ID (for transfers)
    /// </summary>
    public Guid? SourceWarehouseId { get; init; }

    /// <summary>
    /// Source bin ID (for transfers)
    /// </summary>
    public Guid? SourceBinId { get; init; }

    /// <summary>
    /// Destination warehouse ID (for transfers)
    /// </summary>
    public Guid? DestinationWarehouseId { get; init; }

    /// <summary>
    /// Destination bin ID (for transfers)
    /// </summary>
    public Guid? DestinationBinId { get; init; }
}
