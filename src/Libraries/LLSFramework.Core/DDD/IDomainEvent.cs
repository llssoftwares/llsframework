namespace LLSFramework.Core.DDD;

/// <summary>
/// Represents a domain event in Domain-Driven Design (DDD).
/// Domain events capture and communicate important changes or occurrences within the domain.
/// </summary>
public interface IDomainEvent
{
    /// <summary>
    /// Gets the unique identifier for this domain event instance.
    /// </summary>
    Guid EventId => Guid.NewGuid();

    /// <summary>
    /// Gets the UTC timestamp indicating when the event occurred.
    /// </summary>
    public DateTime OccurredOn => DateTime.UtcNow;

    /// <summary>
    /// Gets the fully qualified type name of the event.
    /// Useful for event handling and serialization.
    /// </summary>
    public string EventType => GetType().AssemblyQualifiedName!;
}