using POS.Domain.Common;
using POS.Domain.Exceptions;

namespace POS.Domain.AggregatesModel.CustomerAggregate
{
    public class CustomerSegment : Entity
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public string? Criteria { get; private set; }
        public decimal? MinimumSpend { get; private set; }
        public int? MinimumOrders { get; private set; }
        public DateTime? StartDate { get; private set; }
        public DateTime? EndDate { get; private set; }
        public bool IsActive { get; private set; }
        public string? Tags { get; private set; }
        public int CustomerCount { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? LastModifiedAt { get; private set; }
        public string CreatedBy { get; private set; }
        public string? LastModifiedBy { get; private set; }

        private CustomerSegment() { }

        public CustomerSegment(
            string name,
            string description,
            string createdBy,
            string? criteria = null,
            decimal? minimumSpend = null,
            int? minimumOrders = null,
            DateTime? startDate = null,
            DateTime? endDate = null,
            string? tags = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("Segment name cannot be empty");
            if (string.IsNullOrWhiteSpace(description))
                throw new DomainException("Segment description cannot be empty");
            if (string.IsNullOrWhiteSpace(createdBy))
                throw new DomainException("Creator cannot be empty");
            if (minimumSpend.HasValue && minimumSpend.Value < 0)
                throw new DomainException("Minimum spend cannot be negative");
            if (minimumOrders.HasValue && minimumOrders.Value < 0)
                throw new DomainException("Minimum orders cannot be negative");
            if (startDate.HasValue && endDate.HasValue && startDate.Value > endDate.Value)
                throw new DomainException("Start date cannot be after end date");

            Id = Guid.NewGuid();
            Name = name;
            Description = description;
            Criteria = criteria;
            MinimumSpend = minimumSpend;
            MinimumOrders = minimumOrders;
            StartDate = startDate;
            EndDate = endDate;
            Tags = tags;
            IsActive = true;
            CustomerCount = 0;
            CreatedAt = DateTime.UtcNow;
            CreatedBy = createdBy;
        }

        public void UpdateDetails(
            string name,
            string description,
            string modifiedBy,
            string? criteria = null,
            decimal? minimumSpend = null,
            int? minimumOrders = null,
            DateTime? startDate = null,
            DateTime? endDate = null,
            string? tags = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("Segment name cannot be empty");
            if (string.IsNullOrWhiteSpace(description))
                throw new DomainException("Segment description cannot be empty");
            if (string.IsNullOrWhiteSpace(modifiedBy))
                throw new DomainException("Modifier cannot be empty");
            if (minimumSpend.HasValue && minimumSpend.Value < 0)
                throw new DomainException("Minimum spend cannot be negative");
            if (minimumOrders.HasValue && minimumOrders.Value < 0)
                throw new DomainException("Minimum orders cannot be negative");
            if (startDate.HasValue && endDate.HasValue && startDate.Value > endDate.Value)
                throw new DomainException("Start date cannot be after end date");

            Name = name;
            Description = description;
            Criteria = criteria;
            MinimumSpend = minimumSpend;
            MinimumOrders = minimumOrders;
            StartDate = startDate;
            EndDate = endDate;
            Tags = tags;
            LastModifiedAt = DateTime.UtcNow;
            LastModifiedBy = modifiedBy;
        }

        public void Deactivate(string modifiedBy)
        {
            if (string.IsNullOrWhiteSpace(modifiedBy))
                throw new DomainException("Modifier cannot be empty");

            IsActive = false;
            LastModifiedAt = DateTime.UtcNow;
            LastModifiedBy = modifiedBy;
        }

        public void Reactivate(string modifiedBy)
        {
            if (string.IsNullOrWhiteSpace(modifiedBy))
                throw new DomainException("Modifier cannot be empty");

            IsActive = true;
            LastModifiedAt = DateTime.UtcNow;
            LastModifiedBy = modifiedBy;
        }

        public void IncrementCustomerCount()
        {
            CustomerCount++;
            LastModifiedAt = DateTime.UtcNow;
        }

        public void DecrementCustomerCount()
        {
            if (CustomerCount > 0)
            {
                CustomerCount--;
                LastModifiedAt = DateTime.UtcNow;
            }
        }

        public bool IsExpired => EndDate.HasValue && EndDate.Value < DateTime.UtcNow;
        public bool IsActiveAndValid => IsActive && !IsExpired;
    }
} 
