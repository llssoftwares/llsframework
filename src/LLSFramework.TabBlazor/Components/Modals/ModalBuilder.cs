namespace LLSFramework.TabBlazor.Components.Modals;

public class ModalBuilder<TComponent>(IModalService modalService) where TComponent : IComponent
{
    private readonly RenderComponent<TComponent> _renderComponent = new();
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

    public ModalBuilder<TComponent> Set<TValue>(Expression<Func<TComponent, TValue>> parameterSelector, TValue value)
    {
        if (value != null)
            _renderComponent.Set(parameterSelector, value);

        return this;
    }

    public ModalBuilder<TComponent> Title(string title)
    {
        _title = title;

        return this;
    }

    public ModalBuilder<TComponent> Size(ModalSize size)
    {
        _size = size;

        return this;
    }

    public ModalBuilder<TComponent> VerticalPosition(ModalVerticalPosition verticalPosition)
    {
        _verticalPosition = verticalPosition;

        return this;
    }

    public ModalBuilder<TComponent> ShowHeader(bool showHeader)
    {
        _showHeader = showHeader;

        return this;
    }

    public ModalBuilder<TComponent> ShowCloseButton(bool showCloseButton)
    {
        _showCloseButton = showCloseButton;

        return this;
    }

    public ModalBuilder<TComponent> Scrollable(bool scrollable)
    {
        _scrollable = scrollable;

        return this;
    }

    public ModalBuilder<TComponent> CloseOnClickOutside(bool closeOnClickOutside)
    {
        _closeOnClickOutside = closeOnClickOutside;

        return this;
    }

    public ModalBuilder<TComponent> BlurBackground(bool blurBackground)
    {
        _blurBackground = blurBackground;

        return this;
    }

    public ModalBuilder<TComponent> Backdrop(bool backdrop)
    {
        _backdrop = backdrop;

        return this;
    }

    public ModalBuilder<TComponent> CloseOnEsc(bool closeOnEsc)
    {
        _closeOnEsc = closeOnEsc;

        return this;
    }

    public ModalBuilder<TComponent> Draggable(bool draggable)
    {
        _draggable = draggable;

        return this;
    }

    public ModalBuilder<TComponent> CssClass(string modalCssClass)
    {
        _modalCssClass = modalCssClass;

        return this;
    }

    public ModalBuilder<TComponent> BodyCssClass(string modalBodyCssClass)
    {
        _modalBodyCssClass = modalBodyCssClass;

        return this;
    }

    public ModalBuilder<TComponent> Fullscreen(ModalFullscreen fullscreen)
    {
        _fullscreen = fullscreen;

        return this;
    }

    public ModalBuilder<TComponent> StatusColor(TablerColor statusColor)
    {
        _statusColor = statusColor;

        return this;
    }

    public ModalBuilder<TComponent> Danger()
    {
        _statusColor = TablerColor.Danger;

        return this;
    }

    public ModalBuilder<TComponent> Compact()
    {
        _size = ModalSize.Small;
        _showHeader = false;
        _showCloseButton = false;

        return this;
    }

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