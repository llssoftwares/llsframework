namespace LLSFramework.TabBlazor.Components.Loading;

/// <summary>
/// Represents a simple loading state manager for UI components.
/// Notifies subscribers when the loading state changes.
/// </summary>
public class LoadingState
{
    /// <summary>
    /// Gets a value indicating whether a loading operation is in progress.
    /// </summary>
    public bool IsLoading { get; private set; }

    /// <summary>
    /// Sets the loading state to active and notifies subscribers.
    /// </summary>
    public void Start()
    {
        IsLoading = true;
        OnChanged?.Invoke();
    }

    /// <summary>
    /// Sets the loading state to inactive and notifies subscribers.
    /// </summary>
    public void Stop()
    {
        IsLoading = false;
        OnChanged?.Invoke();
    }

    /// <summary>
    /// Event triggered whenever the loading state changes.
    /// Subscribers can use this to update UI or trigger other actions.
    /// </summary>
    public event Action? OnChanged;
}