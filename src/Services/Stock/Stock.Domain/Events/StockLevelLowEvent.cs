using TossErp.Stock.Domain.SeedWork;
using TossErp.Stock.Domain.ValueObjects;

namespace TossErp.Stock.Domain.Events;

public class StockLevelLowEvent : DomainEvent
{
    public Guid ItemId { get; }
    public Guid WarehouseId { get; }
    public Quantity CurrentQty { get; }
    public Quantity ReorderLevel { get; }

    public StockLevelLowEvent(Guid itemId, Guid warehouseId, Quantity currentQty, Quantity reorderLevel)
    {
        ItemId = itemId;
        WarehouseId = warehouseId;
        CurrentQty = currentQty;
        ReorderLevel = reorderLevel;
    }
}
