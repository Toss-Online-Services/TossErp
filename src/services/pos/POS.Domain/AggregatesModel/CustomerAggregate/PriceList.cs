using POS.Domain.Common;

namespace POS.Domain.AggregatesModel.CustomerAggregate
{
    public class PriceList : ValueObject
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public bool IsDefault { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? ExpiresAt { get; private set; }
        public string CreatedBy { get; private set; }
        public bool IsActive { get; private set; }

        private PriceList()
        {
            Id = Guid.NewGuid();
            Name = string.Empty;
            Description = string.Empty;
            IsDefault = false;
            CreatedAt = DateTime.UtcNow;
            CreatedBy = string.Empty;
            IsActive = true;
        }

        public PriceList(
            string name,
            string description,
            bool isDefault,
            string createdBy,
            DateTime? expiresAt = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("Price list name cannot be empty");

            if (string.IsNullOrWhiteSpace(description))
                throw new DomainException("Price list description cannot be empty");

            if (string.IsNullOrWhiteSpace(createdBy))
                throw new DomainException("Creator name cannot be empty");

            Name = name;
            Description = description;
            IsDefault = isDefault;
            CreatedBy = createdBy;
            CreatedAt = DateTime.UtcNow;
            ExpiresAt = expiresAt;
            IsActive = true;
        }

        public void Deactivate()
        {
            if (!IsActive)
                throw new DomainException("Price list is already inactive");

            IsActive = false;
        }

        public void Reactivate()
        {
            if (IsActive)
                throw new DomainException("Price list is already active");

            if (ExpiresAt.HasValue && ExpiresAt.Value < DateTime.UtcNow)
                throw new DomainException("Cannot reactivate an expired price list");

            IsActive = true;
        }

        public void UpdateDetails(string name, string description)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("Price list name cannot be empty");

            if (string.IsNullOrWhiteSpace(description))
                throw new DomainException("Price list description cannot be empty");

            Name = name;
            Description = description;
        }

        public void SetExpiryDate(DateTime? expiryDate)
        {
            if (expiryDate.HasValue && expiryDate.Value < DateTime.UtcNow)
                throw new DomainException("Expiry date cannot be in the past");

            ExpiresAt = expiryDate;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Name;
            yield return Description;
            yield return IsDefault;
            yield return CreatedAt;
            yield return ExpiresAt ?? DateTime.MaxValue;
            yield return CreatedBy;
            yield return IsActive;
        }
    }
} 
