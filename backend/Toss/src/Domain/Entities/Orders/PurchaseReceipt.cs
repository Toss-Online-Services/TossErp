namespace Toss.Domain.Entities.Orders;

/// <summary>
/// Represents a receipt for a purchase order
/// </summary>
public class PurchaseReceipt : BaseAuditableEntity
{
    public PurchaseReceipt()
    {
        ReceiptNumber = string.Empty;
        ReceivedDate = DateTimeOffset.UtcNow;
        ReceivedBy = string.Empty;
        IsPartialReceipt = false;
        QualityCheckPassed = true;
    }

    /// <summary>
    /// Gets or sets the receipt number
    /// </summary>
    public string ReceiptNumber { get; set; }

    /// <summary>
    /// Gets or sets the purchase order ID
    /// </summary>
    public int PurchaseOrderId { get; set; }
    public PurchaseOrder PurchaseOrder { get; set; } = null!;

    /// <summary>
    /// Gets or sets the date received
    /// </summary>
    public DateTimeOffset ReceivedDate { get; set; }

    /// <summary>
    /// Gets or sets who received the order
    /// </summary>
    public string ReceivedBy { get; set; }

    /// <summary>
    /// Gets or sets whether this is a partial receipt
    /// </summary>
    public bool IsPartialReceipt { get; set; }

    /// <summary>
    /// Gets or sets optional notes
    /// </summary>
    public string? Notes { get; set; }

    // Quality check
    /// <summary>
    /// Gets or sets whether quality check passed
    /// </summary>
    public bool QualityCheckPassed { get; set; }

    /// <summary>
    /// Gets or sets quality check notes
    /// </summary>
    public string? QualityNotes { get; set; }
}

