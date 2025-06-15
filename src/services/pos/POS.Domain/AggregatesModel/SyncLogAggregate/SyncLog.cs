using System;
using TossErp.POS.Domain.SeedWork;

namespace TossErp.POS.Domain.AggregatesModel.SyncLogAggregate;

public class SyncLog : AggregateRoot
{
    public string EntityType { get; private set; }
    public int EntityId { get; private set; }
    public string Operation { get; private set; }
    public DateTime SyncTime { get; private set; }
    public bool IsSuccess { get; private set; }
    public string? ErrorMessage { get; private set; }

    protected SyncLog()
    {
        EntityType = string.Empty;
        Operation = string.Empty;
    }

    public SyncLog(string entityType, int entityId, string operation, bool isSuccess, string? errorMessage = null)
    {
        if (string.IsNullOrWhiteSpace(entityType))
            throw new DomainException("Entity type cannot be empty");
        if (string.IsNullOrWhiteSpace(operation))
            throw new DomainException("Operation cannot be empty");

        EntityType = entityType;
        EntityId = entityId;
        Operation = operation;
        IsSuccess = isSuccess;
        ErrorMessage = errorMessage;
        SyncTime = DateTime.UtcNow;
    }
}

public enum SyncStatus
{
    Pending,
    InProgress,
    Completed,
    Failed
} 
