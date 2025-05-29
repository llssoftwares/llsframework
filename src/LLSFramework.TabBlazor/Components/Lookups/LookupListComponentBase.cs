namespace LLSFramework.TabBlazor.Components.Lookups;

public abstract class LookupListComponentBase : ComponentBase
{
    [Inject] protected IModalService ModalService { get; set; } = default!;

    [Parameter] public List<LookupItemViewModelBase> SelectedItems { get; set; } = [];

    [Parameter] public EventCallback<List<LookupItemViewModelBase>> SelectedItemsChanged { get; set; }

    [Parameter] public bool DisplaySelectedItemsCount { get; set; }

    [Parameter] public bool SearchOnInitialized { get; set; }

    protected async Task SearchAsync()
    {
        var result = await SearchActionAsync();

        if (!result.Cancelled)
            await SelectedItemsChanged.InvokeAsync(result.Selected);
    }

    protected abstract Task<LookupModalResult> SearchActionAsync();

    protected async Task ClearAsync()
    {
        await SelectedItemsChanged.InvokeAsync([]);
    }

    protected string GetFormattedText(string selectedSingular = "selecionado", string selectedPlural = "selecionados")
    {
        if (!DisplaySelectedItemsCount)
            return string.Join(", ", SelectedItems.Select(x => x?.Text));
        else if (SelectedItems?.Count > 0)
            return SelectedItems.Count > 1 ? $"{SelectedItems.Count} {selectedPlural}" : $"1 {selectedSingular}";

        return string.Empty;
    }
}