#nullable enable

using POS.Domain.Common.Events;

namespace POS.Domain.AggregatesModel.SaleAggregate.Events;

public class SaleSyncedDomainEvent : IDomainEvent
{
    public Guid SaleId { get; }
    public string SaleNumber { get; }
    public Guid StoreId { get; }
    public DateTime SyncedAt { get; }
    public bool Success { get; }
    public string? ErrorMessage { get; }

    public SaleSyncedDomainEvent(
        Guid saleId,
        string saleNumber,
        Guid storeId,
        DateTime syncedAt,
        bool success,
        string? errorMessage = null)
    {
        SaleId = saleId;
        SaleNumber = saleNumber;
        StoreId = storeId;
        SyncedAt = syncedAt;
        Success = success;
        ErrorMessage = errorMessage;
    }
} 
