namespace LLSFramework.Core.DDD;

/// <summary>
/// Defines a generic aggregate root contract for domain-driven design.
/// Inherits from <see cref="IEntity{T}"/> and <see cref="IAggregate"/>.
/// </summary>
/// <typeparam name="T">The type of the aggregate's unique identifier.</typeparam>
public interface IAggregate<T> : IEntity<T>, IAggregate;

/// <summary>
/// Represents a non-generic aggregate root contract.
/// Provides access to domain events and event management for aggregates.
/// </summary>
public interface IAggregate : IEntity
{
    /// <summary>
    /// Gets a read-only list of domain events raised by the aggregate.
    /// </summary>
    IReadOnlyList<IDomainEvent> DomainEvents { get; }

    /// <summary>
    /// Clears all domain events from the aggregate and returns them.
    /// Typically called after events have been dispatched.
    /// </summary>
    /// <returns>An array of the domain events that were cleared.</returns>
    IDomainEvent[] ClearDomainEvents();
}
