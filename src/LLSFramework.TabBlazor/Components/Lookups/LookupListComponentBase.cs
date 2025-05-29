namespace LLSFramework.TabBlazor.Components.Lookups;

/// <summary>
/// Abstract base class for lookup list components that support multi-selection,
/// modal-based searching, and formatted display of selected items.
/// </summary>
public abstract class LookupListComponentBase : ComponentBase
{
    /// <summary>
    /// Service for displaying modals, injected by the framework.
    /// </summary>
    [Inject] protected IModalService ModalService { get; set; } = default!;

    /// <summary>
    /// The list of currently selected lookup items.
    /// </summary>
    [Parameter] public List<LookupItemViewModelBase> SelectedItems { get; set; } = [];

    /// <summary>
    /// Event callback triggered when the selected items list changes.
    /// </summary>
    [Parameter] public EventCallback<List<LookupItemViewModelBase>> SelectedItemsChanged { get; set; }

    /// <summary>
    /// If true, displays the count of selected items instead of their text.
    /// </summary>
    [Parameter] public bool DisplaySelectedItemsCount { get; set; }

    /// <summary>
    /// If true, performs a search when the component is initialized.
    /// </summary>
    [Parameter] public bool SearchOnInitialized { get; set; }

    /// <summary>
    /// Opens the search modal and updates the selected items if the user confirms a selection.
    /// </summary>
    protected async Task SearchAsync()
    {
        var result = await SearchActionAsync();

        if (!result.Cancelled)
            await SelectedItemsChanged.InvokeAsync(result.Selected);
    }

    /// <summary>
    /// Abstract method to be implemented by derived classes to perform the search action.
    /// Should return a <see cref="LookupModalResult"/> containing the user's selection.
    /// </summary>
    /// <returns>A task that returns the modal result.</returns>
    protected abstract Task<LookupModalResult> SearchActionAsync();

    /// <summary>
    /// Clears the current selection and notifies parent components.
    /// </summary>
    protected async Task ClearAsync()
    {
        await SelectedItemsChanged.InvokeAsync([]);
    }

    /// <summary>
    /// Returns a formatted string representing the selected items.
    /// If <see cref="DisplaySelectedItemsCount"/> is true, returns the count with singular/plural text.
    /// Otherwise, returns a comma-separated list of selected item texts.
    /// </summary>
    /// <param name="selectedSingular">Text to use for a single selected item (default: "selecionado").</param>
    /// <param name="selectedPlural">Text to use for multiple selected items (default: "selecionados").</param>
    /// <returns>A formatted string for display.</returns>
    protected string GetFormattedText(string selectedSingular = "selecionado", string selectedPlural = "selecionados")
    {
        if (!DisplaySelectedItemsCount)
            return string.Join(", ", SelectedItems.Select(x => x?.Text));
        else if (SelectedItems?.Count > 0)
            return SelectedItems.Count > 1 ? $"{SelectedItems.Count} {selectedPlural}" : $"1 {selectedSingular}";

        return string.Empty;
    }
}