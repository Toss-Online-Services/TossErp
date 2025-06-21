using System;
using TossErp.Domain.SeedWork;

namespace TossErp.Domain.AggregatesModel.StokvelAggregate
{
    public class PayoutSettings : ValueObject
    {
        public decimal MinimumPayoutAmount { get; private set; }
        public string PayoutFrequency { get; private set; } = string.Empty; // Monthly, Quarterly, etc.
        public int RotationOrder { get; private set; }
        public bool IsActive { get; private set; }
        public DateTime? LastPayoutDate { get; private set; }

        public PayoutSettings(decimal minimumPayoutAmount = 0, string payoutFrequency = "Monthly", int rotationOrder = 0)
        {
            if (minimumPayoutAmount < 0)
                throw new ArgumentException("Minimum payout amount cannot be negative.", nameof(minimumPayoutAmount));

            if (string.IsNullOrWhiteSpace(payoutFrequency))
                throw new ArgumentException("Payout frequency is required.", nameof(payoutFrequency));

            MinimumPayoutAmount = minimumPayoutAmount;
            PayoutFrequency = payoutFrequency;
            RotationOrder = rotationOrder;
            IsActive = true;
        }

        public void UpdateMinimumPayoutAmount(decimal amount)
        {
            if (amount < 0)
                throw new ArgumentException("Minimum payout amount cannot be negative.", nameof(amount));

            MinimumPayoutAmount = amount;
        }

        public void UpdatePayoutFrequency(string frequency)
        {
            if (string.IsNullOrWhiteSpace(frequency))
                throw new ArgumentException("Payout frequency is required.", nameof(frequency));

            PayoutFrequency = frequency;
        }

        public void UpdateRotationOrder(int order)
        {
            RotationOrder = order;
        }

        public void SetLastPayoutDate(DateTime date)
        {
            LastPayoutDate = date;
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
            yield return MinimumPayoutAmount;
            yield return PayoutFrequency;
            yield return RotationOrder;
            yield return IsActive;
            yield return LastPayoutDate ?? DateTime.MinValue;
        }
    }
} 
