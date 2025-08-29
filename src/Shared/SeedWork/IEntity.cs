namespace TossErp.Shared.SeedWork;

/// <summary>
/// Base interface for all entities
/// </summary>
public interface IEntity
{
    Guid Id { get; }
}

/// <summary>
/// Base interface for entities with creation tracking
/// </summary>
public interface IEntity<T> : IEntity
{
    new T Id { get; }
    DateTime CreatedAt { get; }
    string CreatedBy { get; }
}

/// <summary>
/// Base interface for auditable entities
/// </summary>
public interface IAuditableEntity : IEntity<Guid>
{
    DateTime? UpdatedAt { get; }
    string? UpdatedBy { get; }
}

/// <summary>
/// Base interface for entities that can be soft deleted
/// </summary>
public interface ISoftDeletableEntity : IAuditableEntity
{
    bool IsDeleted { get; }
    DateTime? DeletedAt { get; }
    string? DeletedBy { get; }
}

/// <summary>
/// Base interface for tenant-aware entities
/// </summary>
public interface ITenantEntity : IAuditableEntity
{
    Guid TenantId { get; }
}
