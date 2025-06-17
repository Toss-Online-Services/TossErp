using POS.Domain.Common.Events;

namespace POS.Domain.AggregatesModel.SaleAggregate.Events;

public class SaleNotesUpdatedDomainEvent : IDomainEvent
{
    public Guid SaleId { get; }
    public string? Notes { get; }
    public DateTime UpdatedAt { get; }

    public SaleNotesUpdatedDomainEvent(Guid saleId, string? notes, DateTime updatedAt)
    {
        SaleId = saleId;
        Notes = notes;
        UpdatedAt = updatedAt;
    }
} 
