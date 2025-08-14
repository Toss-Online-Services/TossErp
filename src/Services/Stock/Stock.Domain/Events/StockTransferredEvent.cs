using TossErp.Stock.Domain.SeedWork;
using TossErp.Stock.Domain.ValueObjects;

namespace TossErp.Stock.Domain.Events;

public class StockTransferredEvent : DomainEvent
{
    public Guid ItemId { get; }
    public Guid FromWarehouseId { get; }
    public Guid ToWarehouseId { get; }
    public Quantity Qty { get; }

    public StockTransferredEvent(Guid itemId, Guid fromWarehouseId, Guid toWarehouseId, Quantity qty)
    {
        ItemId = itemId;
        FromWarehouseId = fromWarehouseId;
        ToWarehouseId = toWarehouseId;
        Qty = qty;
    }
}
