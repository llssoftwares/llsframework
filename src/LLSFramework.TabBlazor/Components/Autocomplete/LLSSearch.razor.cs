using Microsoft.AspNetCore.Components.Web;
using Timer = System.Timers.Timer;

namespace LLSFramework.TabBlazor.Components.Autocomplete;

public partial class LLSSearch<TItem> : TablerBaseComponent, IDisposable
{
    [Inject] public TablerService? TablerService { get; set; }

    [CascadingParameter] protected EditContext? EditContext { get; set; }

    private string FieldCssClasses =>
        new ClassBuilder()
            .Add(ValidationClasses)
            .Add("form-control")
            .AddIf(CssClass, CssClass != null)
            .ToString();

    private FieldIdentifier FieldIdentifier { get; set; }

    [Parameter] public string? CssClass { get; set; }

    [Parameter] public string? ResultHeader { get; set; }

    [Parameter] public required RenderFragment<TItem> ResultTemplate { get; set; }

    [Parameter] public RenderFragment? NotFoundTemplate { get; set; }

    [Parameter] public EventCallback<string> ValueChanged { get; set; }

    [Parameter] public Expression<Func<string>>? ValueExpression { get; set; }

    [Parameter] public EventCallback<FocusEventArgs> OnBlur { set; get; }

    [Parameter] public string? Value { get; set; } = string.Empty;

    [Parameter] public int Debounce { get; set; } = 300;

    [Parameter] public Expression<Func<TItem, object>>? GroupBy { get; set; }

    [Parameter] public required Func<object, string> GroupingHeaderExpression { get; set; }

    [Parameter] public RenderFragment<object>? GroupingHeaderTemplate { get; set; }

    [Parameter] public required Func<string, Task<List<TItem>>> SearchMethod { get; set; }

    [Parameter] public EventCallback<TItem> OnItemSelected { get; set; }

    [Parameter] public string? SeparatorCharacter { get; set; }

    [Parameter] public bool Disabled { get; set; } = false;

    [Parameter] public bool DisableValidation { get; set; }

    [Parameter] public bool ShowOptionOnFocus { get; set; }

    [Parameter] public string? Placeholder { get; set; }

    [Parameter] public int MinimumLength { get; set; } = 2;

    [Parameter] public bool Rounded { get; set; }

    private int SelectedIndex { get; set; } = -1;

    private List<TItem> Result { get; set; } = [];

    private IEnumerable<IGrouping<object, TItem>> GroupedResult { get; set; } = [];

    private List<TItem> ActualItems => Result ?? GroupedResult.SelectMany(x => x).ToList();

    private bool IsShowingSuggestions { get; set; } = false;

    private Timer? Timer { get; set; }

    private string searchText = string.Empty;

    private ElementReference _searchInput;

    private bool _eventsHookedUp;

    private string ValidationClasses => EditContext?.FieldCssClass(FieldIdentifier) ?? "";

    protected override void OnInitialized()
    {
        if (ValueExpression != null)
            FieldIdentifier = FieldIdentifier.Create(ValueExpression);

        GroupingHeaderExpression ??= item => item?.ToString() ?? string.Empty;

        Timer = new Timer
        {
            Interval = Debounce,
            AutoReset = false
        };
        Timer.Elapsed += async (sender, args) => await DoSearchAsync();
    }

    protected override void OnParametersSet()
    {
        UpdateInput();
    }

    private async Task OnBlurTriggeredAsync()
    {
        await Task.Delay(300);
        await UpdateInput();
        IsShowingSuggestions = false;
        await InvokeAsync(StateHasChanged);
        await OnBlur.InvokeAsync();
    }

    private Task UpdateInput()
    {
        UnmatchedParameters ??= new Dictionary<string, object>();

        searchText = Value ?? string.Empty;

        UnmatchedParameters["class"] = FieldCssClasses;

        return Task.CompletedTask;
    }

    private async Task OnFocusAsync()
    {
        if (ShowOptionOnFocus)
        {
            ForceShowOptions = true;
            SearchText ??= "";
            await DoSearchAsync();
            ForceShowOptions = false;
        }
    }

    public bool ForceShowOptions { get; set; }

    private void InputChanged()
    {
        if (ForceShowOptions)
            return;

        if (MinimumLength > 0 && Value == null) return;

        searchText = GetSearchText(Value ?? string.Empty);

        if (searchText.Length < MinimumLength)
        {
            Timer?.Stop();
            SelectedIndex = -1;
            IsShowingSuggestions = false;
        }
        else if (searchText.Length >= MinimumLength)
        {
            Timer?.Stop();
            Timer?.Start();
        }

        ValueChanged.InvokeAsync(Value ?? string.Empty);
        UpdateInput();
        InvokeAsync(StateHasChanged);

        if (!DisableValidation)
            EditContext?.NotifyFieldChanged(FieldIdentifier);
    }

    private string SearchText
    {
        get => searchText;
        set
        {
            Value = value;
            InputChanged();
        }
    }

    private async Task OnItemSelectedCallbackAsync(TItem item)
    {
        if (OnItemSelected.HasDelegate)
            await OnItemSelected.InvokeAsync(item);

        if (!DisableValidation)
            EditContext?.NotifyFieldChanged(FieldIdentifier);

        IsShowingSuggestions = false;
        SelectedIndex = -1;
        searchText = string.Empty;
    }

    private static bool SearchTextInAutoCompleteList(string input, IEnumerable<TItem> listResult)
    {
        return listResult.Any();
    }

    private async Task DoSearchAsync()
    {
        var search = GetSearchText(SearchText);

        if (GroupBy != null)
            GroupedResult = (await SearchMethod!.Invoke(search)).GroupBy(GroupBy.Compile());
        else
        {
            Result = await SearchMethod!.Invoke(search ?? "");
        }

        IsShowingSuggestions = NotFoundTemplate != null || Result?.Count > 0 == true || GroupedResult?.Any() == true;

        SelectedIndex = -1;

        await InvokeAsync(StateHasChanged);
    }

    private void OnClickOutside()
    {
        Close();
        StateHasChanged();
    }

    private void Close()
    {
        IsShowingSuggestions = false;
    }

    public void Dispose()
    {
        Timer?.Dispose();
        GC.SuppressFinalize(this);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender && !Disabled || !_eventsHookedUp && !Disabled)
        {
            try
            {
                await TablerService!.PreventDefaultKey(_searchInput, "keydown", ["Enter"]);
            }
            catch
            {
            }

            _eventsHookedUp = true;
        }
    }

    private void MoveSelection(int count)
    {
        var index = SelectedIndex + count;

        if (index >= ActualItems.Count)
            index = 0;

        if (index < 0)
            index = ActualItems.Count - 1;

        SelectedIndex = index;
    }

    private async Task HandleKeyupAsync(KeyboardEventArgs args)
    {
        if (ActualItems == null)
            return;

        if (args.Key == "ArrowDown")
            MoveSelection(1);
        else if (args.Key == "ArrowUp")
        {
            MoveSelection(-1);
        }
        else if (args.Key == "Enter" && SelectedIndex >= 0 && SelectedIndex < ActualItems.Count)
        {
            await OnItemSelectedCallbackAsync(ActualItems[SelectedIndex]);
        }
        else if (args.Key == "Escape")
        {
            IsShowingSuggestions = false;
            SelectedIndex = -1;
        }
    }

    private string GetSelectedSuggestionClass(TItem item, int index)
    {
        const string resultClass = "active";

        return Equals(item, Value)
            ? index == SelectedIndex ? resultClass : ""
            : index == SelectedIndex ? resultClass : Equals(item, Value) ? resultClass : string.Empty;
    }

    private string GetSearchText(string value)
    {
        if (string.IsNullOrWhiteSpace(SeparatorCharacter))
            return value;

        var splitString = value.Split(SeparatorCharacter);

        return splitString.Length != 0 ? splitString[^1].Trim() : string.Empty;
    }
}