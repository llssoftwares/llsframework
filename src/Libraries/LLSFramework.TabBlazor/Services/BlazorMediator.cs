namespace LLSFramework.TabBlazor.Services;

/// <summary>
/// Implements a simple mediator pattern for publishing and subscribing to notifications within a Blazor application.
/// Allows components and services to communicate via strongly-typed notifications without direct dependencies.
/// </summary>
public class BlazorMediator
{
    // Stores notification type to list of handler delegates (both sync and async).
    private readonly Dictionary<Type, List<Delegate>> _subscriptions = [];

    /// <summary>
    /// Subscribes an asynchronous handler to notifications of type <typeparamref name="TNotification"/>.
    /// </summary>
    /// <typeparam name="TNotification">The notification type to subscribe to.</typeparam>
    /// <param name="handler">The async handler to invoke when the notification is published.</param>
    public void Subscribe<TNotification>(Func<TNotification, CancellationToken, Task> handler)
        where TNotification : INotification
    {
        if (!_subscriptions.ContainsKey(typeof(TNotification)))
            _subscriptions[typeof(TNotification)] = [];

        _subscriptions[typeof(TNotification)].Add(handler);
    }

    /// <summary>
    /// Subscribes a synchronous handler to notifications of type <typeparamref name="TNotification"/>.
    /// </summary>
    /// <typeparam name="TNotification">The notification type to subscribe to.</typeparam>
    /// <param name="handler">The sync handler to invoke when the notification is published.</param>
    public void Subscribe<TNotification>(Action<TNotification> handler)
        where TNotification : INotification
    {
        if (!_subscriptions.ContainsKey(typeof(TNotification)))
            _subscriptions[typeof(TNotification)] = [];

        _subscriptions[typeof(TNotification)].Add(handler);
    }

    /// <summary>
    /// Unsubscribes an asynchronous handler from notifications of type <typeparamref name="TNotification"/>.
    /// </summary>
    /// <typeparam name="TNotification">The notification type to unsubscribe from.</typeparam>
    /// <param name="handler">The async handler to remove.</param>
    public void Unsubscribe<TNotification>(Func<TNotification, CancellationToken, Task> handler)
        where TNotification : INotification
    {
        if (!_subscriptions.ContainsKey(typeof(TNotification))) return;

        _subscriptions[typeof(TNotification)].Remove(handler);

        if (_subscriptions[typeof(TNotification)].Count == 0)
            _subscriptions.Remove(typeof(TNotification));
    }

    /// <summary>
    /// Unsubscribes a synchronous handler from notifications of type <typeparamref name="TNotification"/>.
    /// </summary>
    /// <typeparam name="TNotification">The notification type to unsubscribe from.</typeparam>
    /// <param name="handler">The sync handler to remove.</param>
    public void Unsubscribe<TNotification>(Action<TNotification> handler)
        where TNotification : INotification
    {
        if (!_subscriptions.ContainsKey(typeof(TNotification))) return;

        _subscriptions[typeof(TNotification)].Remove(handler);

        if (_subscriptions[typeof(TNotification)].Count == 0)
            _subscriptions.Remove(typeof(TNotification));
    }

    /// <summary>
    /// Publishes a notification to all subscribed handlers (both async and sync) for the notification type.
    /// </summary>
    /// <typeparam name="TNotification">The notification type to publish.</typeparam>
    /// <param name="notification">The notification instance to publish.</param>
    /// <param name="cancellationToken">A cancellation token for async handlers (optional).</param>
    public async Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
        where TNotification : INotification
    {
        if (!_subscriptions.ContainsKey(typeof(TNotification))) return;

        // Invoke all async handlers for this notification type.
        foreach (var asyncHandler in _subscriptions[typeof(TNotification)].OfType<Func<TNotification, CancellationToken, Task>>())
            await asyncHandler(notification, cancellationToken);

        // Invoke all sync handlers for this notification type.
        foreach (var syncHandler in _subscriptions[typeof(TNotification)].OfType<Action<TNotification>>())
            syncHandler(notification);
    }
}