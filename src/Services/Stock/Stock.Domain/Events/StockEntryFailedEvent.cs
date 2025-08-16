using TossErp.Stock.Domain.Aggregates.StockEntryAggregate;
using TossErp.Stock.Domain.Common;

namespace TossErp.Stock.Domain.Events;

/// <summary>
/// Event raised when a stock entry processing fails
/// </summary>
public class StockEntryFailedEvent : IDomainEvent
{
    public StockEntryAggregate StockEntry { get; }
    public string ErrorMessage { get; }

    public StockEntryFailedEvent(StockEntryAggregate stockEntry, string errorMessage)
    {
        StockEntry = stockEntry;
        ErrorMessage = errorMessage;
    }
}
