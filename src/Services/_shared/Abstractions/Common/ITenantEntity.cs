namespace TossErp.Abstractions.Common;

/// <summary>
/// Interface for entities that belong to a tenant
/// </summary>
public interface ITenantEntity : IEntity
{
    /// <summary>
    /// The tenant identifier
    /// </summary>
    string TenantId { get; }
}
