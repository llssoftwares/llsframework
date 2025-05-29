namespace LLSFramework.TabBlazor.Components.Modals;

/// <summary>
/// Builder class for configuring and displaying a modal dialog containing a custom Blazor component.
/// Supports fluent configuration of modal options and component parameters.
/// </summary>
/// <typeparam name="TComponent">The Blazor component type to render inside the modal. Must implement <see cref="IComponent"/>.</typeparam>
public class ModalBuilder<TComponent>(IModalService modalService) where TComponent : IComponent
{
    // Holds the component and its parameters to be rendered in the modal.
    private readonly RenderComponent<TComponent> _renderComponent = new();

    // Modal configuration fields with their default values.
    private string? _title = null;
    private ModalSize _size = ModalSize.Medium;
    private ModalVerticalPosition _verticalPosition = ModalVerticalPosition.Centered;
    private bool _showHeader = true;
    private bool _showCloseButton = true;
    private bool _scrollable = true;
    private bool _closeOnClickOutside = false;
    private bool _blurBackground = true;
    private bool _backdrop = true;
    private bool _closeOnEsc;
    private bool _draggable;
    private string? _modalCssClass;
    private string? _modalBodyCssClass;
    private ModalFullscreen _fullscreen;
    private TablerColor? _statusColor;

    /// <summary>
    /// Sets a parameter value for the component rendered in the modal.
    /// </summary>
    /// <typeparam name="TValue">The type of the parameter.</typeparam>
    /// <param name="parameterSelector">An expression selecting the parameter property.</param>
    /// <param name="value">The value to set.</param>
    /// <returns>The builder instance for chaining.</returns>
    public ModalBuilder<TComponent> Set<TValue>(Expression<Func<TComponent, TValue>> parameterSelector, TValue value)
    {
        if (value != null)
            _renderComponent.Set(parameterSelector, value);

        return this;
    }

    /// <summary>
    /// Sets the modal title.
    /// </summary>
    public ModalBuilder<TComponent> Title(string title)
    {
        _title = title;

        return this;
    }

    /// <summary>
    /// Sets the modal size.
    /// </summary>
    public ModalBuilder<TComponent> Size(ModalSize size)
    {
        _size = size;

        return this;
    }

    /// <summary>
    /// Sets the vertical position of the modal.
    /// </summary>
    public ModalBuilder<TComponent> VerticalPosition(ModalVerticalPosition verticalPosition)
    {
        _verticalPosition = verticalPosition;

        return this;
    }

    /// <summary>
    /// Sets whether to show the modal header.
    /// </summary>
    public ModalBuilder<TComponent> ShowHeader(bool showHeader)
    {
        _showHeader = showHeader;

        return this;
    }

    /// <summary>
    /// Sets whether to show the close button in the modal header.
    /// </summary>
    public ModalBuilder<TComponent> ShowCloseButton(bool showCloseButton)
    {
        _showCloseButton = showCloseButton;

        return this;
    }

    /// <summary>
    /// Sets whether the modal body is scrollable.
    /// </summary>
    public ModalBuilder<TComponent> Scrollable(bool scrollable)
    {
        _scrollable = scrollable;

        return this;
    }

    /// <summary>
    /// Sets whether clicking outside the modal closes it.
    /// </summary>
    public ModalBuilder<TComponent> CloseOnClickOutside(bool closeOnClickOutside)
    {
        _closeOnClickOutside = closeOnClickOutside;

        return this;
    }

    /// <summary>
    /// Sets whether the background is blurred when the modal is open.
    /// </summary>
    public ModalBuilder<TComponent> BlurBackground(bool blurBackground)
    {
        _blurBackground = blurBackground;

        return this;
    }

    /// <summary>
    /// Sets whether to show a backdrop behind the modal.
    /// </summary>
    public ModalBuilder<TComponent> Backdrop(bool backdrop)
    {
        _backdrop = backdrop;

        return this;
    }

    /// <summary>
    /// Sets whether pressing the Escape key closes the modal.
    /// </summary>
    public ModalBuilder<TComponent> CloseOnEsc(bool closeOnEsc)
    {
        _closeOnEsc = closeOnEsc;

        return this;
    }

    /// <summary>
    /// Sets whether the modal is draggable.
    /// </summary>
    public ModalBuilder<TComponent> Draggable(bool draggable)
    {
        _draggable = draggable;

        return this;
    }

    /// <summary>
    /// Sets a custom CSS class for the modal container.
    /// </summary>
    public ModalBuilder<TComponent> CssClass(string modalCssClass)
    {
        _modalCssClass = modalCssClass;

        return this;
    }

    /// <summary>
    /// Sets a custom CSS class for the modal body.
    /// </summary>
    public ModalBuilder<TComponent> BodyCssClass(string modalBodyCssClass)
    {
        _modalBodyCssClass = modalBodyCssClass;

        return this;
    }

    /// <summary>
    /// Sets the modal to fullscreen mode.
    /// </summary>
    public ModalBuilder<TComponent> Fullscreen(ModalFullscreen fullscreen)
    {
        _fullscreen = fullscreen;

        return this;
    }

    /// <summary>
    /// Sets the status color (e.g., for header or border) of the modal.
    /// </summary>
    public ModalBuilder<TComponent> StatusColor(TablerColor statusColor)
    {
        _statusColor = statusColor;

        return this;
    }

    /// <summary>
    /// Sets the modal status color to "Danger" (typically red).
    /// </summary>
    public ModalBuilder<TComponent> Danger()
    {
        _statusColor = TablerColor.Danger;

        return this;
    }

    /// <summary>
    /// Configures the modal for a compact appearance (small size, no header or close button).
    /// </summary>
    public ModalBuilder<TComponent> Compact()
    {
        _size = ModalSize.Small;
        _showHeader = false;
        _showCloseButton = false;

        return this;
    }

    /// <summary>
    /// Shows the modal asynchronously with the configured options and returns the modal result.
    /// </summary>
    /// <returns>
    /// A <see cref="ModalResult"/> containing the result data and cancellation status.
    /// </returns>
    public async Task<ModalResult> ShowAsync()
    {
        return await modalService.ShowAsync(_title, _renderComponent, new ModalOptions
        {
            Size = _size,
            VerticalPosition = _verticalPosition,
            ShowHeader = _showHeader,
            ShowCloseButton = _showCloseButton,
            Scrollable = _scrollable,
            CloseOnClickOutside = _closeOnClickOutside,
            BlurBackground = _blurBackground,
            Backdrop = _backdrop,
            CloseOnEsc = _closeOnEsc,
            Draggable = _draggable,
            ModalCssClass = _modalCssClass,
            ModalBodyCssClass = _modalBodyCssClass,
            Fullscreen = _fullscreen,
            StatusColor = _statusColor
        });
    }
}