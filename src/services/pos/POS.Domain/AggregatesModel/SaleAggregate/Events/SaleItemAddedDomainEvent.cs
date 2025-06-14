#nullable enable
using TossErp.POS.Domain.Events;

namespace TossErp.POS.Domain.AggregatesModel.SaleAggregate.Events;

public class SaleItemAddedDomainEvent : DomainEvent
{
    public int SaleId { get; }
    public int ProductId { get; }
    public int Quantity { get; }
    public decimal UnitPrice { get; }

    public SaleItemAddedDomainEvent(int saleId, int productId, int quantity, decimal unitPrice)
    {
        SaleId = saleId;
        ProductId = productId;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }
} 
