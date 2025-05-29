namespace LLSFramework.TabBlazor.Components.Lookups;

public abstract class LookupListModalComponentBase<TFilter, TItem>
    : ComponentBase where TFilter
    : EntityFilter, new() where TItem : LookupItemViewModelBase
{
    [Inject] protected IModalService ModalService { get; set; } = default!;

    [Parameter] public List<TItem> SelectedItems { get; set; } = [];

    [Parameter]
    public bool SearchOnInitialized { get; set; }

    protected LLSTable<TItem> Table = new();

    protected PaginatedResult<TItem> PaginatedResult = new();

    protected TFilter Filter = new();

    protected override void OnParametersSet()
    {
        SetFilter();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender && SearchOnInitialized)
            await Table.RefreshAsync();
    }

    protected virtual void SetFilter()
    {
    }

    protected abstract Task<PaginatedResult<TItem>> FetchAsync();

    protected async Task SearchAsync()
    {
        await Table.GoToPageAsync(1);
    }

    protected async Task HandleTableChangedAsync(TableChangedEventArgs e)
    {
        Filter.SetOptions(e);

        PaginatedResult = await FetchAsync();

        PaginatedResult.List.RemoveAll(x => SelectedItems.Select(x => x.Id.ToLower()).Contains(x.Id.ToLower()));
    }

    protected void Confirm()
    {
        ModalService.Close(ModalResult.Ok(SelectedItems.ToBaseList()));
    }

    protected void ClearFilter()
    {
        Filter = new();
    }

    protected void ClearSelectedItems()
    {
        foreach (var item in SelectedItems)
        {
            PaginatedResult.List.Add(item);
        }

        SelectedItems.Clear();
    }

    protected void AddItem(TItem item)
    {
        PaginatedResult.List.Remove(item);

        SelectedItems.Add(item);
    }

    protected void RemoveItem(TItem item)
    {
        SelectedItems.Remove(item);

        PaginatedResult.List.Add(item);
    }
}