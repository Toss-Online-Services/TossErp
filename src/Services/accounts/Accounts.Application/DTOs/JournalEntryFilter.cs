using TossErp.Accounts.Domain.Enums;

namespace TossErp.Accounts.Application.DTOs;

/// <summary>
/// Filter criteria for journal entry queries
/// </summary>
public class JournalEntryFilter
{
    public DateOnly? EntryDateFrom { get; set; }
    public DateOnly? EntryDateTo { get; set; }
    public JournalEntryStatus? Status { get; set; }
    public string? Reference { get; set; }
    public string? Description { get; set; }
    public TransactionType? TransactionType { get; set; }
    public string? JournalNumber { get; set; }
    public decimal? MinAmount { get; set; }
    public decimal? MaxAmount { get; set; }
    public bool? IsPosted { get; set; }
    public bool? IsReversed { get; set; }
    public string? CreatedBy { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 50;
    public string SortBy { get; set; } = "EntryDate";
    public bool SortDescending { get; set; } = true;
}
