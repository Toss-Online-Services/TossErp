#nullable enable

#nullable enable
using POS.Domain.SeedWork;

namespace POS.Domain.AggregatesModel.SaleAggregate.Events;

public class SaleCompletedDomainEvent : DomainEvent
{
    public Sale Sale { get; }

    public SaleCompletedDomainEvent(Sale sale)
    {
        Sale = sale;
    }
} 
