using Toss.Domain.Entities.Stores;
using Toss.Domain.Entities.Vendors;
using Toss.Domain.Enums;

namespace Toss.Domain.Entities.Orders;

/// <summary>
/// Represents a purchase request (PR) that can be converted to a purchase order (PO)
/// </summary>
public class PurchaseRequest : BaseAuditableEntity
{
    public PurchaseRequest()
    {
        PRNumber = string.Empty;
        Status = PurchaseRequestStatus.Draft;
        Items = new List<PurchaseRequestLine>();
    }

    /// <summary>
    /// Gets or sets the purchase request number (unique per business)
    /// </summary>
    public string PRNumber { get; set; }

    /// <summary>
    /// Gets or sets the shop/store ID that needs the items
    /// </summary>
    public int ShopId { get; set; }
    public Store Shop { get; set; } = null!;

    /// <summary>
    /// Gets or sets the vendor/supplier ID
    /// </summary>
    public int VendorId { get; set; }
    public Vendor Vendor { get; set; } = null!;

    /// <summary>
    /// Gets or sets the user ID who requested this purchase
    /// </summary>
    public string RequestedByUserId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the date by which items are required
    /// </summary>
    public DateTime? RequiredByDate { get; set; }

    /// <summary>
    /// Gets or sets the purchase request status
    /// </summary>
    public PurchaseRequestStatus Status { get; set; }

    /// <summary>
    /// Gets or sets the purchase order ID if this PR has been converted to a PO
    /// </summary>
    public int? PurchaseOrderId { get; set; }
    public PurchaseOrder? PurchaseOrder { get; set; }

    /// <summary>
    /// Gets or sets optional notes or comments
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Gets or sets the purchase request line items
    /// </summary>
    public ICollection<PurchaseRequestLine> Items { get; set; }
}

