namespace LLSFramework.TabBlazor.Services;

public class BlazorMediator
{
    private readonly Dictionary<Type, List<Delegate>> _subscriptions = [];

    public void Subscribe<TNotification>(Func<TNotification, CancellationToken, Task> handler)
        where TNotification : INotification
    {
        if (!_subscriptions.ContainsKey(typeof(TNotification)))
            _subscriptions[typeof(TNotification)] = [];

        _subscriptions[typeof(TNotification)].Add(handler);
    }

    public void Subscribe<TNotification>(Action<TNotification> handler)
        where TNotification : INotification
    {
        if (!_subscriptions.ContainsKey(typeof(TNotification)))
            _subscriptions[typeof(TNotification)] = [];

        _subscriptions[typeof(TNotification)].Add(handler);
    }

    public void Unsubscribe<TNotification>(Func<TNotification, CancellationToken, Task> handler)
        where TNotification : INotification
    {
        if (!_subscriptions.ContainsKey(typeof(TNotification))) return;

        _subscriptions[typeof(TNotification)].Remove(handler);

        if (_subscriptions[typeof(TNotification)].Count == 0)
            _subscriptions.Remove(typeof(TNotification));
    }

    public void Unsubscribe<TNotification>(Action<TNotification> handler)
        where TNotification : INotification
    {
        if (!_subscriptions.ContainsKey(typeof(TNotification))) return;

        _subscriptions[typeof(TNotification)].Remove(handler);

        if (_subscriptions[typeof(TNotification)].Count == 0)
            _subscriptions.Remove(typeof(TNotification));
    }

    public async Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
        where TNotification : INotification
    {
        if (!_subscriptions.ContainsKey(typeof(TNotification))) return;

        foreach (var asyncHandler in _subscriptions[typeof(TNotification)].OfType<Func<TNotification, CancellationToken, Task>>())
            await asyncHandler(notification, cancellationToken);

        foreach (var syncHandler in _subscriptions[typeof(TNotification)].OfType<Action<TNotification>>())
            syncHandler(notification);
    }
}