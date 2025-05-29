namespace LLSFramework.Core.DDD;

/// <summary>
/// Represents a base entity with an identifier and audit information.
/// </summary>
/// <typeparam name="T">The type of the entity's unique identifier.</typeparam>
public abstract class Entity<T> : IEntity<T>
{
    /// <summary>
    /// Gets or sets the unique identifier for the entity.
    /// </summary>
    public required T Id { get; set; }

    /// <summary>
    /// Gets the creation timestamp of the entity in UTC.
    /// </summary>
    public DateTime CreatedAt => DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the user who created the entity.
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// Gets or sets the timestamp of the last modification, if any.
    /// </summary>
    public DateTime? LastModified { get; set; }

    /// <summary>
    /// Gets or sets the user who last modified the entity, if any.
    /// </summary>
    public string? LastModifiedBy { get; set; }

    /// <summary>
    /// Determines whether the specified object is equal to the current entity.
    /// </summary>
    /// <param name="obj">The object to compare with the current entity.</param>
    /// <returns><c>true</c> if the specified object is equal to the current entity; otherwise, <c>false</c>.</returns>
    public override bool Equals(object? obj)
    {
        if (obj is not Entity<T> other)
            return false;

        if (ReferenceEquals(this, other))
            return true;

        // If either Id is default, consider not equal
        return !EqualityComparer<T>.Default.Equals(Id, default!)
            && !EqualityComparer<T>.Default.Equals(other.Id, default!)
            && EqualityComparer<T>.Default.Equals(Id, other.Id);
    }

    /// <summary>
    /// Determines whether the specified entity is equal to the current entity.
    /// </summary>
    /// <param name="other">The entity to compare with the current entity.</param>
    /// <returns><c>true</c> if the specified entity is equal to the current entity; otherwise, <c>false</c>.</returns>
    public bool Equals(Entity<T>? other)
    {
        if (other is null)
            return false;

        if (ReferenceEquals(this, other))
            return true;

        return !EqualityComparer<T>.Default.Equals(Id, default!)
            && !EqualityComparer<T>.Default.Equals(other.Id, default!)
            && EqualityComparer<T>.Default.Equals(Id, other.Id);
    }

    /// <summary>
    /// Returns a hash code for the current entity.
    /// </summary>
    /// <returns>A hash code for the current entity.</returns>
    public override int GetHashCode()
    {
        return Id is null ? 0 : EqualityComparer<T>.Default.GetHashCode(Id);
    }

    /// <summary>
    /// Determines whether two entities are equal.
    /// </summary>
    /// <param name="left">The first entity to compare.</param>
    /// <param name="right">The second entity to compare.</param>
    /// <returns><c>true</c> if the entities are equal; otherwise, <c>false</c>.</returns>
    public static bool operator ==(Entity<T>? left, Entity<T>? right)
    {
        return left is null && right is null || left is not null && right is not null && left.Equals(right);
    }

    /// <summary>
    /// Determines whether two entities are not equal.
    /// </summary>
    /// <param name="left">The first entity to compare.</param>
    /// <param name="right">The second entity to compare.</param>
    /// <returns><c>true</c> if the entities are not equal; otherwise, <c>false</c>.</returns>
    public static bool operator !=(Entity<T>? left, Entity<T>? right)
    {
        return !(left == right);
    }
}
