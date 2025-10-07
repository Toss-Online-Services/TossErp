using TossErp.Domain.Common;

namespace TossErp.Domain.Entities.Finance;

public enum JournalEntryStatus
{
    Draft,
    Posted,
    Reversed
}

public class JournalEntry : BaseEntity
{
    public string EntryNumber { get; set; } = string.Empty;
    public DateTime EntryDate { get; set; } = DateTime.UtcNow;
    public JournalEntryStatus Status { get; set; } = JournalEntryStatus.Draft;
    
    public string? ReferenceType { get; set; } // Sale, Purchase, Payment, etc.
    public int? ReferenceId { get; set; }
    public string? ReferenceNumber { get; set; }
    
    public string? Description { get; set; }
    public string? Notes { get; set; }
    
    public int? PostedById { get; set; }
    public string? PostedByName { get; set; }
    public DateTime? PostedDate { get; set; }
    
    // Navigation properties
    public virtual ICollection<JournalEntryLine> Lines { get; set; } = new List<JournalEntryLine>();
    
    // Business logic
    public void Post(string postedBy)
    {
        if (Status != JournalEntryStatus.Draft)
            throw new InvalidOperationException("Only draft entries can be posted");
        
        if (!IsBalanced())
            throw new InvalidOperationException("Journal entry must be balanced (debits = credits)");
        
        Status = JournalEntryStatus.Posted;
        PostedById = null; // Would be actual user ID
        PostedByName = postedBy;
        PostedDate = DateTime.UtcNow;
        
        AddDomainEvent(new JournalEntryPostedEvent(Id, EntryNumber));
    }
    
    public bool IsBalanced()
    {
        var totalDebit = Lines.Sum(l => l.DebitAmount);
        var totalCredit = Lines.Sum(l => l.CreditAmount);
        return Math.Abs(totalDebit - totalCredit) < 0.01m; // Allow for minor rounding differences
    }
    
    public void Reverse(string reason)
    {
        if (Status != JournalEntryStatus.Posted)
            throw new InvalidOperationException("Only posted entries can be reversed");
        
        Status = JournalEntryStatus.Reversed;
        Notes = $"{Notes}\nReversed: {reason}";
        
        AddDomainEvent(new JournalEntryReversedEvent(Id, EntryNumber, reason));
    }
}

public class JournalEntryLine : BaseEntity
{
    public int JournalEntryId { get; set; }
    public virtual JournalEntry JournalEntry { get; set; } = null!;
    
    public int AccountId { get; set; }
    public virtual Account Account { get; set; } = null!;
    
    public decimal DebitAmount { get; set; }
    public decimal CreditAmount { get; set; }
    public string? Description { get; set; }
    
    public int LineNumber { get; set; } // Order within the journal entry
}

// Domain Events
public class JournalEntryPostedEvent : DomainEvent
{
    public int JournalEntryId { get; }
    public string EntryNumber { get; }
    
    public JournalEntryPostedEvent(int journalEntryId, string entryNumber)
    {
        JournalEntryId = journalEntryId;
        EntryNumber = entryNumber;
    }
}

public class JournalEntryReversedEvent : DomainEvent
{
    public int JournalEntryId { get; }
    public string EntryNumber { get; }
    public string Reason { get; }
    
    public JournalEntryReversedEvent(int journalEntryId, string entryNumber, string reason)
    {
        JournalEntryId = journalEntryId;
        EntryNumber = entryNumber;
        Reason = reason;
    }
}

