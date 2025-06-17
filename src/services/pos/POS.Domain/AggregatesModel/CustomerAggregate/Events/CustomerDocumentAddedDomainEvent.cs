using POS.Domain.Common.Events;

namespace POS.Domain.AggregatesModel.CustomerAggregate.Events;

public class CustomerDocumentAddedDomainEvent : IDomainEvent
{
    public Guid CustomerId { get; }
    public Guid DocumentId { get; }
    public string DocumentType { get; }
    public string DocumentNumber { get; }
    public string FileName { get; }
    public string FileType { get; }
    public string AddedBy { get; }
    public DateTime AddedAt { get; }

    public CustomerDocumentAddedDomainEvent(
        Guid customerId,
        Guid documentId,
        string documentType,
        string documentNumber,
        string fileName,
        string fileType,
        string addedBy,
        DateTime addedAt)
    {
        CustomerId = customerId;
        DocumentId = documentId;
        DocumentType = documentType;
        DocumentNumber = documentNumber;
        FileName = fileName;
        FileType = fileType;
        AddedBy = addedBy;
        AddedAt = addedAt;
    }
} 
