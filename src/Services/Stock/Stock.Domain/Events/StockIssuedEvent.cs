using TossErp.Stock.Domain.SeedWork;
using TossErp.Stock.Domain.ValueObjects;

namespace TossErp.Stock.Domain.Events;

public class StockIssuedEvent : DomainEvent
{
    public Guid ItemId { get; }
    public Guid WarehouseId { get; }
    public Quantity Qty { get; }
    public string? RefType { get; }
    public string? RefId { get; }

    public StockIssuedEvent(Guid itemId, Guid warehouseId, Quantity qty, string? refType = null, string? refId = null)
    {
        ItemId = itemId;
        WarehouseId = warehouseId;
        Qty = qty;
        RefType = refType;
        RefId = refId;
    }
}
