using POS.Domain.Common.Events;

namespace POS.Domain.AggregatesModel.CustomerAggregate.Events;

public class CustomerPurchaseRecordedDomainEvent : IDomainEvent
{
    public Guid CustomerId { get; }
    public Guid SaleId { get; }
    public decimal Amount { get; }
    public string Currency { get; }
    public DateTime PurchaseDate { get; }

    public CustomerPurchaseRecordedDomainEvent(
        Guid customerId,
        Guid saleId,
        decimal amount,
        string currency,
        DateTime purchaseDate)
    {
        CustomerId = customerId;
        SaleId = saleId;
        Amount = amount;
        Currency = currency;
        PurchaseDate = purchaseDate;
    }
} 
