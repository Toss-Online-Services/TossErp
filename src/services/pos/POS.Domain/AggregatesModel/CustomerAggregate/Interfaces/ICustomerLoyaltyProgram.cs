namespace POS.Domain.AggregatesModel.CustomerAggregate.Interfaces
{
    public interface ICustomerLoyaltyProgram
    {
        Guid Id { get; }
        string ProgramName { get; }
        string MembershipNumber { get; }
        DateTime EnrollmentDate { get; }
        DateTime? ExpiryDate { get; }
        string MembershipTier { get; }
        decimal PointsBalance { get; }
        decimal LifetimePoints { get; }
        bool IsActive { get; }
        DateTime? LastPointsEarned { get; }
        DateTime? LastPointsRedeemed { get; }
        string? ReferralCode { get; }
        int ReferralCount { get; }
        DateTime CreatedAt { get; }
        DateTime? LastModifiedAt { get; }
        bool IsExpired { get; }

        void AddPoints(decimal points, string reason);
        void RedeemPoints(decimal points, string reason);
        void UpdateMembershipTier(string newTier);
        void UpdateExpiryDate(DateTime? newExpiryDate);
        void Deactivate();
        void Reactivate();
        void IncrementReferralCount();
    }
} 
