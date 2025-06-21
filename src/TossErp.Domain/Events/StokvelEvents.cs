using System;
using MediatR;
using TossErp.Domain.AggregatesModel.StokvelAggregate;

namespace TossErp.Domain.Events
{
    public class StokvelCreatedDomainEvent : INotification
    {
        public Stokvel Stokvel { get; }

        public StokvelCreatedDomainEvent(Stokvel stokvel)
        {
            Stokvel = stokvel;
        }
    }

    public class StokvelUpdatedDomainEvent : INotification
    {
        public Stokvel Stokvel { get; }

        public StokvelUpdatedDomainEvent(Stokvel stokvel)
        {
            Stokvel = stokvel;
        }
    }

    public class StokvelActivatedDomainEvent : INotification
    {
        public Stokvel Stokvel { get; }

        public StokvelActivatedDomainEvent(Stokvel stokvel)
        {
            Stokvel = stokvel;
        }
    }

    public class StokvelDeactivatedDomainEvent : INotification
    {
        public Stokvel Stokvel { get; }

        public StokvelDeactivatedDomainEvent(Stokvel stokvel)
        {
            Stokvel = stokvel;
        }
    }

    public class StokvelMemberAddedDomainEvent : INotification
    {
        public Stokvel Stokvel { get; }
        public StokvelMember Member { get; }

        public StokvelMemberAddedDomainEvent(Stokvel stokvel, StokvelMember member)
        {
            Stokvel = stokvel;
            Member = member;
        }
    }

    public class StokvelMemberUpdatedDomainEvent : INotification
    {
        public Stokvel Stokvel { get; }
        public StokvelMember Member { get; }

        public StokvelMemberUpdatedDomainEvent(Stokvel stokvel, StokvelMember member)
        {
            Stokvel = stokvel;
            Member = member;
        }
    }

    public class StokvelMemberRemovedDomainEvent : INotification
    {
        public Stokvel Stokvel { get; }
        public Guid MemberId { get; }
        public string? Reason { get; }

        public StokvelMemberRemovedDomainEvent(Stokvel stokvel, Guid memberId, string? reason)
        {
            Stokvel = stokvel;
            MemberId = memberId;
            Reason = reason;
        }
    }

    public class StokvelContributionRecordedDomainEvent : INotification
    {
        public Stokvel Stokvel { get; }
        public StokvelContribution Contribution { get; }

        public StokvelContributionRecordedDomainEvent(Stokvel stokvel, StokvelContribution contribution)
        {
            Stokvel = stokvel;
            Contribution = contribution;
        }
    }

    public class StokvelPayoutProcessedDomainEvent : INotification
    {
        public Stokvel Stokvel { get; }
        public StokvelPayout Payout { get; }

        public StokvelPayoutProcessedDomainEvent(Stokvel stokvel, StokvelPayout payout)
        {
            Stokvel = stokvel;
            Payout = payout;
        }
    }

    public class StokvelMeetingScheduledDomainEvent : INotification
    {
        public Stokvel Stokvel { get; }
        public StokvelMeeting Meeting { get; }

        public StokvelMeetingScheduledDomainEvent(Stokvel stokvel, StokvelMeeting meeting)
        {
            Stokvel = stokvel;
            Meeting = meeting;
        }
    }
} 
