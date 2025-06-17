using POS.Domain.Common.Events;

namespace POS.Domain.AggregatesModel.CustomerAggregate.Events;

public class CustomerBalanceUpdatedDomainEvent : IDomainEvent
{
    public Guid CustomerId { get; }
    public decimal OldBalance { get; }
    public decimal NewBalance { get; }
    public string TransactionType { get; }
    public string TransactionReference { get; }
    public DateTime UpdatedAt { get; }

    public CustomerBalanceUpdatedDomainEvent(
        Guid customerId,
        decimal oldBalance,
        decimal newBalance,
        string transactionType,
        string transactionReference,
        DateTime updatedAt)
    {
        CustomerId = customerId;
        OldBalance = oldBalance;
        NewBalance = newBalance;
        TransactionType = transactionType;
        TransactionReference = transactionReference;
        UpdatedAt = updatedAt;
    }
} 
