using System;
using TossErp.Domain.SeedWork;

namespace TossErp.Domain.AggregatesModel.StokvelAggregate
{
    public class StokvelMeeting : Entity
    {
        public string MeetingTitle { get; private set; } = string.Empty;
        public DateTime MeetingDate { get; private set; }
        public string? Location { get; private set; }
        public string? Agenda { get; private set; }
        public string? Minutes { get; private set; }
        public bool IsCompleted { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? LastModifiedAt { get; private set; }
        public string? Notes { get; private set; }

        protected StokvelMeeting() { }

        public StokvelMeeting(
            string meetingTitle,
            DateTime meetingDate,
            string? location = null,
            string? agenda = null,
            string? notes = null)
        {
            MeetingTitle = meetingTitle ?? throw new ArgumentNullException(nameof(meetingTitle));
            MeetingDate = meetingDate;
            Location = location;
            Agenda = agenda;
            Notes = notes;
            IsCompleted = false;
            CreatedAt = DateTime.UtcNow;
        }

        public void CompleteMeeting(string? minutes = null)
        {
            if (IsCompleted)
                throw new InvalidOperationException("Meeting is already completed.");

            Minutes = minutes;
            IsCompleted = true;
            LastModifiedAt = DateTime.UtcNow;
        }

        public void UpdateMeeting(
            string meetingTitle,
            DateTime meetingDate,
            string? location = null,
            string? agenda = null)
        {
            if (IsCompleted)
                throw new InvalidOperationException("Cannot update a completed meeting.");

            MeetingTitle = meetingTitle ?? throw new ArgumentNullException(nameof(meetingTitle));
            MeetingDate = meetingDate;
            Location = location;
            Agenda = agenda;
            LastModifiedAt = DateTime.UtcNow;
        }

        public void UpdateMinutes(string? minutes)
        {
            Minutes = minutes;
            LastModifiedAt = DateTime.UtcNow;
        }

        public void UpdateNotes(string? notes)
        {
            Notes = notes;
            LastModifiedAt = DateTime.UtcNow;
        }

        public bool IsUpcoming => !IsCompleted && MeetingDate > DateTime.UtcNow;

        public bool IsPast => MeetingDate < DateTime.UtcNow;

        public bool IsToday => MeetingDate.Date == DateTime.UtcNow.Date;

        public TimeSpan GetTimeUntilMeeting()
        {
            return MeetingDate - DateTime.UtcNow;
        }
    }
} 
