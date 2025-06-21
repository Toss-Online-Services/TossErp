using System;
using System.Collections.Generic;
using System.Linq;
using TossErp.Domain.SeedWork;
using TossErp.Domain.Enums;
using TossErp.Domain.Events;
using TossErp.Domain.Exceptions;

namespace TossErp.Domain.AggregatesModel.StokvelAggregate
{
    public class Stokvel : Entity, IAggregateRoot
    {
        public string Name { get; private set; } = string.Empty;
        public StokvelType StokvelType { get; private set; }
        public string? Description { get; private set; }
        public ContributionSettings ContributionSettings { get; private set; }
        public PayoutSettings PayoutSettings { get; private set; }
        public MeetingSettings MeetingSettings { get; private set; }
        public int MinimumMembers { get; private set; }
        public int MaxMembers { get; private set; }
        public bool IsActive { get; private set; }
        public DateTime EstablishedDate { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? LastModifiedAt { get; private set; }
        public List<StokvelMember> Members { get; private set; }
        public List<StokvelContribution> Contributions { get; private set; }
        public List<StokvelPayout> Payouts { get; private set; }
        public List<StokvelMeeting> Meetings { get; private set; }

        protected Stokvel()
        {
            ContributionSettings = new ContributionSettings(0, "Monthly");
            PayoutSettings = new PayoutSettings();
            MeetingSettings = new MeetingSettings();
            Members = new List<StokvelMember>();
            Contributions = new List<StokvelContribution>();
            Payouts = new List<StokvelPayout>();
            Meetings = new List<StokvelMeeting>();
        }

        public Stokvel(
            string name,
            string? description,
            StokvelType stokvelType,
            ContributionSettings contributionSettings,
            PayoutSettings payoutSettings,
            MeetingSettings meetingSettings)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Description = description;
            StokvelType = stokvelType;
            ContributionSettings = contributionSettings ?? throw new ArgumentNullException(nameof(contributionSettings));
            PayoutSettings = payoutSettings ?? throw new ArgumentNullException(nameof(payoutSettings));
            MeetingSettings = meetingSettings ?? throw new ArgumentNullException(nameof(meetingSettings));
            MinimumMembers = 5; // Default minimum
            MaxMembers = 20; // Default maximum
            IsActive = true;
            EstablishedDate = DateTime.UtcNow;
            CreatedAt = DateTime.UtcNow;
            Members = new List<StokvelMember>();
            Contributions = new List<StokvelContribution>();
            Payouts = new List<StokvelPayout>();
            Meetings = new List<StokvelMeeting>();

            AddDomainEvent(new StokvelCreatedDomainEvent(this));
        }

        public void UpdateDescription(string description)
        {
            Description = description;
            LastModifiedAt = DateTime.UtcNow;
            AddDomainEvent(new StokvelUpdatedDomainEvent(this));
        }

        public void UpdateContributionSettings(ContributionSettings contributionSettings)
        {
            ContributionSettings = contributionSettings ?? throw new ArgumentNullException(nameof(contributionSettings));
            LastModifiedAt = DateTime.UtcNow;
            AddDomainEvent(new StokvelUpdatedDomainEvent(this));
        }

        public void UpdatePayoutSettings(PayoutSettings payoutSettings)
        {
            PayoutSettings = payoutSettings ?? throw new ArgumentNullException(nameof(payoutSettings));
            LastModifiedAt = DateTime.UtcNow;
            AddDomainEvent(new StokvelUpdatedDomainEvent(this));
        }

        public void UpdateMeetingSettings(MeetingSettings meetingSettings)
        {
            MeetingSettings = meetingSettings ?? throw new ArgumentNullException(nameof(meetingSettings));
            LastModifiedAt = DateTime.UtcNow;
            AddDomainEvent(new StokvelUpdatedDomainEvent(this));
        }

        public void Activate()
        {
            if (IsActive)
                throw new TossErpDomainException("Stokvel is already active.");

            IsActive = true;
            LastModifiedAt = DateTime.UtcNow;

            AddDomainEvent(new StokvelActivatedDomainEvent(this));
        }

        public void Deactivate()
        {
            if (!IsActive)
                throw new TossErpDomainException("Stokvel is already inactive.");

            IsActive = false;
            LastModifiedAt = DateTime.UtcNow;

            AddDomainEvent(new StokvelDeactivatedDomainEvent(this));
        }

        public StokvelMember AddMember(
            string firstName,
            string lastName,
            string idNumber,
            string phoneNumber,
            string email,
            string address)
        {
            if (Members.Count >= MaxMembers)
                throw new TossErpDomainException($"Stokvel has reached its member limit of {MaxMembers}.");

            if (Members.Any(m => m.IdNumber == idNumber))
                throw new TossErpDomainException($"Member with ID number '{idNumber}' is already a member of this stokvel.");

            var member = new StokvelMember(firstName, lastName, idNumber, phoneNumber, email, address);
            Members.Add(member);

            AddDomainEvent(new StokvelMemberAddedDomainEvent(this, member));
            return member;
        }

        public StokvelMember UpdateMember(
            Guid memberId,
            string firstName,
            string lastName,
            string phoneNumber,
            string email,
            string address)
        {
            var member = Members.FirstOrDefault(m => m.Id == memberId);
            if (member == null)
                throw new TossErpDomainException($"Member with ID '{memberId}' not found in stokvel.");

            member.UpdateDetails(firstName, lastName, phoneNumber, email, address);
            LastModifiedAt = DateTime.UtcNow;

            AddDomainEvent(new StokvelMemberUpdatedDomainEvent(this, member));
            return member;
        }

        public void RemoveMember(Guid memberId)
        {
            var member = Members.FirstOrDefault(m => m.Id == memberId);
            if (member == null)
                throw new TossErpDomainException($"Member with ID '{memberId}' not found in stokvel.");

            member.ExitStokvel(DateTime.UtcNow);
            AddDomainEvent(new StokvelMemberRemovedDomainEvent(this, memberId, null));
        }

        public StokvelContribution RecordContribution(
            Guid memberId,
            decimal amount,
            DateTime contributionDate,
            string? referenceNumber = null)
        {
            var member = Members.FirstOrDefault(m => m.Id == memberId && m.IsActive);
            if (member == null)
                throw new TossErpDomainException($"Active member with ID '{memberId}' not found in stokvel.");

            var contribution = new StokvelContribution(memberId, amount, contributionDate, referenceNumber);
            Contributions.Add(contribution);

            AddDomainEvent(new StokvelContributionRecordedDomainEvent(this, contribution));
            return contribution;
        }

        public StokvelPayout ProcessPayout(
            Guid memberId,
            decimal amount,
            DateTime payoutDate,
            string? referenceNumber = null)
        {
            var member = Members.FirstOrDefault(m => m.Id == memberId && m.IsActive);
            if (member == null)
                throw new TossErpDomainException($"Active member with ID '{memberId}' not found in stokvel.");

            var payout = new StokvelPayout(memberId, amount, payoutDate, referenceNumber);
            Payouts.Add(payout);

            AddDomainEvent(new StokvelPayoutProcessedDomainEvent(this, payout));
            return payout;
        }

        public StokvelMeeting ScheduleMeeting(
            string meetingTitle,
            DateTime meetingDate,
            string? location = null,
            string? agenda = null)
        {
            var meeting = new StokvelMeeting(meetingTitle, meetingDate, location, agenda);
            Meetings.Add(meeting);

            AddDomainEvent(new StokvelMeetingScheduledDomainEvent(this, meeting));
            return meeting;
        }

        public decimal GetTotalContributions()
        {
            return Contributions.Sum(c => c.Amount);
        }

        public decimal GetTotalPayouts()
        {
            return Payouts.Sum(p => p.Amount);
        }

        public decimal GetCurrentBalance()
        {
            return GetTotalContributions() - GetTotalPayouts();
        }

        public decimal GetMemberContributionTotal(Guid memberId)
        {
            return Contributions.Where(c => c.MemberId == memberId).Sum(c => c.Amount);
        }

        public decimal GetMemberPayoutTotal(Guid memberId)
        {
            return Payouts.Where(p => p.MemberId == memberId).Sum(p => p.Amount);
        }

        public decimal GetMemberBalance(Guid memberId)
        {
            return GetMemberContributionTotal(memberId) - GetMemberPayoutTotal(memberId);
        }

        public int GetActiveMemberCount()
        {
            return Members.Count(m => m.IsActive);
        }

        public bool HasMinimumMembers(int minimumMembers = 5)
        {
            return GetActiveMemberCount() >= minimumMembers;
        }

        public bool IsFull()
        {
            return GetActiveMemberCount() >= MaxMembers;
        }

        public List<StokvelMember> GetMembersInRotationOrder()
        {
            return Members.Where(m => m.IsActive).OrderBy(m => m.JoinDate).ToList();
        }

        public StokvelMember? GetMember(Guid memberId)
        {
            return Members.FirstOrDefault(m => m.Id == memberId);
        }

        public int GetMemberContributionCount(Guid memberId)
        {
            return Contributions.Count(c => c.MemberId == memberId);
        }

        public int GetMemberPayoutCount(Guid memberId)
        {
            return Payouts.Count(p => p.MemberId == memberId);
        }

        public DateTime GetLastContributionDate(Guid memberId)
        {
            var lastContribution = Contributions
                .Where(c => c.MemberId == memberId)
                .OrderByDescending(c => c.ContributionDate)
                .FirstOrDefault();
            
            return lastContribution?.ContributionDate ?? DateTime.MinValue;
        }

        public DateTime GetLastPayoutDate(Guid memberId)
        {
            var lastPayout = Payouts
                .Where(p => p.MemberId == memberId)
                .OrderByDescending(p => p.PayoutDate)
                .FirstOrDefault();
            
            return lastPayout?.PayoutDate ?? DateTime.MinValue;
        }
    }
} 
