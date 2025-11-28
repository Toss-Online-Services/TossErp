using Toss.Domain.Entities.Stores;
using Toss.Domain.Entities.Vendors;
using Toss.Domain.Enums;

namespace Toss.Domain.Entities.Orders;

/// <summary>
/// Unified purchase document entity for vendor invoices, bills, credit notes, and debit notes.
/// Completes the procurement-to-payment cycle: PO → Receipt → Document (Invoice/Bill) → Payment
/// </summary>
public class PurchaseDocument : BaseAuditableEntity
{
    public PurchaseDocument()
    {
        DocumentNumber = string.Empty;
        DocumentDate = DateTimeOffset.UtcNow;
        Subtotal = 0;
        TaxAmount = 0;
        TotalAmount = 0;
        IsPaid = false;
        IsApproved = false;
        IsMatchedToPO = false;
        IsMatchedToReceipt = false;
        Lines = new List<PurchaseDocumentLine>();
    }

    /// <summary>
    /// Document number (e.g., INV-2025-001, BILL-2025-001, CRN-2025-001)
    /// </summary>
    public string DocumentNumber { get; set; }

    /// <summary>
    /// Type of purchase document
    /// </summary>
    public PurchaseDocumentType DocumentType { get; set; }

    /// <summary>
    /// Related purchase order for three-way matching
    /// </summary>
    public int PurchaseOrderId { get; set; }
    public PurchaseOrder PurchaseOrder { get; set; } = null!;

    /// <summary>
    /// Vendor who issued this document
    /// </summary>
    public int VendorId { get; set; }
    public Vendor Vendor { get; set; } = null!;

    /// <summary>
    /// Shop/Store receiving the goods (optional, can be derived from PO)
    /// </summary>
    public int? ShopId { get; set; }
    public Store? Shop { get; set; }

    /// <summary>
    /// Document issue date (from vendor)
    /// </summary>
    public DateTimeOffset DocumentDate { get; set; }

    /// <summary>
    /// Payment due date (for invoices/bills)
    /// </summary>
    public DateTimeOffset? DueDate { get; set; }

    /// <summary>
    /// Date payment was made
    /// </summary>
    public DateTimeOffset? PaidDate { get; set; }

    // Amounts
    public decimal Subtotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// Whether this document has been paid
    /// </summary>
    public bool IsPaid { get; set; }

    /// <summary>
    /// Whether this document has been approved for payment
    /// </summary>
    public bool IsApproved { get; set; }

    /// <summary>
    /// Who approved the document
    /// </summary>
    public string? ApprovedBy { get; set; }

    /// <summary>
    /// When the document was approved
    /// </summary>
    public DateTimeOffset? ApprovedDate { get; set; }

    // Three-way matching status
    /// <summary>
    /// Whether this document has been matched to the purchase order
    /// </summary>
    public bool IsMatchedToPO { get; set; }

    /// <summary>
    /// Whether this document has been matched to the purchase receipt
    /// </summary>
    public bool IsMatchedToReceipt { get; set; }

    /// <summary>
    /// Additional notes or comments
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Payment reference (transaction ID, check number, etc.)
    /// </summary>
    public string? PaymentReference { get; set; }

    /// <summary>
    /// Detailed invoice lines
    /// </summary>
    public ICollection<PurchaseDocumentLine> Lines { get; set; }
}
