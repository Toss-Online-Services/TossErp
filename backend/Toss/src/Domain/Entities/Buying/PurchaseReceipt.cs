namespace Toss.Domain.Entities.Buying;

public class PurchaseReceipt : BaseAuditableEntity
{
    public string ReceiptNumber { get; set; } = string.Empty;
    
    public int PurchaseOrderId { get; set; }
    public PurchaseOrder PurchaseOrder { get; set; } = null!;
    
    public DateTimeOffset ReceivedDate { get; set; }
    public string ReceivedBy { get; set; } = string.Empty;
    
    public bool IsPartialReceipt { get; set; }
    public string? Notes { get; set; }
    
    // Quality check
    public bool QualityCheckPassed { get; set; } = true;
    public string? QualityNotes { get; set; }
}

