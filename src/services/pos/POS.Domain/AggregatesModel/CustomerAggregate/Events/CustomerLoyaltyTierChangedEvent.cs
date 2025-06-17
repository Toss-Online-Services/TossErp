using POS.Domain.Common;

namespace POS.Domain.AggregatesModel.CustomerAggregate.Events
{
    public class CustomerLoyaltyTierChangedEvent : DomainEvent
    {
        public Guid CustomerId { get; }
        public Guid LoyaltyProgramId { get; }
        public string OldTier { get; }
        public string NewTier { get; }
        public DateTime ChangedAt { get; }
        public string? Reason { get; }
        public decimal PointsAtChange { get; }

        public CustomerLoyaltyTierChangedEvent(
            Guid customerId,
            Guid loyaltyProgramId,
            string oldTier,
            string newTier,
            decimal pointsAtChange,
            string? reason = null)
        {
            CustomerId = customerId;
            LoyaltyProgramId = loyaltyProgramId;
            OldTier = oldTier;
            NewTier = newTier;
            PointsAtChange = pointsAtChange;
            Reason = reason;
            ChangedAt = DateTime.UtcNow;
        }
    }
} 
