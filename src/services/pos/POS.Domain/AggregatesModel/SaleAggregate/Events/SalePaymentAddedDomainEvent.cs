#nullable enable
using POS.Domain.SeedWork;

namespace POS.Domain.AggregatesModel.SaleAggregate.Events;

public class SalePaymentAddedDomainEvent : DomainEvent
{
    public Sale Sale { get; }
    public Payment Payment { get; }

    public SalePaymentAddedDomainEvent(Sale sale, Payment payment)
    {
        Sale = sale;
        Payment = payment;
    }
} 
