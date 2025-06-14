#nullable enable
using TossErp.POS.Domain.Events;

namespace TossErp.POS.Domain.AggregatesModel.SaleAggregate.Events;

public class SaleCompletedDomainEvent : DomainEvent
{
    public int SaleId { get; }
    public decimal TotalAmount { get; }

    public SaleCompletedDomainEvent(int saleId, decimal totalAmount)
    {
        SaleId = saleId;
        TotalAmount = totalAmount;
    }
} 
