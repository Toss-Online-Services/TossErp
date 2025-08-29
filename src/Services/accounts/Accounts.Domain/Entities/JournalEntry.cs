using TossErp.Accounts.Domain.Enums;
using TossErp.Shared.SeedWork;
using TossErp.Accounts.Domain.ValueObjects;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TossErp.Accounts.Domain.Entities;

/// <summary>
/// Journal Entry entity for double-entry bookkeeping in TOSS ERP
/// Designed for South African township SMME context
/// </summary>
[Table("JournalEntries")]
public class JournalEntry : AggregateRoot
{
    public override Guid Id { get; protected set; }
    public override DateTime CreatedAt { get; protected set; }
    public override string CreatedBy { get; protected set; }
    [Required]
    [StringLength(50)]
    public string JournalNumber { get; private set; } = string.Empty;

    // Application layer compatibility property
    public string EntryNumber => JournalNumber;

    public DateTime EntryDate { get; private set; }

    public DateTime PostingDate { get; private set; }

    [Required]
    [StringLength(500)]
    public string Description { get; private set; } = string.Empty;

    public TransactionType TransactionType { get; private set; } = TransactionType.JournalEntry;

    public JournalEntryStatus Status { get; private set; } = JournalEntryStatus.Draft;

    [StringLength(100)]
    public string? ReferenceNumber { get; private set; }

    // Application layer compatibility property
    public string? Reference => ReferenceNumber;

    [StringLength(200)]
    public string? ExternalReference { get; private set; }

    public Money TotalDebitAmount { get; private set; } = Money.Zero(CurrencyCode.ZAR);

    public Money TotalCreditAmount { get; private set; } = Money.Zero(CurrencyCode.ZAR);

    [StringLength(3)]
    public string Currency { get; private set; } = "ZAR";

    // Source information
    [StringLength(100)]
    public string? SourceDocument { get; private set; }

    public Guid? SourceDocumentId { get; private set; }

    [StringLength(50)]
    public string? SourceDocumentType { get; private set; } // Invoice, Payment, etc.

    // Township/SMME specific fields
    [StringLength(200)]
    public string? LocationOfTransaction { get; private set; }

    [StringLength(200)]
    public string? CommunityContext { get; private set; }

    public bool IsCommunityTransaction { get; private set; } = false;

    public bool IsRecurring { get; private set; } = false;

    [StringLength(50)]
    public string? RecurringPattern { get; private set; } // Monthly, Weekly, etc.

    // Approval workflow
    public bool RequiresApproval { get; private set; } = false;

    public DateTime? SubmittedDate { get; private set; }

    [StringLength(100)]
    public string? SubmittedBy { get; private set; }

    public DateTime? ApprovedDate { get; private set; }

    [StringLength(100)]
    public string? ApprovedBy { get; private set; }

    public DateTime? PostedDate { get; private set; }

    [StringLength(100)]
    public string? PostedBy { get; private set; }

    // Reversal information
    public bool IsReversed { get; private set; } = false;

    public DateTime? ReversedDate { get; private set; }

    [StringLength(100)]
    public string? ReversedBy { get; private set; }

    public Guid? ReversalJournalId { get; private set; }

    [StringLength(500)]
    public string? ReversalReason { get; private set; }

    // Audit fields
    public DateTime ModifiedAt { get; private set; } = DateTime.UtcNow;

    [StringLength(100)]
    public string? ModifiedBy { get; private set; }

    // Navigation properties
    private readonly List<JournalEntryLine> _journalLines = new();
    public IReadOnlyList<JournalEntryLine> JournalLines => _journalLines.AsReadOnly();

    // Application layer compatibility properties
    public IReadOnlyList<JournalEntryLine> Lines => _journalLines.AsReadOnly();
    
    [StringLength(1000)]
    public string? Notes { get; private set; }

    public DateTime? PostedAt => PostedDate;

    public DateTime LastModified => ModifiedAt;
    public string? LastModifiedBy => ModifiedBy;

    public JournalEntry? ReversalJournal { get; private set; }

    private JournalEntry() : base() { } // EF Core

    public JournalEntry(
        Guid id,
        string tenantId,
        string journalNumber,
        DateTime entryDate,
        string description,
        TransactionType transactionType,
        string createdBy,
        DateTime? postingDate = null,
        string? referenceNumber = null)
    {
        JournalNumber = journalNumber?.Trim() ?? throw new ArgumentException("Journal number cannot be empty");
        EntryDate = entryDate.Date;
        PostingDate = postingDate?.Date ?? entryDate.Date;
        Description = description?.Trim() ?? throw new ArgumentException("Description cannot be empty");
        TransactionType = transactionType;
        ModifiedBy = createdBy?.Trim() ?? throw new ArgumentException("CreatedBy cannot be empty");
        ReferenceNumber = referenceNumber?.Trim();
        ModifiedAt = DateTime.UtcNow;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = createdBy;
    }

    public static JournalEntry Create(
        string tenantId,
        string journalNumber,
        DateTime entryDate,
        string description,
        TransactionType transactionType,
        string createdBy,
        DateTime? postingDate = null,
        string? referenceNumber = null)
    {
        return new JournalEntry(Guid.NewGuid(), tenantId, journalNumber, entryDate, description,
            transactionType, createdBy, postingDate, referenceNumber);
    }

    public void AddJournalLine(JournalEntryLine journalLine)
    {
        if (journalLine == null)
            throw new ArgumentNullException(nameof(journalLine));

        if (Status != JournalEntryStatus.Draft)
            throw new InvalidOperationException("Cannot modify non-draft journal entry");

        _journalLines.Add(journalLine);
        RecalculateTotals();
    }

    public void RemoveJournalLine(Guid lineId)
    {
        if (Status != JournalEntryStatus.Draft)
            throw new InvalidOperationException("Cannot modify non-draft journal entry");

        var line = _journalLines.FirstOrDefault(x => x.Id == lineId);
        if (line != null)
        {
            _journalLines.Remove(line);
            RecalculateTotals();
        }
    }

    public void UpdateJournalLine(Guid lineId, SignedMoney amount, string? description = null)
    {
        if (Status != JournalEntryStatus.Draft)
            throw new InvalidOperationException("Cannot modify non-draft journal entry");

        var line = _journalLines.FirstOrDefault(x => x.Id == lineId);
        if (line != null)
        {
            // Update amount through constructor since there's no UpdateAmount method
            var updatedLine = new JournalEntryLine(
                line.Id,
                line.JournalEntryId,
                line.AccountId,
                line.AccountName,
                amount,
                description ?? line.Description,
                line.CostCenterId,
                line.Reference);

            _journalLines.Remove(line);
            _journalLines.Add(updatedLine);
            RecalculateTotals();
        }
    }

    public void Submit(string submittedBy)
    {
        if (Status != JournalEntryStatus.Draft)
            throw new InvalidOperationException("Only draft journal entries can be submitted");

        ValidateJournalEntry();

        Status = JournalEntryStatus.Submitted;
        SubmittedDate = DateTime.UtcNow;
        SubmittedBy = submittedBy?.Trim() ?? throw new ArgumentException("SubmittedBy cannot be empty");
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = submittedBy;
    }

    public void Approve(string approvedBy)
    {
        if (Status != JournalEntryStatus.Submitted)
            throw new InvalidOperationException("Only submitted journal entries can be approved");

        ValidateJournalEntry();

        ApprovedDate = DateTime.UtcNow;
        ApprovedBy = approvedBy?.Trim() ?? throw new ArgumentException("ApprovedBy cannot be empty");
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = approvedBy;

        // Auto-post if no further approval required
        if (!RequiresApproval)
        {
            Post(approvedBy);
        }
    }

    public void Post(string postedBy)
    {
        if (Status == JournalEntryStatus.Posted)
            return;

        if (Status != JournalEntryStatus.Submitted && !ApprovedDate.HasValue)
            throw new InvalidOperationException("Journal entry must be submitted and approved before posting");

        ValidateJournalEntry();

        Status = JournalEntryStatus.Posted;
        PostedDate = DateTime.UtcNow;
        PostedBy = postedBy?.Trim() ?? throw new ArgumentException("PostedBy cannot be empty");
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = postedBy;
    }

    public JournalEntry Reverse(string reversalReason, string reversedBy)
    {
        if (Status != JournalEntryStatus.Posted)
            throw new InvalidOperationException("Only posted journal entries can be reversed");

        if (IsReversed)
            throw new InvalidOperationException("Journal entry is already reversed");

        // Create reversal journal entry
        var reversalJournal = new JournalEntry(
            Guid.NewGuid(),
            TenantId,
            $"REV-{JournalNumber}",
            DateTime.UtcNow.Date,
            $"Reversal of {JournalNumber}: {reversalReason}",
            TransactionType,
            reversedBy,
            DateTime.UtcNow.Date,
            ReferenceNumber)
        {
            SourceDocument = "Journal Reversal",
            SourceDocumentId = Id,
            SourceDocumentType = "JournalEntry"
        };

        // Add reversed journal lines
        foreach (var line in _journalLines)
        {
            var reversalLine = new JournalEntryLine(
                Guid.NewGuid(),
                reversalJournal.Id,
                line.AccountId,
                line.AccountName,
                line.Amount.Negate(), // Reverse the signed amount
                $"Reversal of {line.Description}",
                line.CostCenterId,
                line.Reference);

            reversalJournal.AddJournalLine(reversalLine);
        }

        // Post the reversal immediately
        reversalJournal.Status = JournalEntryStatus.Posted;
        reversalJournal.PostedDate = DateTime.UtcNow;
        reversalJournal.PostedBy = reversedBy;

        // Mark original as reversed
        IsReversed = true;
        ReversedDate = DateTime.UtcNow;
        ReversedBy = reversedBy?.Trim() ?? throw new ArgumentException("ReversedBy cannot be empty");
        ReversalJournalId = reversalJournal.Id;
        ReversalReason = reversalReason?.Trim();
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = reversedBy;

        return reversalJournal;
    }

    public void Cancel(string cancelledBy)
    {
        if (Status == JournalEntryStatus.Posted)
            throw new InvalidOperationException("Cannot cancel posted journal entry - use reversal instead");

        Status = JournalEntryStatus.Cancelled;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = cancelledBy?.Trim() ?? throw new ArgumentException("CancelledBy cannot be empty");
    }

    public void UpdateSourceDocument(string? sourceDocument, Guid? sourceDocumentId, string? sourceDocumentType, string modifiedBy)
    {
        if (Status == JournalEntryStatus.Posted)
            throw new InvalidOperationException("Cannot modify posted journal entry");

        SourceDocument = sourceDocument?.Trim();
        SourceDocumentId = sourceDocumentId;
        SourceDocumentType = sourceDocumentType?.Trim();
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy?.Trim() ?? throw new ArgumentException("ModifiedBy cannot be empty");
    }

    public void SetCommunityContext(string communityContext, string? location, string modifiedBy)
    {
        IsCommunityTransaction = true;
        CommunityContext = communityContext?.Trim();
        LocationOfTransaction = location?.Trim();
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy?.Trim() ?? throw new ArgumentException("ModifiedBy cannot be empty");
    }

    public void SetRecurringPattern(string recurringPattern, string modifiedBy)
    {
        IsRecurring = true;
        RecurringPattern = recurringPattern?.Trim();
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy?.Trim() ?? throw new ArgumentException("ModifiedBy cannot be empty");
    }

    public void UpdateDescription(string description, string modifiedBy)
    {
        if (Status == JournalEntryStatus.Posted)
            throw new InvalidOperationException("Cannot modify posted journal entry");

        Description = description?.Trim() ?? throw new ArgumentException("Description cannot be empty");
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy?.Trim() ?? throw new ArgumentException("ModifiedBy cannot be empty");
    }

    public void UpdatePostingDate(DateTime postingDate, string modifiedBy)
    {
        if (Status == JournalEntryStatus.Posted)
            throw new InvalidOperationException("Cannot modify posted journal entry");

        PostingDate = postingDate.Date;
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = modifiedBy?.Trim() ?? throw new ArgumentException("ModifiedBy cannot be empty");
    }

    private void RecalculateTotals()
    {
        TotalDebitAmount = _journalLines
            .Where(x => x.IsDebit)
            .Aggregate(Money.Zero(CurrencyCode.ZAR), (sum, line) => sum.Add(line.Amount.ToMoney()));

        TotalCreditAmount = _journalLines
            .Where(x => x.IsCredit)
            .Aggregate(Money.Zero(CurrencyCode.ZAR), (sum, line) => sum.Add(line.Amount.ToMoney()));

        // Update currency if we have journal lines
        if (_journalLines.Any())
        {
            Currency = _journalLines.First().Amount.Currency.ToString();
        }
    }

    private void ValidateJournalEntry()
    {
        if (!_journalLines.Any())
            throw new InvalidOperationException("Journal entry must have at least one journal line");

        if (_journalLines.Count < 2)
            throw new InvalidOperationException("Journal entry must have at least two journal lines for double-entry");

        if (TotalDebitAmount.Amount != TotalCreditAmount.Amount)
            throw new InvalidOperationException("Total debits must equal total credits");

        if (_journalLines.Any(x => x.Amount.Amount <= 0))
            throw new InvalidOperationException("All journal line amounts must be positive");

        // Validate all lines have the same currency
        var currencies = _journalLines.Select(x => x.Amount.Currency).Distinct().ToList();
        if (currencies.Count > 1)
            throw new InvalidOperationException("All journal lines must have the same currency");
    }

    public bool IsBalanced => TotalDebitAmount.Amount == TotalCreditAmount.Amount;

    public bool IsDraft => Status == JournalEntryStatus.Draft;

    public bool IsSubmitted => Status == JournalEntryStatus.Submitted;

    public bool IsPosted => Status == JournalEntryStatus.Posted;

    public bool IsCancelled => Status == JournalEntryStatus.Cancelled;

    public bool CanBeModified => Status == JournalEntryStatus.Draft;

    public bool CanBeSubmitted => Status == JournalEntryStatus.Draft && IsBalanced && _journalLines.Count >= 2;

    public bool CanBePosted => Status == JournalEntryStatus.Submitted && IsBalanced;

    public bool CanBeReversed => Status == JournalEntryStatus.Posted && !IsReversed;

    public int LineCount => _journalLines.Count;

    public Money NetAmount => TotalDebitAmount.Subtract(TotalCreditAmount);
}
