using Toss.Domain.Entities.Businesses;
using Toss.Domain.Entities.Orders;
using Toss.Domain.Entities.Vendors;
using Toss.Domain.Enums;

namespace Toss.Domain.Entities.Accounting;

/// <summary>
/// Represents an accounts payable ledger entry for a vendor invoice.
/// </summary>
public class VendorLedgerEntry : BaseAuditableEntity, IBusinessScopedEntity
{
    public int BusinessId { get; set; }
    public Business Business { get; set; } = null!;

    public int VendorId { get; set; }
    public Vendor Vendor { get; set; } = null!;

    public int PurchaseDocumentId { get; set; }
    public PurchaseDocument PurchaseDocument { get; set; } = null!;

    public decimal Amount { get; set; }
    public decimal PaidAmount { get; set; }

    /// <summary>
    /// Outstanding balance (Amount - PaidAmount)
    /// </summary>
    public decimal Balance { get; set; }

    public DateTimeOffset DocumentDate { get; set; }
    public DateTimeOffset? DueDate { get; set; }
    public VendorLedgerStatus Status { get; set; } = VendorLedgerStatus.Open;
    public string ReferenceNumber { get; set; } = string.Empty;
}

