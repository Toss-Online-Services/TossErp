using POS.Domain.AggregatesModel.StoreAggregate;
using POS.Domain.Exceptions;
using POS.Domain.SeedWork;

namespace POS.Domain.AggregatesModel.SyncAggregate;

public class SyncLog : AggregateRoot
{
    public Guid StoreId { get; private set; }
    public Store Store { get; private set; } = null!;
    public string EntityType { get; private set; }
    public string EntityId { get; private set; }
    public string Action { get; private set; }
    public string Status { get; private set; }
    public string? ErrorMessage { get; private set; }
    public int RetryCount { get; private set; }
    public DateTime? LastRetryAt { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    protected SyncLog()
    {
        EntityType = string.Empty;
        EntityId = string.Empty;
        Action = string.Empty;
        Status = "Pending";
        CreatedAt = DateTime.UtcNow;
    }

    public SyncLog(Guid storeId, string entityType, string entityId, string action)
    {
        if (storeId == Guid.Empty)
            throw new DomainException("Store ID cannot be empty");
        if (string.IsNullOrWhiteSpace(entityType))
            throw new DomainException("Entity type cannot be empty");
        if (string.IsNullOrWhiteSpace(entityId))
            throw new DomainException("Entity ID cannot be empty");
        if (string.IsNullOrWhiteSpace(action))
            throw new DomainException("Action cannot be empty");

        StoreId = storeId;
        EntityType = entityType;
        EntityId = entityId;
        Action = action;
        Status = "Pending";
        RetryCount = 0;
        CreatedAt = DateTime.UtcNow;
    }

    public void SetStatus(string status, string? errorMessage = null)
    {
        if (string.IsNullOrWhiteSpace(status))
            throw new DomainException("Status cannot be empty");

        Status = status;
        ErrorMessage = errorMessage;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsProcessed()
    {
        if (Status == "Processed")
            throw new DomainException("Sync log is already processed");

        Status = "Processed";
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsFailed(string errorMessage)
    {
        if (string.IsNullOrWhiteSpace(errorMessage))
            throw new DomainException("Error message cannot be empty");

        Status = "Failed";
        ErrorMessage = errorMessage;
        UpdatedAt = DateTime.UtcNow;
    }

    public void IncrementRetryCount()
    {
        RetryCount++;
        LastRetryAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void ResetRetryCount()
    {
        RetryCount = 0;
        LastRetryAt = null;
        UpdatedAt = DateTime.UtcNow;
    }

    public bool CanRetry(int maxRetries)
    {
        return Status == "Failed" && RetryCount < maxRetries;
    }
} 
