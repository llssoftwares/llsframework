namespace LLSFramework.TabBlazor.Components.Lookups;

/// <summary>
/// Base class for lookup components that provide modal-based selection functionality.
/// Handles value binding, text display, and modal interaction for lookups.
/// </summary>
public abstract class LookupComponentBase : InputBase<string?>
{
    /// <summary>
    /// Service for displaying modals, injected by the framework.
    /// </summary>
    [Inject] protected IModalService ModalService { get; set; } = default!;

    /// <summary>
    /// The selected item's identifier.
    /// </summary>
    [Parameter] public string? Id { get; set; }

    /// <summary>
    /// Event callback triggered when the selected item's identifier changes.
    /// </summary>
    [Parameter] public EventCallback<string?> IdChanged { get; set; }

    /// <summary>
    /// The display text for the selected item.
    /// </summary>
    [Parameter] public string Text { get; set; } = string.Empty;

    /// <summary>
    /// Event callback triggered when the display text changes.
    /// </summary>
    [Parameter] public EventCallback<string?> TextChanged { get; set; }

    /// <summary>
    /// If true, performs a search when the component is initialized.
    /// </summary>
    [Parameter] public bool SearchOnInitialized { get; set; }

    /// <summary>
    /// If true, disables the input field.
    /// </summary>
    [Parameter] public bool DisabledInput { get; set; }

    /// <summary>
    /// Always succeeds in parsing the input value from a string.
    /// </summary>
    protected override bool TryParseValueFromString(string? value, out string? result, out string validationErrorMessage)
    {
        result = value;
        validationErrorMessage = string.Empty;
        return true;
    }

    /// <summary>
    /// Configures the modal dialog for the lookup.
    /// Must be implemented by derived classes to provide modal options.
    /// </summary>
    /// <returns>A <see cref="LookupModalResult"/> representing the modal outcome.</returns>
    protected abstract Task<LookupModalResult> ConfigureAsync();

    /// <summary>
    /// Opens the lookup modal, updates Id and Text if a selection is made.
    /// </summary>
    protected async Task OpenAsync()
    {
        var result = await ConfigureAsync();

        if (!result.Cancelled)
        {
            var selected = result.Selected.Single();

            await IdChanged.InvokeAsync(selected.Id);
            await TextChanged.InvokeAsync(selected.Text);
        }
    }

    /// <summary>
    /// Clears the current selection and notifies parent components.
    /// </summary>
    protected async Task ClearAsync()
    {
        await IdChanged.InvokeAsync(null);
        await TextChanged.InvokeAsync(null);
    }

    /// <summary>
    /// Returns the appropriate CSS class for the input field based on validation state.
    /// </summary>
    protected string GetCssClass()
    {
        if (EditContext == null || FieldIdentifier.Equals(default))
            return "form-control";

        var isTouched = EditContext.IsModified(FieldIdentifier) || EditContext.GetValidationMessages(FieldIdentifier).Any();

        if (!isTouched && string.IsNullOrEmpty(Id))
            return "form-control";

        var isValid = !EditContext.GetValidationMessages(FieldIdentifier).Any();

        return isValid ? "form-control is-valid" : "form-control is-invalid";
    }

    /// <summary>
    /// Handles input changes, updates the display text, and notifies parent components.
    /// </summary>
    protected async Task HandleOnInputAsync(ChangeEventArgs e)
    {
        Text = e.Value?.ToString() ?? string.Empty;

        await TextChanged.InvokeAsync(Text);
    }
}