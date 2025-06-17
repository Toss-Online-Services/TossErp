#nullable enable

using POS.Domain.Common.Events;

namespace POS.Domain.AggregatesModel.SaleAggregate.Events;

public class SaleCompletedDomainEvent : IDomainEvent
{
    public Guid SaleId { get; }
    public decimal TotalAmount { get; }
    public DateTime CompletedAt { get; }

    public SaleCompletedDomainEvent(Guid saleId, decimal totalAmount, DateTime completedAt)
    {
        SaleId = saleId;
        TotalAmount = totalAmount;
        CompletedAt = completedAt;
    }
} 
