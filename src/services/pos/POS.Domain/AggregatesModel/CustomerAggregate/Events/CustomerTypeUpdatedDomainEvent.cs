using POS.Domain.Common.Events;

namespace POS.Domain.AggregatesModel.CustomerAggregate.Events;

public class CustomerTypeUpdatedDomainEvent : IDomainEvent
{
    public Guid CustomerId { get; }
    public string OldCustomerType { get; }
    public string NewCustomerType { get; }
    public string UpdatedBy { get; }
    public DateTime UpdatedAt { get; }

    public CustomerTypeUpdatedDomainEvent(
        Guid customerId,
        string oldCustomerType,
        string newCustomerType,
        string updatedBy,
        DateTime updatedAt)
    {
        CustomerId = customerId;
        OldCustomerType = oldCustomerType;
        NewCustomerType = newCustomerType;
        UpdatedBy = updatedBy;
        UpdatedAt = updatedAt;
    }
} 
