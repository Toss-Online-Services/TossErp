using POS.Domain.Common.Events;

namespace POS.Domain.AggregatesModel.CustomerAggregate.Events;

public class CustomerNoteAddedDomainEvent : IDomainEvent
{
    public Guid CustomerId { get; }
    public Guid NoteId { get; }
    public string Title { get; }
    public string Content { get; }
    public string NoteType { get; }
    public string AddedBy { get; }
    public DateTime AddedAt { get; }

    public CustomerNoteAddedDomainEvent(
        Guid customerId,
        Guid noteId,
        string title,
        string content,
        string noteType,
        string addedBy,
        DateTime addedAt)
    {
        CustomerId = customerId;
        NoteId = noteId;
        Title = title;
        Content = content;
        NoteType = noteType;
        AddedBy = addedBy;
        AddedAt = addedAt;
    }
} 
