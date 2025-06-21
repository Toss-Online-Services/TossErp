using System;
using TossErp.Domain.SeedWork;

namespace TossErp.Domain.AggregatesModel.StokvelAggregate
{
    public class StokvelContribution : Entity
    {
        public Guid MemberId { get; private set; }
        public decimal Amount { get; private set; }
        public DateTime ContributionDate { get; private set; }
        public string? ReferenceNumber { get; private set; }
        public bool IsConfirmed { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? ConfirmedAt { get; private set; }
        public string? Notes { get; private set; }

        protected StokvelContribution() { }

        public StokvelContribution(
            Guid memberId,
            decimal amount,
            DateTime contributionDate,
            string? referenceNumber = null,
            string? notes = null)
        {
            MemberId = memberId;
            Amount = amount;
            ContributionDate = contributionDate;
            ReferenceNumber = referenceNumber;
            Notes = notes;
            IsConfirmed = false;
            CreatedAt = DateTime.UtcNow;

            if (amount <= 0)
                throw new ArgumentException("Contribution amount must be greater than zero.");
        }

        public void ConfirmContribution()
        {
            if (IsConfirmed)
                throw new InvalidOperationException("Contribution is already confirmed.");

            IsConfirmed = true;
            ConfirmedAt = DateTime.UtcNow;
        }

        public void UpdateNotes(string? notes)
        {
            Notes = notes;
        }

        public bool IsLate(int daysThreshold = 7)
        {
            return ContributionDate < DateTime.UtcNow.AddDays(-daysThreshold);
        }

        public bool IsOnTime()
        {
            return ContributionDate.Date <= DateTime.UtcNow.Date;
        }
    }
} 
