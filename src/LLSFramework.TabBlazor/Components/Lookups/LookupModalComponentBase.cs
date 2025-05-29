namespace LLSFramework.TabBlazor.Components.Lookups;

public abstract class LookupModalComponentBase<TFilter, TItem>
    : ComponentBase where TFilter
    : EntityFilter, new() where TItem : LookupItemViewModelBase
{
    [Inject] protected IModalService ModalService { get; set; } = default!;

    [Parameter] public string Text { get; set; } = string.Empty;

    [Parameter]
    public bool SearchOnInitialized { get; set; }

    protected LLSTable<TItem> Table = new();

    protected PaginatedResult<TItem> PaginatedResult = new();

    protected TFilter Filter = new();

    protected override void OnInitialized()
    {
        SetFilter();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender && SearchOnInitialized)
            await Table.RefreshAsync();
    }

    protected abstract void SetFilter();

    protected abstract Task<PaginatedResult<TItem>> FetchAsync();

    protected async Task SearchAsync()
    {
        await Table.GoToPageAsync(1);
    }

    protected async Task HandleTableChangedAsync(TableChangedEventArgs e)
    {
        Filter.SetOptions(e);

        PaginatedResult = await FetchAsync();
    }

    protected void SelectItem(LookupItemViewModelBase item)
    {
        ModalService.Close(ModalResult.Ok(new List<LookupItemViewModelBase> { item }));
    }

    protected void ClearFilter()
    {
        Filter = new();
    }
}