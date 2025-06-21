using TossErp.Domain.SeedWork;
using TossErp.Domain.AggregatesModel.SaleAggregate;

namespace TossErp.Domain.Events;

public class SaleCreatedDomainEvent : DomainEvent
{
    public Sale Sale { get; }

    public SaleCreatedDomainEvent(Sale sale)
    {
        Sale = sale;
    }
}

public class SaleCompletedDomainEvent : DomainEvent
{
    public Sale Sale { get; }

    public SaleCompletedDomainEvent(Sale sale)
    {
        Sale = sale;
    }
}

public class SaleCancelledDomainEvent : DomainEvent
{
    public Sale Sale { get; }

    public SaleCancelledDomainEvent(Sale sale)
    {
        Sale = sale;
    }
} 
