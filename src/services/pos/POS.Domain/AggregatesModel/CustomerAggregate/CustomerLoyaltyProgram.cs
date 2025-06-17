using POS.Domain.Common;
using POS.Domain.Exceptions;

namespace POS.Domain.AggregatesModel.CustomerAggregate
{
    public class CustomerLoyaltyProgram : Entity
    {
        public string ProgramName { get; private set; }
        public string MembershipNumber { get; private set; }
        public DateTime EnrollmentDate { get; private set; }
        public DateTime? ExpiryDate { get; private set; }
        public string MembershipTier { get; private set; }
        public decimal PointsBalance { get; private set; }
        public decimal LifetimePoints { get; private set; }
        public bool IsActive { get; private set; }
        public DateTime? LastPointsEarned { get; private set; }
        public DateTime? LastPointsRedeemed { get; private set; }
        public string? ReferralCode { get; private set; }
        public int ReferralCount { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? LastModifiedAt { get; private set; }

        private CustomerLoyaltyProgram() { }

        public CustomerLoyaltyProgram(
            string programName,
            string membershipNumber,
            string membershipTier,
            DateTime? expiryDate = null,
            string? referralCode = null)
        {
            if (string.IsNullOrWhiteSpace(programName))
                throw new DomainException("Program name cannot be empty");
            if (string.IsNullOrWhiteSpace(membershipNumber))
                throw new DomainException("Membership number cannot be empty");
            if (string.IsNullOrWhiteSpace(membershipTier))
                throw new DomainException("Membership tier cannot be empty");

            Id = Guid.NewGuid();
            ProgramName = programName;
            MembershipNumber = membershipNumber;
            MembershipTier = membershipTier;
            EnrollmentDate = DateTime.UtcNow;
            ExpiryDate = expiryDate;
            PointsBalance = 0;
            LifetimePoints = 0;
            IsActive = true;
            ReferralCode = referralCode;
            ReferralCount = 0;
            CreatedAt = DateTime.UtcNow;
        }

        public void AddPoints(decimal points, string reason)
        {
            if (points <= 0)
                throw new DomainException("Points must be greater than zero");
            if (string.IsNullOrWhiteSpace(reason))
                throw new DomainException("Reason for points cannot be empty");

            PointsBalance += points;
            LifetimePoints += points;
            LastPointsEarned = DateTime.UtcNow;
            LastModifiedAt = DateTime.UtcNow;
        }

        public void RedeemPoints(decimal points, string reason)
        {
            if (points <= 0)
                throw new DomainException("Points must be greater than zero");
            if (points > PointsBalance)
                throw new DomainException("Insufficient points balance");
            if (string.IsNullOrWhiteSpace(reason))
                throw new DomainException("Reason for redemption cannot be empty");

            PointsBalance -= points;
            LastPointsRedeemed = DateTime.UtcNow;
            LastModifiedAt = DateTime.UtcNow;
        }

        public void UpdateMembershipTier(string newTier)
        {
            if (string.IsNullOrWhiteSpace(newTier))
                throw new DomainException("Membership tier cannot be empty");

            MembershipTier = newTier;
            LastModifiedAt = DateTime.UtcNow;
        }

        public void UpdateExpiryDate(DateTime? newExpiryDate)
        {
            ExpiryDate = newExpiryDate;
            LastModifiedAt = DateTime.UtcNow;
        }

        public void Deactivate()
        {
            IsActive = false;
            LastModifiedAt = DateTime.UtcNow;
        }

        public void Reactivate()
        {
            IsActive = true;
            LastModifiedAt = DateTime.UtcNow;
        }

        public void IncrementReferralCount()
        {
            ReferralCount++;
            LastModifiedAt = DateTime.UtcNow;
        }

        public bool IsExpired => ExpiryDate.HasValue && ExpiryDate.Value < DateTime.UtcNow;
    }
} 
