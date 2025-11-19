namespace Toss.Domain.Events;

public class StockLowEvent : BaseEvent
{
    public StockLowEvent(StockAlert alert)
    {
        Alert = alert;
    }

    public StockAlert Alert { get; }
}

