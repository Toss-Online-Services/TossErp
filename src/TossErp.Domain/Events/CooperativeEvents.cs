using System;
using MediatR;
using TossErp.Domain.AggregatesModel.CooperativeAggregate;

namespace TossErp.Domain.Events
{
    public class CooperativeCreatedDomainEvent : INotification
    {
        public Cooperative Cooperative { get; }

        public CooperativeCreatedDomainEvent(Cooperative cooperative)
        {
            Cooperative = cooperative;
        }
    }

    public class CooperativeUpdatedDomainEvent : INotification
    {
        public Cooperative Cooperative { get; }

        public CooperativeUpdatedDomainEvent(Cooperative cooperative)
        {
            Cooperative = cooperative;
        }
    }

    public class CooperativeRegisteredDomainEvent : INotification
    {
        public Cooperative Cooperative { get; }

        public CooperativeRegisteredDomainEvent(Cooperative cooperative)
        {
            Cooperative = cooperative;
        }
    }

    public class CooperativeActivatedDomainEvent : INotification
    {
        public Cooperative Cooperative { get; }

        public CooperativeActivatedDomainEvent(Cooperative cooperative)
        {
            Cooperative = cooperative;
        }
    }

    public class CooperativeDeactivatedDomainEvent : INotification
    {
        public Cooperative Cooperative { get; }

        public CooperativeDeactivatedDomainEvent(Cooperative cooperative)
        {
            Cooperative = cooperative;
        }
    }

    public class CooperativeMemberAddedDomainEvent : INotification
    {
        public Cooperative Cooperative { get; }
        public CooperativeMember Member { get; }

        public CooperativeMemberAddedDomainEvent(Cooperative cooperative, CooperativeMember member)
        {
            Cooperative = cooperative;
            Member = member;
        }
    }

    public class CooperativeMemberUpdatedDomainEvent : INotification
    {
        public Cooperative Cooperative { get; }
        public CooperativeMember Member { get; }

        public CooperativeMemberUpdatedDomainEvent(Cooperative cooperative, CooperativeMember member)
        {
            Cooperative = cooperative;
            Member = member;
        }
    }

    public class CooperativeMemberRemovedDomainEvent : INotification
    {
        public Cooperative Cooperative { get; }
        public Guid MemberId { get; }
        public string? Reason { get; }

        public CooperativeMemberRemovedDomainEvent(Cooperative cooperative, Guid memberId, string? reason)
        {
            Cooperative = cooperative;
            MemberId = memberId;
            Reason = reason;
        }
    }

    public class CooperativeDocumentAddedDomainEvent : INotification
    {
        public Cooperative Cooperative { get; }
        public CooperativeDocument Document { get; }

        public CooperativeDocumentAddedDomainEvent(Cooperative cooperative, CooperativeDocument document)
        {
            Cooperative = cooperative;
            Document = document;
        }
    }

    public class CooperativeMeetingScheduledDomainEvent : INotification
    {
        public Cooperative Cooperative { get; }
        public CooperativeMeeting Meeting { get; }

        public CooperativeMeetingScheduledDomainEvent(Cooperative cooperative, CooperativeMeeting meeting)
        {
            Cooperative = cooperative;
            Meeting = meeting;
        }
    }

    public class CooperativeMemberShareValueUpdatedDomainEvent : INotification
    {
        public Cooperative Cooperative { get; }
        public Guid MemberId { get; }
        public decimal NewShareValue { get; }

        public CooperativeMemberShareValueUpdatedDomainEvent(Cooperative cooperative, Guid memberId, decimal newShareValue)
        {
            Cooperative = cooperative;
            MemberId = memberId;
            NewShareValue = newShareValue;
        }
    }
} 
