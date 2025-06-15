using POS.Domain.SeedWork;

namespace POS.Domain.AggregatesModel.SyncAggregate;

public class SyncLog : Entity
{
    public Guid StoreId { get; private set; }
    public string EntityType { get; private set; }
    public Guid EntityId { get; private set; }
    public string Action { get; private set; }
    public string Status { get; private set; }
    public string? ErrorMessage { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    protected SyncLog() { }

    public SyncLog(Guid storeId, string entityType, Guid entityId, string action)
    {
        if (storeId == Guid.Empty)
            throw new ArgumentException("Store ID cannot be empty", nameof(storeId));

        if (string.IsNullOrWhiteSpace(entityType))
            throw new ArgumentException("Entity type cannot be empty", nameof(entityType));

        if (entityId == Guid.Empty)
            throw new ArgumentException("Entity ID cannot be empty", nameof(entityId));

        if (string.IsNullOrWhiteSpace(action))
            throw new ArgumentException("Action cannot be empty", nameof(action));

        StoreId = storeId;
        EntityType = entityType;
        EntityId = entityId;
        Action = action;
        Status = "Pending";
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetStatus(string status, string? errorMessage = null)
    {
        if (string.IsNullOrWhiteSpace(status))
            throw new ArgumentException("Status cannot be empty", nameof(status));

        Status = status;
        ErrorMessage = errorMessage;
        UpdatedAt = DateTime.UtcNow;
    }
} 
