namespace LLSFramework.TabBlazor.Components.Loading;

public class LoadingState
{
    public bool IsLoading { get; private set; }

    public void Start()
    {
        IsLoading = true;
        OnChanged?.Invoke();
    }

    public void Stop()
    {
        IsLoading = false;
        OnChanged?.Invoke();
    }

    public event Action? OnChanged;
}