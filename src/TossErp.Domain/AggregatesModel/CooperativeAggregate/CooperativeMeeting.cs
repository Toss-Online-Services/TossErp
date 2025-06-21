using System;
using System.Collections.Generic;
using System.Linq;
using TossErp.Domain.SeedWork;
using TossErp.Domain.Exceptions;

namespace TossErp.Domain.AggregatesModel.CooperativeAggregate
{
    public class CooperativeMeeting : Entity
    {
        public string MeetingType { get; private set; } = string.Empty;
        public string Subject { get; private set; } = string.Empty;
        public string Description { get; private set; } = string.Empty;
        public DateTime ScheduledDate { get; private set; }
        public string? Location { get; private set; }
        public bool IsCompleted { get; private set; }
        public DateTime CreatedDate { get; private set; }
        public DateTime? LastModifiedDate { get; private set; }
        public List<MeetingAttendee> Attendees { get; private set; }
        public List<MeetingMinute> Minutes { get; private set; }

        protected CooperativeMeeting()
        {
            Attendees = new List<MeetingAttendee>();
            Minutes = new List<MeetingMinute>();
        }

        public CooperativeMeeting(
            string meetingType,
            string subject,
            string description,
            DateTime scheduledDate,
            string? location = null)
        {
            MeetingType = meetingType ?? throw new ArgumentNullException(nameof(meetingType));
            Subject = subject ?? throw new ArgumentNullException(nameof(subject));
            Description = description ?? throw new ArgumentNullException(nameof(description));
            ScheduledDate = scheduledDate;
            Location = location;
            IsCompleted = false;
            CreatedDate = DateTime.UtcNow;
            Attendees = new List<MeetingAttendee>();
            Minutes = new List<MeetingMinute>();
        }

        public void UpdateDetails(
            string meetingType,
            string subject,
            string description,
            DateTime scheduledDate,
            string? location = null)
        {
            MeetingType = meetingType ?? throw new ArgumentNullException(nameof(meetingType));
            Subject = subject ?? throw new ArgumentNullException(nameof(subject));
            Description = description ?? throw new ArgumentNullException(nameof(description));
            ScheduledDate = scheduledDate;
            Location = location;
            LastModifiedDate = DateTime.UtcNow;
        }

        public void MarkAsCompleted()
        {
            IsCompleted = true;
            LastModifiedDate = DateTime.UtcNow;
        }

        public void AddAttendee(Guid memberId, string memberName, bool isPresent = false)
        {
            if (Attendees.Any(a => a.MemberId == memberId))
                throw new TossErpDomainException($"Member with ID '{memberId}' is already an attendee.");

            var attendee = new MeetingAttendee(memberId, memberName, isPresent);
            Attendees.Add(attendee);
        }

        public void MarkAttendance(Guid memberId, bool isPresent)
        {
            var attendee = Attendees.FirstOrDefault(a => a.MemberId == memberId);
            if (attendee == null)
                throw new TossErpDomainException($"Attendee with ID '{memberId}' not found.");

            attendee.MarkAttendance(isPresent);
        }

        public void AddMinute(string topic, string discussion, string? actionItems = null)
        {
            var minute = new MeetingMinute(topic, discussion, actionItems);
            Minutes.Add(minute);
        }

        public bool IsUpcoming()
        {
            return ScheduledDate > DateTime.UtcNow && !IsCompleted;
        }

        public bool IsToday()
        {
            return ScheduledDate.Date == DateTime.UtcNow.Date;
        }

        public int GetAttendanceCount()
        {
            return Attendees.Count(a => a.IsPresent);
        }

        public decimal GetAttendancePercentage()
        {
            if (Attendees.Count == 0) return 0;
            return (decimal)GetAttendanceCount() / Attendees.Count * 100;
        }
    }

    public class MeetingAttendee : Entity
    {
        public Guid MemberId { get; private set; }
        public string MemberName { get; private set; } = string.Empty;
        public bool IsPresent { get; private set; }
        public DateTime? ArrivalTime { get; private set; }
        public DateTime? DepartureTime { get; private set; }

        protected MeetingAttendee() { }

        public MeetingAttendee(Guid memberId, string memberName, bool isPresent = false)
        {
            MemberId = memberId;
            MemberName = memberName ?? throw new ArgumentNullException(nameof(memberName));
            IsPresent = isPresent;
        }

        public void MarkAttendance(bool isPresent)
        {
            IsPresent = isPresent;
            if (isPresent)
                ArrivalTime = DateTime.UtcNow;
            else
                DepartureTime = DateTime.UtcNow;
        }
    }

    public class MeetingMinute : Entity
    {
        public string Topic { get; private set; } = string.Empty;
        public string Discussion { get; private set; } = string.Empty;
        public string? ActionItems { get; private set; }
        public DateTime CreatedAt { get; private set; }

        protected MeetingMinute() { }

        public MeetingMinute(string topic, string discussion, string? actionItems = null)
        {
            Topic = topic ?? throw new ArgumentNullException(nameof(topic));
            Discussion = discussion ?? throw new ArgumentNullException(nameof(discussion));
            ActionItems = actionItems;
            CreatedAt = DateTime.UtcNow;
        }
    }
}
