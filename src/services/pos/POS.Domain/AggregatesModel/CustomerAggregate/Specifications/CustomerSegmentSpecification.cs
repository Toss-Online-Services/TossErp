using System;
using System.Linq.Expressions;
using POS.Domain.Common;
using POS.Domain.SeedWork;

namespace POS.Domain.AggregatesModel.CustomerAggregate.Specifications
{
    public class CustomerSegmentSpecification : Specification<Customer>
    {
        private readonly CustomerSegment _segment;

        public CustomerSegmentSpecification(CustomerSegment segment)
        {
            _segment = segment ?? throw new ArgumentNullException(nameof(segment));
        }

        public override bool IsSatisfiedBy(Customer customer)
        {
            if (!_segment.IsActiveAndValid)
                return false;

            // Check minimum spend requirement
            if (_segment.MinimumSpend.HasValue && 
                customer.TotalPurchases < _segment.MinimumSpend.Value)
                return false;

            // Check minimum orders requirement
            if (_segment.MinimumOrders.HasValue && 
                customer.PurchaseCount < _segment.MinimumOrders.Value)
                return false;

            // Check custom criteria if specified
            if (!string.IsNullOrWhiteSpace(_segment.Criteria))
            {
                // Here you would implement custom criteria evaluation
                // This could involve complex business rules, external services, etc.
                // For now, we'll just return true if criteria exists
                return true;
            }

            return true;
        }

        public override Expression<Func<Customer, bool>> ToExpression()
        {
            return customer => IsSatisfiedBy(customer);
        }
    }

    public class HighValueCustomerSpecification : Specification<Customer>
    {
        private readonly decimal _minimumSpend;
        private readonly int _minimumOrders;
        private readonly TimeSpan _timeFrame;

        public HighValueCustomerSpecification(
            decimal minimumSpend = 10000,
            int minimumOrders = 10,
            TimeSpan? timeFrame = null)
        {
            _minimumSpend = minimumSpend;
            _minimumOrders = minimumOrders;
            _timeFrame = timeFrame ?? TimeSpan.FromDays(365);
        }

        public override bool IsSatisfiedBy(Customer customer)
        {
            if (customer.LastPurchaseDate == null)
                return false;

            var timeSinceLastPurchase = DateTime.UtcNow - customer.LastPurchaseDate.Value;
            if (timeSinceLastPurchase > _timeFrame)
                return false;

            return customer.TotalPurchases >= _minimumSpend && 
                   customer.PurchaseCount >= _minimumOrders;
        }

        public override Expression<Func<Customer, bool>> ToExpression()
        {
            return customer => IsSatisfiedBy(customer);
        }
    }

    public class LoyalCustomerSpecification : Specification<Customer>
    {
        private readonly int _minimumOrders;
        private readonly TimeSpan _timeFrame;

        public LoyalCustomerSpecification(
            int minimumOrders = 5,
            TimeSpan? timeFrame = null)
        {
            _minimumOrders = minimumOrders;
            _timeFrame = timeFrame ?? TimeSpan.FromDays(180);
        }

        public override bool IsSatisfiedBy(Customer customer)
        {
            if (customer.LastPurchaseDate == null)
                return false;

            var timeSinceLastPurchase = DateTime.UtcNow - customer.LastPurchaseDate.Value;
            if (timeSinceLastPurchase > _timeFrame)
                return false;

            return customer.PurchaseCount >= _minimumOrders;
        }

        public override Expression<Func<Customer, bool>> ToExpression()
        {
            return customer => IsSatisfiedBy(customer);
        }
    }

    public class AtRiskCustomerSpecification : Specification<Customer>
    {
        private readonly TimeSpan _inactivityThreshold;
        private readonly decimal _minimumPreviousSpend;

        public AtRiskCustomerSpecification(
            TimeSpan? inactivityThreshold = null,
            decimal minimumPreviousSpend = 1000)
        {
            _inactivityThreshold = inactivityThreshold ?? TimeSpan.FromDays(90);
            _minimumPreviousSpend = minimumPreviousSpend;
        }

        public override bool IsSatisfiedBy(Customer customer)
        {
            if (customer.LastPurchaseDate == null)
                return false;

            var timeSinceLastPurchase = DateTime.UtcNow - customer.LastPurchaseDate.Value;
            return timeSinceLastPurchase > _inactivityThreshold && 
                   customer.TotalPurchases >= _minimumPreviousSpend;
        }

        public override Expression<Func<Customer, bool>> ToExpression()
        {
            return customer => IsSatisfiedBy(customer);
        }
    }
} 
