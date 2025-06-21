using System;
using TossErp.Domain.SeedWork;

namespace TossErp.Domain.AggregatesModel.StokvelAggregate
{
    public class StokvelPayout : Entity
    {
        public Guid MemberId { get; private set; }
        public decimal Amount { get; private set; }
        public DateTime PayoutDate { get; private set; }
        public string? ReferenceNumber { get; private set; }
        public bool IsProcessed { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? ProcessedAt { get; private set; }
        public string? Notes { get; private set; }

        protected StokvelPayout() { }

        public StokvelPayout(
            Guid memberId,
            decimal amount,
            DateTime payoutDate,
            string? referenceNumber = null,
            string? notes = null)
        {
            MemberId = memberId;
            Amount = amount;
            PayoutDate = payoutDate;
            ReferenceNumber = referenceNumber;
            Notes = notes;
            IsProcessed = false;
            CreatedAt = DateTime.UtcNow;

            if (amount <= 0)
                throw new ArgumentException("Payout amount must be greater than zero.");
        }

        public void ProcessPayout()
        {
            if (IsProcessed)
                throw new InvalidOperationException("Payout is already processed.");

            IsProcessed = true;
            ProcessedAt = DateTime.UtcNow;
        }

        public void UpdateNotes(string? notes)
        {
            Notes = notes;
        }

        public bool IsPending()
        {
            return !IsProcessed;
        }

        public bool IsOverdue(int daysThreshold = 3)
        {
            return !IsProcessed && PayoutDate < DateTime.UtcNow.AddDays(-daysThreshold);
        }
    }
} 
