#nullable enable
using eShop.POS.Domain.SeedWork;

namespace eShop.POS.Domain.AggregatesModel.SyncLogAggregate;

public class SyncLog : Entity, IAggregateRoot
{
    public required string StoreId { get; set; }
    public DateTime SyncDate { get; set; }
    public SyncStatus Status { get; set; }
    public required string ErrorMessage { get; set; }
}

public enum SyncStatus
{
    Pending,
    InProgress,
    Completed,
    Failed
} 
