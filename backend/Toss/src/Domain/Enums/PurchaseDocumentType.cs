namespace Toss.Domain.Enums;

/// <summary>
/// Types of purchase documents in the procurement-to-payment cycle
/// </summary>
public enum PurchaseDocumentType
{
    /// <summary>
    /// Vendor invoice - formal bill for goods/services delivered
    /// </summary>
    VendorInvoice = 1,
    
    /// <summary>
    /// Vendor bill - immediate payment request (same as invoice but more casual)
    /// </summary>
    VendorBill = 2,
    
    /// <summary>
    /// Credit note - vendor owes you money (returns, refunds, overpayment)
    /// </summary>
    CreditNote = 3,
    
    /// <summary>
    /// Debit note - you charge the vendor (damage claims, shipping charges)
    /// </summary>
    DebitNote = 4
}
