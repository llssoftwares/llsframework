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
}