namespace TossErp.Accounting.Domain.Common;

/// <summary>
/// Base entity class with common properties
/// </summary>
public abstract class Entity<T> : IEquatable<Entity<T>> where T : notnull
{
    public T Id { get; protected set; } = default!;
    public DateTime CreatedAt { get; protected set; }
    public DateTime? UpdatedAt { get; protected set; }
    public string CreatedBy { get; protected set; } = string.Empty;
    public string? UpdatedBy { get; protected set; }
    public string TenantId { get; protected set; } = string.Empty;

    protected Entity()
    {
        CreatedAt = DateTime.UtcNow;
    }

    protected Entity(T id) : this()
    {
        Id = id;
    }

    protected Entity(T id, string tenantId) : this(id)
    {
        TenantId = tenantId ?? throw new ArgumentNullException(nameof(tenantId));
    }

    public bool Equals(Entity<T>? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return EqualityComparer<T>.Default.Equals(Id, other.Id);
    }

    public override bool Equals(object? obj)
    {
        return obj is Entity<T> other && Equals(other);
    }

    public override int GetHashCode()
    {
        return EqualityComparer<T>.Default.GetHashCode(Id);
    }

    public static bool operator ==(Entity<T>? left, Entity<T>? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(Entity<T>? left, Entity<T>? right)
    {
        return !Equals(left, right);
    }

    protected void MarkAsUpdated(string updatedBy)
    {
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }
}

