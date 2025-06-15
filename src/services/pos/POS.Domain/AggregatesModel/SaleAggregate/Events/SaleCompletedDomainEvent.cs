#nullable enable
using TossErp.POS.Domain.SeedWork;

namespace TossErp.POS.Domain.AggregatesModel.SaleAggregate.Events;

public class SaleCompletedDomainEvent : DomainEvent
{
    public Sale Sale { get; }

    public SaleCompletedDomainEvent(Sale sale)
    {
        Sale = sale;
    }
} 
