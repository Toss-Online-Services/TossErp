namespace Toss.Domain.Events;

public class SaleCompletedEvent : BaseEvent
{
    public SaleCompletedEvent(Sale sale)
    {
        Sale = sale;
    }

    public Sale Sale { get; }
}

