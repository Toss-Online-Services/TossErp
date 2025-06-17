using POS.Domain.Common.Events;

namespace POS.Domain.AggregatesModel.CustomerAggregate.Events;

public class CustomerDocumentRemovedDomainEvent : IDomainEvent
{
    public Guid CustomerId { get; }
    public Guid DocumentId { get; }
    public string DocumentType { get; }
    public string DocumentNumber { get; }
    public string RemovedBy { get; }
    public DateTime RemovedAt { get; }

    public CustomerDocumentRemovedDomainEvent(
        Guid customerId,
        Guid documentId,
        string documentType,
        string documentNumber,
        string removedBy,
        DateTime removedAt)
    {
        CustomerId = customerId;
        DocumentId = documentId;
        DocumentType = documentType;
        DocumentNumber = documentNumber;
        RemovedBy = removedBy;
        RemovedAt = removedAt;
    }
} 
