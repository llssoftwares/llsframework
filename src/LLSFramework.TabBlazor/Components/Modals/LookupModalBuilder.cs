namespace LLSFramework.TabBlazor.Components.Modals;

public class LookupModalBuilder<TComponent>(IModalService modalService) where TComponent : IComponent
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

    public LookupModalBuilder<TComponent> Set<TValue>(Expression<Func<TComponent, TValue>> parameterSelector, TValue value)
    {
        if (value != null)
            _renderComponent.Set(parameterSelector, value);

        return this;
    }

    public LookupModalBuilder<TComponent> Title(string title)
    {
        _title = title;

        return this;
    }

    public LookupModalBuilder<TComponent> Size(ModalSize size)
    {
        _size = size;

        return this;
    }

    public LookupModalBuilder<TComponent> VerticalPosition(ModalVerticalPosition verticalPosition)
    {
        _verticalPosition = verticalPosition;

        return this;
    }

    public LookupModalBuilder<TComponent> ShowHeader(bool showHeader)
    {
        _showHeader = showHeader;

        return this;
    }

    public LookupModalBuilder<TComponent> ShowCloseButton(bool showCloseButton)
    {
        _showCloseButton = showCloseButton;

        return this;
    }

    public LookupModalBuilder<TComponent> Scrollable(bool scrollable)
    {
        _scrollable = scrollable;

        return this;
    }

    public LookupModalBuilder<TComponent> CloseOnClickOutside(bool closeOnClickOutside)
    {
        _closeOnClickOutside = closeOnClickOutside;

        return this;
    }

    public LookupModalBuilder<TComponent> BlurBackground(bool blurBackground)
    {
        _blurBackground = blurBackground;

        return this;
    }

    public LookupModalBuilder<TComponent> Backdrop(bool backdrop)
    {
        _backdrop = backdrop;

        return this;
    }

    public LookupModalBuilder<TComponent> CloseOnEsc(bool closeOnEsc)
    {
        _closeOnEsc = closeOnEsc;

        return this;
    }

    public LookupModalBuilder<TComponent> Draggable(bool draggable)
    {
        _draggable = draggable;

        return this;
    }

    public LookupModalBuilder<TComponent> CssClass(string modalCssClass)
    {
        _modalCssClass = modalCssClass;

        return this;
    }

    public LookupModalBuilder<TComponent> BodyCssClass(string modalBodyCssClass)
    {
        _modalBodyCssClass = modalBodyCssClass;

        return this;
    }

    public LookupModalBuilder<TComponent> Fullscreen(ModalFullscreen fullscreen)
    {
        _fullscreen = fullscreen;

        return this;
    }

    public LookupModalBuilder<TComponent> StatusColor(TablerColor statusColor)
    {
        _statusColor = statusColor;

        return this;
    }

    public LookupModalBuilder<TComponent> Danger()
    {
        _statusColor = TablerColor.Danger;

        return this;
    }

    public LookupModalBuilder<TComponent> Compact()
    {
        _size = ModalSize.Small;
        _showHeader = false;
        _showCloseButton = false;

        return this;
    }

    public async Task<LookupModalResult> ShowAsync()
    {
        var modalResult = await modalService.ShowAsync(_title, _renderComponent, new ModalOptions
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

        List<LookupItemViewModelBase> selected = [];

        switch (modalResult.Data)
        {
            case List<LookupItemViewModelBase> selectedList:
                selected = selectedList;
                break;
            case LookupItemViewModelBase selectedItem:
                selected = [selectedItem];
                break;
        }

        return new LookupModalResult(selected, modalResult.Cancelled);
    }
}