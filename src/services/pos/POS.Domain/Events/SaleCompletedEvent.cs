using POS.Domain.SeedWork;

namespace POS.Domain.Events;

public class SaleCompletedEvent : DomainEvent
{
    public Guid SaleId { get; }
    public decimal TotalAmount { get; }
    public string Currency { get; }
    public DateTime CompletedAt { get; }

    public SaleCompletedEvent(Guid saleId, decimal totalAmount, string currency)
    {
        SaleId = saleId;
        TotalAmount = totalAmount;
        Currency = currency;
        CompletedAt = DateTime.UtcNow;
    }
} 
