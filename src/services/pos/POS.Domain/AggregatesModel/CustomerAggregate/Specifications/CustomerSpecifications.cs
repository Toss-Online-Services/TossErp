using POS.Domain.SeedWork;
using POS.Domain.Specifications;

namespace POS.Domain.AggregatesModel.CustomerAggregate.Specifications;

public static class CustomerSpecifications
{
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

    public class IsActive : ISpecification<Customer>
    {
        public bool IsSatisfiedBy(Customer customer) => 
            customer.IsActive;
    }

    public class HasLoyaltyProgram : ISpecification<Customer>
    {
        public bool IsSatisfiedBy(Customer customer) => 
            customer.LoyaltyProgram != null;
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
            customer.Address != null;
    }

    public class HasBalance : ISpecification<Customer>
    {
        public bool IsSatisfiedBy(Customer customer) => 
            customer.Balance > 0;
    }
} 
