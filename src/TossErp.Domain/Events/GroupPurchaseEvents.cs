using TossErp.Domain.SeedWork;
using TossErp.Domain.AggregatesModel.GroupPurchaseAggregate;

namespace TossErp.Domain.Events;

public class GroupPurchaseCreatedDomainEvent : DomainEvent
{
    public GroupPurchase GroupPurchase { get; }

    public GroupPurchaseCreatedDomainEvent(GroupPurchase groupPurchase)
    {
        GroupPurchase = groupPurchase;
    }
}

public class GroupPurchaseActivatedDomainEvent : DomainEvent
{
    public GroupPurchase GroupPurchase { get; }

    public GroupPurchaseActivatedDomainEvent(GroupPurchase groupPurchase)
    {
        GroupPurchase = groupPurchase;
    }
}

public class GroupPurchaseCompletedDomainEvent : DomainEvent
{
    public GroupPurchase GroupPurchase { get; }

    public GroupPurchaseCompletedDomainEvent(GroupPurchase groupPurchase)
    {
        GroupPurchase = groupPurchase;
    }
}

public class GroupPurchaseCancelledDomainEvent : DomainEvent
{
    public GroupPurchase GroupPurchase { get; }
    public string Reason { get; }

    public GroupPurchaseCancelledDomainEvent(GroupPurchase groupPurchase, string reason)
    {
        GroupPurchase = groupPurchase;
        Reason = reason;
    }
}

public class GroupPurchaseMemberAddedDomainEvent : DomainEvent
{
    public GroupPurchase GroupPurchase { get; }
    public GroupPurchaseMember Member { get; }

    public GroupPurchaseMemberAddedDomainEvent(GroupPurchase groupPurchase, GroupPurchaseMember member)
    {
        GroupPurchase = groupPurchase;
        Member = member;
    }
}

public class GroupPurchaseMemberRemovedDomainEvent : DomainEvent
{
    public GroupPurchase GroupPurchase { get; }
    public GroupPurchaseMember Member { get; }

    public GroupPurchaseMemberRemovedDomainEvent(GroupPurchase groupPurchase, GroupPurchaseMember member)
    {
        GroupPurchase = groupPurchase;
        Member = member;
    }
} 
