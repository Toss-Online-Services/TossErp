namespace TossErp.Abstractions.Common;

/// <summary>
/// Base interface for all entities with identity
/// </summary>
/// <typeparam name="TId">The type of the entity identifier</typeparam>
public interface IEntity<out TId>
{
    /// <summary>
    /// The unique identifier of the entity
    /// </summary>
    TId Id { get; }
}

/// <summary>
/// Base interface for entities with Guid identifiers
/// </summary>
public interface IEntity : IEntity<Guid>
{
}
