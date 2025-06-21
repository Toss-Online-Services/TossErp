using System;
using TossErp.Domain.SeedWork;

namespace TossErp.Domain.AggregatesModel.StokvelAggregate
{
    public class ContributionSettings : ValueObject
    {
        public decimal Amount { get; private set; }
        public string Frequency { get; private set; } = string.Empty; // Weekly, Monthly, etc.
        public DateTime? StartDate { get; private set; }
        public bool IsActive { get; private set; }

        public ContributionSettings(decimal amount, string frequency, DateTime? startDate = null)
        {
            if (amount <= 0)
                throw new ArgumentException("Contribution amount must be greater than zero.", nameof(amount));

            if (string.IsNullOrWhiteSpace(frequency))
                throw new ArgumentException("Contribution frequency is required.", nameof(frequency));

            Amount = amount;
            Frequency = frequency;
            StartDate = startDate ?? DateTime.UtcNow;
            IsActive = true;
        }

        public void UpdateAmount(decimal amount)
        {
            if (amount <= 0)
                throw new ArgumentException("Contribution amount must be greater than zero.", nameof(amount));

            Amount = amount;
        }

        public void UpdateFrequency(string frequency)
        {
            if (string.IsNullOrWhiteSpace(frequency))
                throw new ArgumentException("Contribution frequency is required.", nameof(frequency));

            Frequency = frequency;
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
            yield return Amount;
            yield return Frequency;
            yield return StartDate ?? DateTime.MinValue;
            yield return IsActive;
        }
    }
} 
