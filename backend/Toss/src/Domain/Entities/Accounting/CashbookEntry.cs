using Toss.Domain.Common;
using Toss.Domain.Entities.Businesses;
using Toss.Domain.Entities.Payments;
using Toss.Domain.Enums;

namespace Toss.Domain.Entities.Accounting;

/// <summary>
/// Represents a cashbook entry recording money in or out of an account
/// </summary>
public class CashbookEntry : BaseAuditableEntity, IBusinessScopedEntity
{
    public CashbookEntry()
    {
        Reference = string.Empty;
        EntryDate = DateTimeOffset.UtcNow;
    }

    /// <summary>
    /// Gets or sets the business/tenant identifier
    /// </summary>
    public int BusinessId { get; set; }
    public Business Business { get; set; } = null!;

    /// <summary>
    /// Gets or sets the account ID this entry belongs to
    /// </summary>
    public int AccountId { get; set; }
    public Account Account { get; set; } = null!;

    /// <summary>
    /// Gets or sets the entry amount (positive for money in, negative for money out)
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// Gets or sets the entry date
    /// </summary>
    public DateTimeOffset EntryDate { get; set; }

    /// <summary>
    /// Gets or sets the entry type
    /// </summary>
    public CashbookEntryType Type { get; set; }

    /// <summary>
    /// Gets or sets an optional reference number or identifier
    /// </summary>
    public string? Reference { get; set; }

    /// <summary>
    /// Gets or sets optional notes
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Gets or sets the counterparty account ID for transfers (when Type = Transfer)
    /// </summary>
    public int? CounterpartyAccountId { get; set; }
    public Account? CounterpartyAccount { get; set; }

    /// <summary>
    /// Gets or sets the source type (e.g., "Sale", "PurchaseOrder", "Expense")
    /// </summary>
    public string? SourceType { get; set; }

    /// <summary>
    /// Gets or sets the source ID (e.g., SaleId, PurchaseOrderId)
    /// </summary>
    public int? SourceId { get; set; }

    /// <summary>
    /// Gets or sets the optional payment ID if this entry is linked to a Payment
    /// </summary>
    public int? PaymentId { get; set; }
    public Payment? Payment { get; set; }
}

