using POS.Domain.Common.Events;

namespace POS.Domain.AggregatesModel.CustomerAggregate.Events;

public class CustomerPaymentTermsUpdatedDomainEvent : IDomainEvent
{
    public Guid CustomerId { get; }
    public string OldPaymentTerms { get; }
    public string NewPaymentTerms { get; }
    public string UpdatedBy { get; }
    public DateTime UpdatedAt { get; }

    public CustomerPaymentTermsUpdatedDomainEvent(
        Guid customerId,
        string oldPaymentTerms,
        string newPaymentTerms,
        string updatedBy,
        DateTime updatedAt)
    {
        CustomerId = customerId;
        OldPaymentTerms = oldPaymentTerms;
        NewPaymentTerms = newPaymentTerms;
        UpdatedBy = updatedBy;
        UpdatedAt = updatedAt;
    }
} 
