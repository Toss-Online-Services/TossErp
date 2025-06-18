using System;
using System.Linq.Expressions;
using POS.Domain.SeedWork;

namespace POS.Domain.AggregatesModel.CustomerAggregate.Specifications;

public class CustomerSpecifications
{
    private class SimpleSpecification : Specification<Customer>
    {
        private readonly Func<Customer, bool> _predicate;
        public SimpleSpecification(Func<Customer, bool> predicate) => _predicate = predicate;
        public override bool IsSatisfiedBy(Customer customer) => _predicate(customer);
        public override Expression<Func<Customer, bool>> ToExpression() => c => _predicate(c);
    }

    public class HasAvailableCredit : ISpecification<Customer>
    {
        public bool IsSatisfiedBy(Customer customer) => 
            customer.HasCreditAvailable;
    }

    public class IsOverdue : ISpecification<Customer>
    {
        public bool IsSatisfiedBy(Customer customer) => 
            customer.IsOverdue;
    }

    public static ISpecification<Customer> IsActive()
    {
        return new SimpleSpecification(c => c.IsActive);
    }

    public class HasLoyaltyProgram : ISpecification<Customer>
    {
        public bool IsSatisfiedBy(Customer customer) => customer.LoyaltyProgram is not null;
    }

    public class HasPriceLists : ISpecification<Customer>
    {
        public bool IsSatisfiedBy(Customer customer) => 
            customer.PriceLists.Any();
    }

    public class HasContacts : ISpecification<Customer>
    {
        public bool IsSatisfiedBy(Customer customer) => 
            customer.Contacts.Any();
    }

    public class HasDocuments : ISpecification<Customer>
    {
        public bool IsSatisfiedBy(Customer customer) => 
            customer.Documents.Any();
    }

    public class HasNotes : ISpecification<Customer>
    {
        public bool IsSatisfiedBy(Customer customer) => 
            customer.CustomerNotes.Any();
    }

    public class HasAddress : ISpecification<Customer>
    {
        public bool IsSatisfiedBy(Customer customer) => 
            customer.Address is not null;
    }

    public class HasBalance : ISpecification<Customer>
    {
        public bool IsSatisfiedBy(Customer customer) => 
            customer.Balance > 0;
    }

    public static ISpecification<Customer> HasLoyaltyPoints()
    {
        return new SimpleSpecification(c => c.LoyaltyProgram is not null);
    }

    public static ISpecification<Customer> BySegment(string segment)
    {
        return new SimpleSpecification(c => c.CustomerType.ToString().Equals(segment));
    }

    public static ISpecification<Customer> ByLoyaltyProgram(string program)
    {
        return new SimpleSpecification(c => c.LoyaltyProgram?.Name?.Equals(program) ?? false);
    }
} 
