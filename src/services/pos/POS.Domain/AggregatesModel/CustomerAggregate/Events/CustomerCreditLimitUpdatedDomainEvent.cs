using POS.Domain.Common.Events;

namespace POS.Domain.AggregatesModel.CustomerAggregate.Events;

public class CustomerCreditLimitUpdatedDomainEvent : IDomainEvent
{
    public Guid CustomerId { get; }
    public decimal OldCreditLimit { get; }
    public decimal NewCreditLimit { get; }
    public string UpdatedBy { get; }
    public DateTime UpdatedAt { get; }

    public CustomerCreditLimitUpdatedDomainEvent(
        Guid customerId,
        decimal oldCreditLimit,
        decimal newCreditLimit,
        string updatedBy,
        DateTime updatedAt)
    {
        CustomerId = customerId;
        OldCreditLimit = oldCreditLimit;
        NewCreditLimit = newCreditLimit;
        UpdatedBy = updatedBy;
        UpdatedAt = updatedAt;
    }
} 
