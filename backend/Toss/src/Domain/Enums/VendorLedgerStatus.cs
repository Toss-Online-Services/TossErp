namespace Toss.Domain.Enums;

/// <summary>
/// Status for vendor ledger (accounts payable) entries.
/// </summary>
public enum VendorLedgerStatus
{
    Open = 0,
    PartiallyPaid = 1,
    Settled = 2,
    Overdue = 3
}

