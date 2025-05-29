namespace LLSFramework.TabBlazor.Components.Lookups;

public abstract class LookupComponentBase : InputBase<string?>
{
    [Inject] protected IModalService ModalService { get; set; } = default!;

    [Parameter] public string? Id { get; set; }

    [Parameter] public EventCallback<string?> IdChanged { get; set; }

    [Parameter] public string Text { get; set; } = string.Empty;

    [Parameter] public EventCallback<string?> TextChanged { get; set; }

    [Parameter] public bool SearchOnInitialized { get; set; }

    [Parameter] public bool DisabledInput { get; set; }

    protected override bool TryParseValueFromString(string? value, out string? result, out string validationErrorMessage)
    {
        result = value;
        validationErrorMessage = string.Empty;
        return true;
    }

    protected abstract Task<LookupModalResult> ConfigureAsync();

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

    protected async Task ClearAsync()
    {
        await IdChanged.InvokeAsync(null);
        await TextChanged.InvokeAsync(null);
    }

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

    protected async Task HandleOnInputAsync(ChangeEventArgs e)
    {
        Text = e.Value?.ToString() ?? string.Empty;

        await TextChanged.InvokeAsync(Text);
    }
}