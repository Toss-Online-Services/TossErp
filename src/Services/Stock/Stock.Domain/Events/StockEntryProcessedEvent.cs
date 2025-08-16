using TossErp.Stock.Domain.Aggregates.StockEntryAggregate;
using TossErp.Stock.Domain.Common;

namespace TossErp.Stock.Domain.Events;

/// <summary>
/// Event raised when a stock entry is processed
/// </summary>
public class StockEntryProcessedEvent : IDomainEvent
{
    public StockEntryAggregate StockEntry { get; }

    public StockEntryProcessedEvent(StockEntryAggregate stockEntry)
    {
        StockEntry = stockEntry;
    }
}
