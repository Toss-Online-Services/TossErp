using Toss.Domain.Entities.Catalog;

namespace Toss.Domain.Entities.Orders;

/// <summary>
/// Represents a detailed line on a supplier invoice or purchase document.
/// </summary>
public class PurchaseDocumentLine : BaseAuditableEntity
{
    public int PurchaseDocumentId { get; set; }
    public PurchaseDocument PurchaseDocument { get; set; } = null!;

    public int PurchaseOrderItemId { get; set; }
    public PurchaseOrderItem PurchaseOrderItem { get; set; } = null!;

    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public string Description { get; set; } = string.Empty;

    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TaxRate { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal LineTotal { get; set; }
}

