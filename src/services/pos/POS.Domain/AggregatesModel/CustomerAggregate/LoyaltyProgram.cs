using POS.Domain.Common;

namespace POS.Domain.AggregatesModel.CustomerAggregate
{
    public class LoyaltyProgram : ValueObject
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public string MembershipNumber { get; private set; }
        public string MembershipTier { get; private set; }
        public decimal PointsBalance { get; private set; }
        public DateTime EnrolledAt { get; private set; }
        public DateTime? ExpiresAt { get; private set; }
        public bool IsActive { get; private set; }
        public string EnrolledBy { get; private set; }

        private LoyaltyProgram()
        {
            Id = Guid.NewGuid();
            Name = string.Empty;
            Description = string.Empty;
            MembershipNumber = string.Empty;
            MembershipTier = string.Empty;
            PointsBalance = 0;
            EnrolledAt = DateTime.UtcNow;
            IsActive = true;
            EnrolledBy = string.Empty;
        }

        public LoyaltyProgram(
            string name,
            string description,
            string membershipNumber,
            string membershipTier,
            string enrolledBy,
            DateTime? expiresAt = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("Loyalty program name cannot be empty");

            if (string.IsNullOrWhiteSpace(description))
                throw new DomainException("Loyalty program description cannot be empty");

            if (string.IsNullOrWhiteSpace(membershipNumber))
                throw new DomainException("Membership number cannot be empty");

            if (string.IsNullOrWhiteSpace(membershipTier))
                throw new DomainException("Membership tier cannot be empty");

            if (string.IsNullOrWhiteSpace(enrolledBy))
                throw new DomainException("Enroller name cannot be empty");

            Name = name;
            Description = description;
            MembershipNumber = membershipNumber;
            MembershipTier = membershipTier;
            PointsBalance = 0;
            EnrolledAt = DateTime.UtcNow;
            ExpiresAt = expiresAt;
            IsActive = true;
            EnrolledBy = enrolledBy;
        }

        public void AddPoints(decimal points, string reason)
        {
            if (points <= 0)
                throw new DomainException("Points to add must be greater than zero");

            if (string.IsNullOrWhiteSpace(reason))
                throw new DomainException("Reason for adding points cannot be empty");

            if (!IsActive)
                throw new DomainException("Cannot add points to an inactive loyalty program");

            if (IsExpired)
                throw new DomainException("Cannot add points to an expired loyalty program");

            PointsBalance += points;
        }

        public void RedeemPoints(decimal points, string reason)
        {
            if (points <= 0)
                throw new DomainException("Points to redeem must be greater than zero");

            if (string.IsNullOrWhiteSpace(reason))
                throw new DomainException("Reason for redeeming points cannot be empty");

            if (!IsActive)
                throw new DomainException("Cannot redeem points from an inactive loyalty program");

            if (IsExpired)
                throw new DomainException("Cannot redeem points from an expired loyalty program");

            if (points > PointsBalance)
                throw new DomainException("Insufficient points balance");

            PointsBalance -= points;
        }

        public void Deactivate()
        {
            if (!IsActive)
                throw new DomainException("Loyalty program is already inactive");

            IsActive = false;
        }

        public void Reactivate()
        {
            if (IsActive)
                throw new DomainException("Loyalty program is already active");

            if (IsExpired)
                throw new DomainException("Cannot reactivate an expired loyalty program");

            IsActive = true;
        }

        public void UpdateMembershipTier(string newTier)
        {
            if (string.IsNullOrWhiteSpace(newTier))
                throw new DomainException("Membership tier cannot be empty");

            if (!IsActive)
                throw new DomainException("Cannot update tier of an inactive loyalty program");

            if (IsExpired)
                throw new DomainException("Cannot update tier of an expired loyalty program");

            MembershipTier = newTier;
        }

        public void SetExpiryDate(DateTime? expiryDate)
        {
            if (expiryDate.HasValue && expiryDate.Value < DateTime.UtcNow)
                throw new DomainException("Expiry date cannot be in the past");

            ExpiresAt = expiryDate;
        }

        public bool IsExpired => ExpiresAt.HasValue && ExpiresAt.Value < DateTime.UtcNow;

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Name;
            yield return Description;
            yield return MembershipNumber;
            yield return MembershipTier;
            yield return PointsBalance;
            yield return EnrolledAt;
            yield return ExpiresAt ?? DateTime.MaxValue;
            yield return IsActive;
            yield return EnrolledBy;
        }
    }
} 
