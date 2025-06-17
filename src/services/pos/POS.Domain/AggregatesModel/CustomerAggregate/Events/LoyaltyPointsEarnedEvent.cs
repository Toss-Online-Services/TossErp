using POS.Domain.Common;

namespace POS.Domain.AggregatesModel.CustomerAggregate.Events
{
    public class LoyaltyPointsEarnedEvent : DomainEvent
    {
        public Guid CustomerId { get; }
        public Guid LoyaltyProgramId { get; }
        public decimal PointsEarned { get; }
        public decimal NewBalance { get; }
        public string Reason { get; }
        public DateTime EarnedAt { get; }
        public string? TransactionId { get; }

        public LoyaltyPointsEarnedEvent(
            Guid customerId,
            Guid loyaltyProgramId,
            decimal pointsEarned,
            decimal newBalance,
            string reason,
            string? transactionId = null)
        {
            CustomerId = customerId;
            LoyaltyProgramId = loyaltyProgramId;
            PointsEarned = pointsEarned;
            NewBalance = newBalance;
            Reason = reason;
            TransactionId = transactionId;
            EarnedAt = DateTime.UtcNow;
        }
    }
} 
