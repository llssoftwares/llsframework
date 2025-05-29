namespace LLSFramework.TabBlazor.Components.Navbar;

/// <summary>
/// Represents a navigation bar component that supports both horizontal and vertical layouts,
/// manages menu items, and handles navigation events for responsive UI updates.
/// </summary>
public partial class LLSNavbar : TablerBaseComponent, IDisposable
{
    /// <summary>
    /// Provides navigation and URL management for the application.
    /// </summary>
    [Inject] private NavigationManager? NavigationManager { get; set; }

    /// <summary>
    /// Sets the background style of the navbar.
    /// </summary>
    [Parameter] public NavbarBackground Background { get; set; }

    /// <summary>
    /// Sets the direction (horizontal or vertical) of the navbar.
    /// </summary>
    [Parameter] public NavbarDirection Direction { get; set; }

    /// <summary>
    /// Indicates whether the navbar is currently expanded (for responsive layouts).
    /// </summary>
    public bool IsExpanded = false;

    /// <summary>
    /// The HTML tag used for rendering the navbar container.
    /// </summary>
    protected static string HtmlTag => "div";

    /// <summary>
    /// Internal list of all registered navbar menu items.
    /// </summary>
    private readonly List<LLSNavbarMenuItem> _navbarItems = [];

    /// <summary>
    /// Subscribes to navigation events when the component is initialized.
    /// </summary>
    protected override void OnInitialized()
    {
        if (NavigationManager != null)
            NavigationManager.LocationChanged += LocationChanged;

        base.OnInitialized();
    }

    /// <summary>
    /// Handles navigation changes. Closes all dropdowns if the navbar is horizontal.
    /// </summary>
    private void LocationChanged(object? sender, LocationChangedEventArgs e)
    {
        if (Direction == NavbarDirection.Horizontal)
            CloseAll();
    }

    /// <summary>
    /// Builds the CSS class string for the navbar based on its direction.
    /// </summary>
    protected override string ClassNames => ClassBuilder
        .Add("navbar navbar-expand-md")
        .AddIf("navbar-vertical", Direction == NavbarDirection.Vertical)
        .ToString();

    /// <summary>
    /// Toggles the expanded/collapsed state of the navbar (for mobile/responsive layouts).
    /// </summary>
    public void ToogleExpand()
    {
        IsExpanded = !IsExpanded;
    }

    /// <summary>
    /// Closes all top-level dropdown menus in the navbar.
    /// </summary>
    public void CloseAll()
    {
        foreach (var item in _navbarItems.Where(e => e.IsTopMenuItem))
        {
            item.CloseDropdown();
        }

        StateHasChanged();
    }

    /// <summary>
    /// Registers a menu item with the navbar if it is not already present.
    /// </summary>
    /// <param name="item">The menu item to add.</param>
    public void AddNavbarMenuItem(LLSNavbarMenuItem item)
    {
        if (!_navbarItems.Contains(item))
            _navbarItems.Add(item);
    }

    /// <summary>
    /// Removes a menu item from the navbar.
    /// </summary>
    /// <param name="item">The menu item to remove.</param>
    public void RemoveNavbarMenuItem(LLSNavbarMenuItem item)
    {
        _navbarItems.Remove(item);
    }

    /// <summary>
    /// Gets the submenu items for a given parent menu item.
    /// </summary>
    /// <param name="parentMenuItem">The parent menu item.</param>
    /// <returns>An enumerable of submenu items.</returns>
    public IEnumerable<LLSNavbarMenuItem> GetSubmenuItems(LLSNavbarMenuItem parentMenuItem)
    {
        return _navbarItems.Where(item => item.ParentMenuItem == parentMenuItem);
    }

    /// <summary>
    /// Unsubscribes from navigation events and releases resources.
    /// </summary>
    public void Dispose()
    {
        if (NavigationManager != null)
            NavigationManager.LocationChanged -= LocationChanged;

        GC.SuppressFinalize(this);
    }
}