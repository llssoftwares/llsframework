namespace LLSFramework.TabBlazor.Components.Lookups;

/// <summary>
/// Abstract base class for modal lookup components that support filtering, pagination, and item selection.
/// Handles table state, filter management, and single-item selection logic for modal dialogs.
/// </summary>
/// <typeparam name="TFilter">The filter type, must inherit from <see cref="EntityFilter"/>.</typeparam>
/// <typeparam name="TItem">The item type, must inherit from <see cref="LookupItemViewModelBase"/>.</typeparam>
public abstract class LookupModalComponentBase<TFilter, TItem>
    : ComponentBase where TFilter
    : EntityFilter, new() where TItem : LookupItemViewModelBase
{
    /// <summary>
    /// Service for displaying and closing modals, injected by the framework.
    /// </summary>
    [Inject] protected IModalService ModalService { get; set; } = default!;

    /// <summary>
    /// The display text for the selected item.
    /// </summary>
    [Parameter] public string Text { get; set; } = string.Empty;

    /// <summary>
    /// If true, performs a search when the modal is initialized.
    /// </summary>
    [Parameter]
    public bool SearchOnInitialized { get; set; }

    /// <summary>
    /// The table component instance for managing pagination, sorting, and refresh.
    /// </summary>
    protected LLSTable<TItem> Table = new();

    /// <summary>
    /// The current paginated result set for the table.
    /// </summary>
    protected PaginatedResult<TItem> PaginatedResult = new();

    /// <summary>
    /// The current filter used for searching and pagination.
    /// </summary>
    protected TFilter Filter = new();

    /// <summary>
    /// Called when the component is initialized; sets up the filter.
    /// </summary>
    protected override void OnInitialized()
    {
        SetFilter();
    }

    /// <summary>
    /// After first render, triggers a table refresh if <see cref="SearchOnInitialized"/> is true.
    /// </summary>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender && SearchOnInitialized)
            await Table.RefreshAsync();
    }

    /// <summary>
    /// Allows derived classes to customize the filter when the component is initialized.
    /// Must be implemented by derived classes.
    /// </summary>
    protected abstract void SetFilter();

    /// <summary>
    /// Abstract method to fetch paginated results based on the current filter.
    /// Must be implemented by derived classes.
    /// </summary>
    /// <returns>A paginated result set of items.</returns>
    protected abstract Task<PaginatedResult<TItem>> FetchAsync();

    /// <summary>
    /// Initiates a search by navigating the table to the first page.
    /// </summary>
    protected async Task SearchAsync()
    {
        await Table.GoToPageAsync(1);
    }

    /// <summary>
    /// Handles table state changes (pagination, sorting, etc.), updates the filter,
    /// and fetches new data.
    /// </summary>
    /// <param name="e">The table change event arguments.</param>
    protected async Task HandleTableChangedAsync(TableChangedEventArgs e)
    {
        Filter.SetOptions(e);

        PaginatedResult = await FetchAsync();
    }

    /// <summary>
    /// Selects an item and closes the modal, returning the selected item.
    /// </summary>
    /// <param name="item">The item to select.</param>
    protected void SelectItem(LookupItemViewModelBase item)
    {
        ModalService.Close(ModalResult.Ok(new List<LookupItemViewModelBase> { item }));
    }

    /// <summary>
    /// Clears the current filter, resetting it to a new instance.
    /// </summary>
    protected void ClearFilter()
    {
        Filter = new();
    }
}