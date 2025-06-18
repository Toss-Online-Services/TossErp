using POS.Domain.SeedWork;
using POS.Domain.AggregatesModel.CustomerAggregate;
using POS.Domain.Common;
using POS.Domain.Exceptions;
using POS.Domain.Specifications;

namespace POS.Domain.AggregatesModel.CustomerAggregate.Specifications;

public static class ComplexCustomerSpecifications
{
    public class HasActiveLoyaltyProgram : ISpecification<Customer>
    {
        public bool IsSatisfiedBy(Customer customer)
        {
            return customer.LoyaltyProgram != null && 
                   customer.LoyaltyProgram.IsActive && 
                   !customer.LoyaltyProgram.IsExpired;
        }
    }

    public class HasAvailablePoints : ISpecification<Customer>
    {
        private readonly decimal _requiredPoints;

        public HasAvailablePoints(decimal requiredPoints)
        {
            if (requiredPoints <= 0)
                throw new DomainException("Required points must be greater than zero");
            _requiredPoints = requiredPoints;
        }

        public bool IsSatisfiedBy(Customer customer)
        {
            return customer.LoyaltyProgram != null && 
                   customer.LoyaltyProgram.IsActive && 
                   !customer.LoyaltyProgram.IsExpired &&
                   customer.LoyaltyProgram.PointsBalance >= _requiredPoints;
        }
    }

    public class IsEligibleForTierUpgrade : ISpecification<Customer>
    {
        private readonly decimal _requiredPoints;
        private readonly string _targetTier;

        public IsEligibleForTierUpgrade(decimal requiredPoints, string targetTier)
        {
            if (requiredPoints <= 0)
                throw new DomainException("Required points must be greater than zero");
            if (string.IsNullOrWhiteSpace(targetTier))
                throw new DomainException("Target tier cannot be empty");

            _requiredPoints = requiredPoints;
            _targetTier = targetTier;
        }

        public bool IsSatisfiedBy(Customer customer)
        {
            return customer.LoyaltyProgram != null && 
                   customer.LoyaltyProgram.IsActive && 
                   !customer.LoyaltyProgram.IsExpired &&
                   customer.LoyaltyProgram.PointsBalance >= _requiredPoints &&
                   customer.LoyaltyProgram.MembershipTier != _targetTier;
        }
    }

    public class HasRecentPurchase : ISpecification<Customer>
    {
        private readonly int _daysThreshold;

        public HasRecentPurchase(int daysThreshold)
        {
            if (daysThreshold <= 0)
                throw new DomainException("Days threshold must be greater than zero");
            _daysThreshold = daysThreshold;
        }

        public bool IsSatisfiedBy(Customer customer)
        {
            return customer.LastPurchaseDate.HasValue && 
                   (DateTime.UtcNow - customer.LastPurchaseDate.Value).TotalDays <= _daysThreshold;
        }
    }

    public class HasHighValuePurchases : ISpecification<Customer>
    {
        private readonly decimal _minimumValue;
        private readonly int _daysThreshold;

        public HasHighValuePurchases(decimal minimumValue, int daysThreshold)
        {
            if (minimumValue <= 0)
                throw new DomainException("Minimum value must be greater than zero");
            if (daysThreshold <= 0)
                throw new DomainException("Days threshold must be greater than zero");

            _minimumValue = minimumValue;
            _daysThreshold = daysThreshold;
        }

        public bool IsSatisfiedBy(Customer customer)
        {
            return customer.LastPurchaseDate.HasValue && 
                   (DateTime.UtcNow - customer.LastPurchaseDate.Value).TotalDays <= _daysThreshold &&
                   customer.TotalPurchases >= _minimumValue;
        }
    }

    public class HasPreferredPaymentMethod : ISpecification<Customer>
    {
        private readonly string _paymentMethod;

        public HasPreferredPaymentMethod(string paymentMethod)
        {
            if (string.IsNullOrWhiteSpace(paymentMethod))
                throw new DomainException("Payment method cannot be empty");
            _paymentMethod = paymentMethod;
        }

        public bool IsSatisfiedBy(Customer customer)
        {
            return customer.Preferences != null && 
                   customer.Preferences.PreferredPaymentMethod == _paymentMethod;
        }
    }

    public class HasPreferredShippingMethod : ISpecification<Customer>
    {
        private readonly string _shippingMethod;

        public HasPreferredShippingMethod(string shippingMethod)
        {
            if (string.IsNullOrWhiteSpace(shippingMethod))
                throw new DomainException("Shipping method cannot be empty");
            _shippingMethod = shippingMethod;
        }

        public bool IsSatisfiedBy(Customer customer)
        {
            return customer.Preferences != null && 
                   customer.Preferences.PreferredShippingMethod == _shippingMethod;
        }
    }

    public class HasOptedInForMarketing : ISpecification<Customer>
    {
        public bool IsSatisfiedBy(Customer customer)
        {
            return customer.Preferences != null && 
                   customer.Preferences.OptInMarketing;
        }
    }

    public class HasOptedInForThirdParty : ISpecification<Customer>
    {
        public bool IsSatisfiedBy(Customer customer)
        {
            return customer.Preferences != null && 
                   customer.Preferences.OptInThirdParty;
        }
    }

    public class HasSpecialRequirements : ISpecification<Customer>
    {
        public bool IsSatisfiedBy(Customer customer)
        {
            return customer.Preferences != null && 
                   (!string.IsNullOrWhiteSpace(customer.Preferences.DietaryRestrictions) ||
                    !string.IsNullOrWhiteSpace(customer.Preferences.SpecialInstructions));
        }
    }
} 
