using Toss.Domain.Entities.CRM;
using Toss.Domain.Entities.Stores;
using Toss.Domain.Enums;

namespace Toss.Domain.Entities.Sales;

public class SalesDocument : BaseAuditableEntity
{
    public string DocumentNumber { get; set; } = string.Empty;

    public SalesDocumentType DocumentType { get; set; }

    public int SaleId { get; set; }
    public Sale Sale { get; set; } = null!;

    // Optional for Receipts; required for Invoices
    public int? CustomerId { get; set; }
    public Customer? Customer { get; set; }

    // Redundant but useful for filtering; kept nullable and derivable from Sale
    public int? ShopId { get; set; }
    public Store? Shop { get; set; }

    public DateTimeOffset DocumentDate { get; set; }
    public DateTimeOffset? DueDate { get; set; }
    public DateTimeOffset? PaidDate { get; set; }

    public decimal Subtotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal TotalAmount { get; set; }

    public bool IsPaid { get; set; }

    public string? Notes { get; set; }
}
