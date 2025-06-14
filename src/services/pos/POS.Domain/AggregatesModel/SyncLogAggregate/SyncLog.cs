using System;
using TossErp.POS.Domain.Common;

namespace TossErp.POS.Domain.AggregatesModel.SyncLogAggregate;

public class SyncLog : Entity, IAggregateRoot
{
    public string EntityType { get; private set; }
    public int EntityId { get; private set; }
    public string Operation { get; private set; }
    public string Status { get; private set; }
    public string? ErrorMessage { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    protected SyncLog()
    {
        EntityType = string.Empty;
        Operation = string.Empty;
        Status = string.Empty;
        CreatedAt = DateTime.UtcNow;
    }

    public SyncLog(string entityType, int entityId, string operation, string status)
    {
        if (string.IsNullOrWhiteSpace(entityType))
            throw new DomainException("Entity type cannot be empty");

        if (string.IsNullOrWhiteSpace(operation))
            throw new DomainException("Operation cannot be empty");

        if (string.IsNullOrWhiteSpace(status))
            throw new DomainException("Status cannot be empty");

        EntityType = entityType;
        EntityId = entityId;
        Operation = operation;
        Status = status;
        CreatedAt = DateTime.UtcNow;
    }

    public void UpdateStatus(string status, string? errorMessage = null)
    {
        if (string.IsNullOrWhiteSpace(status))
            throw new DomainException("Status cannot be empty");

        Status = status;
        ErrorMessage = errorMessage;
        UpdatedAt = DateTime.UtcNow;
    }
}

public enum SyncStatus
{
    Pending,
    InProgress,
    Completed,
    Failed
} 
