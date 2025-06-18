using System;
using System.Linq.Expressions;
using POS.Domain.SeedWork;
using POS.Domain.AggregatesModel.CustomerAggregate;
using POS.Domain.Common;
using POS.Domain.Exceptions;

namespace POS.Domain.AggregatesModel.CustomerAggregate.Specifications;

public static class ComplexCustomerSpecifications
{
    public class HasActiveLoyaltyProgram : Specification<Customer>
    {
        public override bool IsSatisfiedBy(Customer customer)
        {
            return customer.LoyaltyProgram?.IsActive == true && 
                   !customer.LoyaltyProgram.IsExpired;
        }
        public override Expression<Func<Customer, bool>> ToExpression() => customer => IsSatisfiedBy(customer);
    }

    public class HasAvailablePoints : Specification<Customer>
    {
        private readonly decimal _requiredPoints;

        public HasAvailablePoints(decimal requiredPoints)
        {
            if (requiredPoints <= 0)
                throw new DomainException("Required points must be greater than zero");
            _requiredPoints = requiredPoints;
        }

        public override bool IsSatisfiedBy(Customer customer)
        {
            return customer.LoyaltyProgram?.IsActive == true && 
                   !customer.LoyaltyProgram.IsExpired &&
                   customer.LoyaltyProgram.PointsBalance >= _requiredPoints;
        }
        public override Expression<Func<Customer, bool>> ToExpression() => customer => IsSatisfiedBy(customer);
    }

    public class IsEligibleForTierUpgrade : Specification<Customer>
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

        public override bool IsSatisfiedBy(Customer customer)
        {
            return customer.LoyaltyProgram?.IsActive == true && 
                   !customer.LoyaltyProgram.IsExpired &&
                   customer.LoyaltyProgram.PointsBalance >= _requiredPoints &&
                   customer.LoyaltyProgram.MembershipTier != _targetTier;
        }
        public override Expression<Func<Customer, bool>> ToExpression() => customer => IsSatisfiedBy(customer);
    }

    public class HasRecentPurchase : Specification<Customer>
    {
        private readonly int _daysThreshold;

        public HasRecentPurchase(int daysThreshold)
        {
            if (daysThreshold <= 0)
                throw new DomainException("Days threshold must be greater than zero");
            _daysThreshold = daysThreshold;
        }

        public override bool IsSatisfiedBy(Customer customer)
        {
            return customer.LastPurchaseDate.HasValue && 
                   (DateTime.UtcNow - customer.LastPurchaseDate.Value).TotalDays <= _daysThreshold;
        }
        public override Expression<Func<Customer, bool>> ToExpression() => customer => IsSatisfiedBy(customer);
    }

    public class HasHighValuePurchases : Specification<Customer>
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

        public override bool IsSatisfiedBy(Customer customer)
        {
            return customer.LastPurchaseDate.HasValue && 
                   (DateTime.UtcNow - customer.LastPurchaseDate.Value).TotalDays <= _daysThreshold &&
                   customer.TotalPurchases >= _minimumValue;
        }
        public override Expression<Func<Customer, bool>> ToExpression() => customer => IsSatisfiedBy(customer);
    }

    public class HasPreferredPaymentMethod : Specification<Customer>
    {
        private readonly string _paymentMethod;

        public HasPreferredPaymentMethod(string paymentMethod)
        {
            if (string.IsNullOrWhiteSpace(paymentMethod))
                throw new DomainException("Payment method cannot be empty");
            _paymentMethod = paymentMethod;
        }

        public override bool IsSatisfiedBy(Customer customer)
        {
            return customer.Preferences?.PreferredPaymentMethod == _paymentMethod;
        }
        public override Expression<Func<Customer, bool>> ToExpression() => customer => IsSatisfiedBy(customer);
    }

    public class HasPreferredShippingMethod : Specification<Customer>
    {
        private readonly string _shippingMethod;

        public HasPreferredShippingMethod(string shippingMethod)
        {
            if (string.IsNullOrWhiteSpace(shippingMethod))
                throw new DomainException("Shipping method cannot be empty");
            _shippingMethod = shippingMethod;
        }

        public override bool IsSatisfiedBy(Customer customer)
        {
            return customer.Preferences?.PreferredShippingMethod == _shippingMethod;
        }
        public override Expression<Func<Customer, bool>> ToExpression() => customer => IsSatisfiedBy(customer);
    }

    public class HasOptedInForMarketing : Specification<Customer>
    {
        public override bool IsSatisfiedBy(Customer customer)
        {
            return customer.Preferences?.OptInMarketing == true;
        }
        public override Expression<Func<Customer, bool>> ToExpression() => customer => IsSatisfiedBy(customer);
    }

    public class HasOptedInForThirdParty : Specification<Customer>
    {
        public override bool IsSatisfiedBy(Customer customer)
        {
            return customer.Preferences?.OptInThirdParty == true;
        }
        public override Expression<Func<Customer, bool>> ToExpression() => customer => IsSatisfiedBy(customer);
    }

    public class HasSpecialRequirements : Specification<Customer>
    {
        public override bool IsSatisfiedBy(Customer customer)
        {
            return customer.Preferences != null && 
                   (!string.IsNullOrWhiteSpace(customer.Preferences.DietaryRestrictions) ||
                    !string.IsNullOrWhiteSpace(customer.Preferences.SpecialInstructions));
        }
        public override Expression<Func<Customer, bool>> ToExpression() => customer => IsSatisfiedBy(customer);
    }
} 
