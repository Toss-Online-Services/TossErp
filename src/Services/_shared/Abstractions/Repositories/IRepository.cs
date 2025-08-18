using TossErp.Abstractions.Common;

namespace TossErp.Abstractions.Repositories;

/// <summary>
/// Generic repository interface for basic CRUD operations
/// </summary>
/// <typeparam name="TEntity">The entity type</typeparam>
/// <typeparam name="TId">The identifier type</typeparam>
public interface IRepository<TEntity, in TId> 
    where TEntity : class, IEntity<TId>
{
    /// <summary>
    /// Get an entity by its identifier
    /// </summary>
    Task<TEntity?> GetByIdAsync(TId id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Add a new entity
    /// </summary>
    Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update an existing entity
    /// </summary>
    Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete an entity
    /// </summary>
    Task DeleteAsync(TId id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if an entity exists
    /// </summary>
    Task<bool> ExistsAsync(TId id, CancellationToken cancellationToken = default);
}

/// <summary>
/// Repository interface for entities with Guid identifiers
/// </summary>
/// <typeparam name="TEntity">The entity type</typeparam>
public interface IRepository<TEntity> : IRepository<TEntity, Guid> 
    where TEntity : class, IEntity
{
}

/// <summary>
/// Repository interface for tenant-specific entities
/// </summary>
/// <typeparam name="TEntity">The entity type</typeparam>
public interface ITenantRepository<TEntity> : IRepository<TEntity>
    where TEntity : class, ITenantEntity
{
    /// <summary>
    /// Get all entities for a specific tenant
    /// </summary>
    Task<IReadOnlyList<TEntity>> GetByTenantAsync(string tenantId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get an entity by ID within a tenant scope
    /// </summary>
    Task<TEntity?> GetByIdAndTenantAsync(Guid id, string tenantId, CancellationToken cancellationToken = default);
}
