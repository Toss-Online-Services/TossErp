namespace Toss.Domain.Events;

public class PoolConfirmedEvent : BaseEvent
{
    public PoolConfirmedEvent(GroupBuyPool pool)
    {
        Pool = pool;
    }

    public GroupBuyPool Pool { get; }
}

