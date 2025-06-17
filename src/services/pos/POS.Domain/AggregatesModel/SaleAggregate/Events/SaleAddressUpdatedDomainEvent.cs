using POS.Domain.Common.Events;
using POS.Domain.Common.ValueObjects;

namespace POS.Domain.AggregatesModel.SaleAggregate.Events;

public class SaleAddressUpdatedDomainEvent : IDomainEvent
{
    public Guid SaleId { get; }
    public Address? Address { get; }
    public DateTime UpdatedAt { get; }

    public SaleAddressUpdatedDomainEvent(Guid saleId, Address? address, DateTime updatedAt)
    {
        SaleId = saleId;
        Address = address;
        UpdatedAt = updatedAt;
    }
} 
