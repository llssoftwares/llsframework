using Microsoft.AspNetCore.Components.Web;
using Timer = System.Timers.Timer;

namespace LLSFramework.TabBlazor.Components.Autocomplete;

/// <summary>
/// Generic Blazor autocomplete/search component with support for grouping, custom templates,
/// debounced search, and keyboard navigation.
/// </summary>
public partial class LLSSearch<TItem> : TablerBaseComponent, IDisposable
{
    // Service for handling Tabler-specific JS interop (e.g., keyboard event prevention)
    [Inject] public TablerService? TablerService { get; set; }

    // EditContext for form validation and field tracking
    [CascadingParameter] protected EditContext? EditContext { get; set; }

    // Computes the CSS classes for the input field, including validation and custom classes
    private string FieldCssClasses =>
        new ClassBuilder()
            .Add(ValidationClasses)
            .Add("form-control")
            .AddIf(CssClass, CssClass != null)
            .ToString();

    // Identifies the field for validation and change notification
    private FieldIdentifier FieldIdentifier { get; set; }

    // Custom CSS class for the input field
    [Parameter] public string? CssClass { get; set; }

    // Optional header to display above the result list
    [Parameter] public string? ResultHeader { get; set; }

    // Template for rendering each search result item (required)
    [Parameter] public required RenderFragment<TItem> ResultTemplate { get; set; }

    // Template to display when no results are found
    [Parameter] public RenderFragment? NotFoundTemplate { get; set; }

    // Event callback for when the input value changes
    [Parameter] public EventCallback<string> ValueChanged { get; set; }

    // Expression for binding to the input value (for validation)
    [Parameter] public Expression<Func<string>>? ValueExpression { get; set; }

    // Event callback for when the input loses focus
    [Parameter] public EventCallback<FocusEventArgs> OnBlur { set; get; }

    // The current value of the input field
    [Parameter] public string? Value { get; set; } = string.Empty;

    // Debounce interval in milliseconds for search execution
    [Parameter] public int Debounce { get; set; } = 300;

    // Expression for grouping search results
    [Parameter] public Expression<Func<TItem, object>>? GroupBy { get; set; }

    // Function to get the group header text from a group key (required if grouping)
    [Parameter] public required Func<object, string> GroupingHeaderExpression { get; set; }

    // Template for rendering group headers
    [Parameter] public RenderFragment<object>? GroupingHeaderTemplate { get; set; }

    // The search method to call with the search text (required)
    [Parameter] public required Func<string, Task<List<TItem>>> SearchMethod { get; set; }

    // Event callback for when a result item is selected
    [Parameter] public EventCallback<TItem> OnItemSelected { get; set; }

    // Optional separator character for multi-value input (e.g., comma-separated tags)
    [Parameter] public string? SeparatorCharacter { get; set; }

    // Disables the input field if true
    [Parameter] public bool Disabled { get; set; } = false;

    // Disables validation if true
    [Parameter] public bool DisableValidation { get; set; }

    // If true, shows options when the input receives focus
    [Parameter] public bool ShowOptionOnFocus { get; set; }

    // Placeholder text for the input field
    [Parameter] public string? Placeholder { get; set; }

    // Minimum length of input before search is triggered
    [Parameter] public int MinimumLength { get; set; } = 2;

    // If true, renders the input with rounded corners
    [Parameter] public bool Rounded { get; set; }

    // Index of the currently selected suggestion (for keyboard navigation)
    private int SelectedIndex { get; set; } = -1;

    // Flat list of search results
    private List<TItem> Result { get; set; } = [];

    // Grouped search results (if grouping is enabled)
    private IEnumerable<IGrouping<object, TItem>> GroupedResult { get; set; } = [];

    // Returns the actual items to display, whether grouped or flat
    private List<TItem> ActualItems => Result ?? GroupedResult.SelectMany(x => x).ToList();

    // Indicates whether the suggestion dropdown is visible
    private bool IsShowingSuggestions { get; set; } = false;

    // Timer for debouncing search input
    private Timer? Timer { get; set; }

    // The current search text (may differ from Value if using separators)
    private string searchText = string.Empty;

    // Reference to the input element for JS interop
    private ElementReference _searchInput;

    // Tracks whether JS event handlers have been hooked up
    private bool _eventsHookedUp;

    // Gets the validation CSS classes for the field
    private string ValidationClasses => EditContext?.FieldCssClass(FieldIdentifier) ?? "";

    /// <summary>
    /// Initializes component state, sets up grouping and debounce timer.
    /// </summary>
    protected override void OnInitialized()
    {
        if (ValueExpression != null)
            FieldIdentifier = FieldIdentifier.Create(ValueExpression);

        // Default group header expression if not provided
        GroupingHeaderExpression ??= item => item?.ToString() ?? string.Empty;

        // Set up debounce timer for search
        Timer = new Timer
        {
            Interval = Debounce,
            AutoReset = false
        };
        Timer.Elapsed += async (sender, args) => await DoSearchAsync();
    }

    /// <summary>
    /// Updates the input field and unmatched parameters when parameters are set.
    /// </summary>
    protected override void OnParametersSet()
    {
        UpdateInput();
    }

    /// <summary>
    /// Handles blur event: waits briefly, updates input, hides suggestions, and notifies parent.
    /// </summary>
    private async Task OnBlurTriggeredAsync()
    {
        await Task.Delay(300);
        await UpdateInput();
        IsShowingSuggestions = false;
        await InvokeAsync(StateHasChanged);
        await OnBlur.InvokeAsync();
    }

    /// <summary>
    /// Updates the input field's value and CSS class in unmatched parameters.
    /// </summary>
    private Task UpdateInput()
    {
        UnmatchedParameters ??= new Dictionary<string, object>();

        searchText = Value ?? string.Empty;

        UnmatchedParameters["class"] = FieldCssClasses;

        return Task.CompletedTask;
    }

    /// <summary>
    /// Handles focus event: optionally shows options and triggers search.
    /// </summary>
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

    // Forces the suggestion dropdown to show regardless of input
    public bool ForceShowOptions { get; set; }

    /// <summary>
    /// Handles input changes: manages debounce, validation, and triggers search.
    /// </summary>
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

    /// <summary>
    /// Gets or sets the search text, updating the input and triggering search logic.
    /// </summary>
    private string SearchText
    {
        get => searchText;
        set
        {
            Value = value;
            InputChanged();
        }
    }

    /// <summary>
    /// Handles selection of a result item: notifies parent, resets state, and hides suggestions.
    /// </summary>
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

    /// <summary>
    /// Checks if there are any results in the autocomplete list.
    /// </summary>
    private static bool SearchTextInAutoCompleteList(string input, IEnumerable<TItem> listResult)
    {
        return listResult.Any();
    }

    /// <summary>
    /// Executes the search using the provided SearchMethod and updates results.
    /// Handles grouping if GroupBy is set.
    /// </summary>
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

    /// <summary>
    /// Handles click outside the component: closes suggestions.
    /// </summary>
    private void OnClickOutside()
    {
        Close();
        StateHasChanged();
    }

    /// <summary>
    /// Closes the suggestion dropdown.
    /// </summary>
    private void Close()
    {
        IsShowingSuggestions = false;
    }

    /// <summary>
    /// Disposes resources (timer) when the component is destroyed.
    /// </summary>
    public void Dispose()
    {
        Timer?.Dispose();
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// After first render, sets up JS event handlers to prevent default key actions (e.g., Enter).
    /// </summary>
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

    /// <summary>
    /// Moves the selection up or down in the suggestion list for keyboard navigation.
    /// </summary>
    private void MoveSelection(int count)
    {
        var index = SelectedIndex + count;

        if (index >= ActualItems.Count)
            index = 0;

        if (index < 0)
            index = ActualItems.Count - 1;

        SelectedIndex = index;
    }

    /// <summary>
    /// Handles keyup events for navigation and selection in the suggestion list.
    /// </summary>
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

    /// <summary>
    /// Returns the CSS class for a suggestion item based on selection and value.
    /// </summary>
    private string GetSelectedSuggestionClass(TItem item, int index)
    {
        const string resultClass = "active";

        return Equals(item, Value)
            ? index == SelectedIndex ? resultClass : ""
            : index == SelectedIndex ? resultClass : Equals(item, Value) ? resultClass : string.Empty;
    }

    /// <summary>
    /// Extracts the search text from the input value, handling separator characters if present.
    /// </summary>
    private string GetSearchText(string value)
    {
        if (string.IsNullOrWhiteSpace(SeparatorCharacter))
            return value;

        var splitString = value.Split(SeparatorCharacter);

        return splitString.Length != 0 ? splitString[^1].Trim() : string.Empty;
    }
}