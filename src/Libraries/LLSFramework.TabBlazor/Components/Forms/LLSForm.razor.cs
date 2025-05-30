namespace LLSFramework.TabBlazor.Components.Forms;

public partial class LLSForm : ComponentBase
{
    [Inject]
    protected IServiceProvider? Provider { get; set; }

    [Parameter(CaptureUnmatchedValues = true)]
    public IDictionary<string, object>? UnknownParameters { get; set; }

    [Parameter]
    public object? Model { get; set; }

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    [Parameter]
    public IFormValidator? Validator { get; set; }

    [Parameter]
    public EventCallback<EditContext> OnValidSubmit { get; set; }

    [Parameter]

    public bool IsValid { get; set; } = true;

    [Parameter]
    public EventCallback<bool> IsValidChanged { get; set; }

    public DynamicComponent? ValidatorInstance { get; set; }

    protected EditContext? EditContext { get; set; }

    public bool RenderForm { get; set; }

    public bool CanSubmit => IsValid;

    private bool _initialized;

    protected override async Task OnParametersSetAsync()
    {
        await SetupFormAsync();
    }

    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(DataAnnotationsValidator))]
    protected override void OnInitialized()
    {
    }

    private async Task SetupFormAsync()
    {
        if (Model == null)
        {
            RenderForm = false;
            EditContext = null;
            return;
        }

        Validator = GetValidator();

        if (EditContext == null || !EditContext.Model.Equals(Model))
        {
            EditContext = new EditContext(Model);
            await ValidateAsync();
        }

        EditContext.SetFieldCssClassProvider(new TabFieldCssClassProvider());

        RenderForm = true;
    }

    private IFormValidator GetValidator() => Validator ?? Provider?.GetRequiredService<IFormValidator>() ?? throw new InvalidOperationException();

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!RenderForm) return;

        if (firstRender)
            OnAfterModelValidation(true);
        else
        {
            var valid = await ValidateAsync();
            OnAfterModelValidation(valid);
        }
    }

    public void OnAfterModelValidation(bool isValid)
    {
        if (isValid != IsValid || !_initialized)
        {
            _initialized = true;
            IsValid = isValid;
            StateHasChanged();
            IsValidChanged.InvokeAsync(IsValid);
        }
    }

    public async Task<bool> ValidateAsync()
    {
        if (Validator == null) throw new ArgumentNullException(nameof(Validator));

        var valid = await Validator.ValidateAsync(ValidatorInstance?.Instance, EditContext);

        OnAfterModelValidation(valid);

        return IsValid;
    }

    public bool Validate()
    {
        if (Validator == null) throw new ArgumentNullException(nameof(Validator));

        var valid = Validator.Validate(ValidatorInstance?.Instance, EditContext);

        OnAfterModelValidation(valid);

        return IsValid;
    }

    protected async Task HandleValidSubmitAsync()
    {
        if (CanSubmit)
        {
            await OnValidSubmit.InvokeAsync(EditContext);
            EditContext?.MarkAsUnmodified();
        }
    }

    public void Dispose()
    {
        EditContext = null;
    }
}