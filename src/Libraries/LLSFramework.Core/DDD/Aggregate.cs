namespace LLSFramework.Core.DDD;

/// <summary>
/// Represents an aggregate root in Domain-Driven Design (DDD).
/// Aggregates encapsulate domain entities and manage domain events.
/// </summary>
/// <typeparam name="T">The type of the aggregate's unique identifier.</typeparam>
public abstract class Aggregate<T> : Entity<T>, IAggregate<T>
{
    // Stores domain events raised by the aggregate.
    private readonly List<IDomainEvent> _domainEvents = [];

    /// <summary>
    /// Gets a read-only collection of domain events raised by this aggregate.
    /// </summary>
    public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    /// <summary>
    /// Adds a domain event to the aggregate's event collection.
    /// </summary>
    /// <param name="domainEvent">The domain event to add.</param>
    public void AddDomainEvents(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    /// <summary>
    /// Clears all domain events from the aggregate and returns them.
    /// Typically called after events have been dispatched.
    /// </summary>
    /// <returns>An array of the domain events that were cleared.</returns>
    public IDomainEvent[] ClearDomainEvents()
    {
        var dequeueDomainEvents = _domainEvents.ToArray();

        _domainEvents.Clear();

        return dequeueDomainEvents;
    }
}