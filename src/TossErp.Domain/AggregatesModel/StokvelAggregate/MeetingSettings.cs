using System;
using TossErp.Domain.SeedWork;

namespace TossErp.Domain.AggregatesModel.StokvelAggregate
{
    public class MeetingSettings : ValueObject
    {
        public string MeetingFrequency { get; private set; } = string.Empty; // Weekly, Monthly, etc.
        public string? DefaultLocation { get; private set; }
        public TimeSpan? DefaultTime { get; private set; }
        public int QuorumRequired { get; private set; }
        public bool IsActive { get; private set; }
        public DateTime? LastMeetingDate { get; private set; }

        public MeetingSettings(string meetingFrequency = "Monthly", string? defaultLocation = null, TimeSpan? defaultTime = null, int quorumRequired = 3)
        {
            if (string.IsNullOrWhiteSpace(meetingFrequency))
                throw new ArgumentException("Meeting frequency is required.", nameof(meetingFrequency));

            if (quorumRequired <= 0)
                throw new ArgumentException("Quorum required must be greater than zero.", nameof(quorumRequired));

            MeetingFrequency = meetingFrequency;
            DefaultLocation = defaultLocation;
            DefaultTime = defaultTime;
            QuorumRequired = quorumRequired;
            IsActive = true;
        }

        public void UpdateMeetingFrequency(string frequency)
        {
            if (string.IsNullOrWhiteSpace(frequency))
                throw new ArgumentException("Meeting frequency is required.", nameof(frequency));

            MeetingFrequency = frequency;
        }

        public void UpdateDefaultLocation(string? location)
        {
            DefaultLocation = location;
        }

        public void UpdateDefaultTime(TimeSpan? time)
        {
            DefaultTime = time;
        }

        public void UpdateQuorumRequired(int quorum)
        {
            if (quorum <= 0)
                throw new ArgumentException("Quorum required must be greater than zero.", nameof(quorum));

            QuorumRequired = quorum;
        }

        public void SetLastMeetingDate(DateTime date)
        {
            LastMeetingDate = date;
        }

        public void Deactivate()
        {
            IsActive = false;
        }

        public void Activate()
        {
            IsActive = true;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return MeetingFrequency;
            yield return DefaultLocation ?? string.Empty;
            yield return DefaultTime ?? TimeSpan.Zero;
            yield return QuorumRequired;
            yield return IsActive;
            yield return LastMeetingDate ?? DateTime.MinValue;
        }
    }
} 
