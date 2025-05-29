namespace LLSFramework.TabBlazor.Components.Modals;

public class LookupModalResult(List<LookupItemViewModelBase> selected, bool cancelled)
{
    public List<LookupItemViewModelBase> Selected { get; } = selected;

    public bool Cancelled { get; } = cancelled;
}