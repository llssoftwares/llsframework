namespace LLSFramework.TabBlazor.Components.Lookups;

/// <summary>
/// Abstract base class for modal lookup list components with filtering, pagination, and selection support.
/// Handles table state, filter management, and item selection logic for modal dialogs.
/// </summary>
/// <typeparam name="TFilter">The filter type, must inherit from <see cref="EntityFilter"/>.</typeparam>
/// <typeparam name="TItem">The item type, must inherit from <see cref="LookupItemViewModelBase"/>.</typeparam>
public abstract class LookupListModalComponentBase<TFilter, TItem>
    : ComponentBase where TFilter
    : EntityFilter, new() where TItem : LookupItemViewModelBase
{
    /// <summary>
    /// Service for displaying and closing modals, injected by the framework.
    /// </summary>
    [Inject] protected IModalService ModalService { get; set; } = default!;

    /// <summary>
    /// The list of currently selected items in the modal.
    /// </summary>
    [Parameter] public List<TItem> SelectedItems { get; set; } = [];

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
    /// Called when component parameters are set; updates the filter.
    /// </summary>
    protected override void OnParametersSet()
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
    /// Allows derived classes to customize the filter when parameters are set.
    /// </summary>
    protected virtual void SetFilter()
    {
    }

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
    /// fetches new data, and removes already selected items from the result list.
    /// </summary>
    /// <param name="e">The table change event arguments.</param>
    protected async Task HandleTableChangedAsync(TableChangedEventArgs e)
    {
        Filter.SetOptions(e);

        PaginatedResult = await FetchAsync();

        // Remove items from the result list that are already selected (case-insensitive).
        PaginatedResult.List.RemoveAll(x => SelectedItems.Select(x => x.Id.ToLower()).Contains(x.Id.ToLower()));
    }

    /// <summary>
    /// Confirms the selection and closes the modal, returning the selected items as base types.
    /// </summary>
    protected void Confirm()
    {
        ModalService.Close(ModalResult.Ok(SelectedItems.ToBaseList()));
    }

    /// <summary>
    /// Clears the current filter, resetting it to a new instance.
    /// </summary>
    protected void ClearFilter()
    {
        Filter = new();
    }

    /// <summary>
    /// Moves all selected items back to the result list and clears the selection.
    /// </summary>
    protected void ClearSelectedItems()
    {
        foreach (var item in SelectedItems)
        {
            PaginatedResult.List.Add(item);
        }

        SelectedItems.Clear();
    }

    /// <summary>
    /// Adds an item to the selection and removes it from the result list.
    /// </summary>
    /// <param name="item">The item to add.</param>
    protected void AddItem(TItem item)
    {
        PaginatedResult.List.Remove(item);

        SelectedItems.Add(item);
    }

    /// <summary>
    /// Removes an item from the selection and adds it back to the result list.
    /// </summary>
    /// <param name="item">The item to remove.</param>
    protected void RemoveItem(TItem item)
    {
        SelectedItems.Remove(item);

        PaginatedResult.List.Add(item);
    }
}