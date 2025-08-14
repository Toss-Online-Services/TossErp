using TossErp.Stock.Domain.SeedWork;
using TossErp.Stock.Domain.ValueObjects;

namespace TossErp.Stock.Domain.Events;

public class StockAdjustedEvent : DomainEvent
{
    public Guid ItemId { get; }
    public Guid WarehouseId { get; }
    public Quantity OldQty { get; }
    public Quantity NewQty { get; }
    public string Reason { get; }

    public StockAdjustedEvent(Guid itemId, Guid warehouseId, Quantity oldQty, Quantity newQty, string reason)
    {
        ItemId = itemId;
        WarehouseId = warehouseId;
        OldQty = oldQty;
        NewQty = newQty;
        Reason = reason;
    }
}
