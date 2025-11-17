namespace Toss.Domain.Events;

public class DeliveryCompletedEvent : BaseEvent
{
    public DeliveryCompletedEvent(SharedDeliveryRun deliveryRun)
    {
        DeliveryRun = deliveryRun;
    }

    public SharedDeliveryRun DeliveryRun { get; }
}

