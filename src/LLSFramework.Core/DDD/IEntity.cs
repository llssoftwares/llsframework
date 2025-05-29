namespace LLSFramework.Core.DDD;

/// <summary>
/// Defines a generic contract for an entity with a unique identifier.
/// Inherits audit properties from <see cref="IEntity"/>.
/// </summary>
/// <typeparam name="T">The type of the entity's unique identifier.</typeparam>
public interface IEntity<T> : IEntity
{
    /// <summary>
    /// Gets or sets the unique identifier for the entity.
    /// </summary>
    public T Id { get; set; }
}

/// <summary>
/// Defines a contract for an auditable entity.
/// Provides properties for creation and modification tracking.
/// </summary>
public interface IEntity
{
    /// <summary>
    /// Gets the UTC timestamp when the entity was created.
    /// </summary>
    public DateTime CreatedAt { get; }

    /// <summary>
    /// Gets or sets the user who created the entity.
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// Gets or sets the UTC timestamp of the last modification, if any.
    /// </summary>
    public DateTime? LastModified { get; set; }

    /// <summary>
    /// Gets or sets the user who last modified the entity, if any.
    /// </summary>
    public string? LastModifiedBy { get; set; }
}