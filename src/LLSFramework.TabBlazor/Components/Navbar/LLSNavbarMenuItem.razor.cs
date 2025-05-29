namespace LLSFramework.TabBlazor.Components.Navbar;

/// <summary>
/// Represents a menu item within a navigation bar, supporting nested submenus, dropdowns,
/// and active state tracking based on navigation.
/// </summary>
public partial class LLSNavbarMenuItem : TablerBaseComponent, IDisposable
{
    /// <summary>
    /// Provides navigation and URL management for the application.
    /// </summary>
    [Inject] NavigationManager? NavigationManager { get; set; }

    /// <summary>
    /// The parent navbar component, provided via cascading parameter.
    /// Used for menu registration and submenu management.
    /// </summary>
    [CascadingParameter(Name = "Navbar")] LLSNavbar? Navbar { get; set; }

    /// <summary>
    /// The parent menu item, if this is a submenu item. Provided via cascading parameter.
    /// </summary>
    [CascadingParameter(Name = "Parent")] public LLSNavbarMenuItem? ParentMenuItem { get; set; }

    /// <summary>
    /// The base URL for the menu item (used for active state matching).
    /// </summary>
    [Parameter] public string? BaseHref { get; set; }

    /// <summary>
    /// The navigation URL for the menu item.
    /// </summary>
    [Parameter] public string? Href { get; set; }

    /// <summary>
    /// The display text for the menu item.
    /// </summary>
    [Parameter] public string? Text { get; set; }

    /// <summary>
    /// Optional icon to display with the menu item.
    /// </summary>
    [Parameter] public RenderFragment? MenuItemIcon { get; set; }

    /// <summary>
    /// Optional submenu content for dropdowns or nested menus.
    /// </summary>
    [Parameter] public RenderFragment? SubMenu { get; set; }

    /// <summary>
    /// If true, the menu item is initially expanded.
    /// </summary>
    [Parameter] public bool Expanded { get; set; }

    /// <summary>
    /// If true, the menu item can be expanded to show a submenu.
    /// </summary>
    [Parameter] public bool Expandable { get; set; } = true;

    /// <summary>
    /// The direction (horizontal or vertical) of the navbar, provided via cascading parameter.
    /// </summary>
    [CascadingParameter(Name = "Direction")] public NavbarDirection Direction { get; set; }

    /// <summary>
    /// Indicates whether this menu item is a top-level item (not a submenu).
    /// </summary>
    public bool IsTopMenuItem => ParentMenuItem == null;

    /// <summary>
    /// The HTML tag used for rendering the menu item container.
    /// </summary>
    protected static string HtmlTag => "li";

    /// <summary>
    /// Indicates whether the menu item is currently expanded.
    /// </summary>
    protected bool IsExpanded { get; set; }

    /// <summary>
    /// Indicates whether this menu item is a dropdown (has a submenu and is expandable).
    /// </summary>
    protected bool IsDropdown => SubMenu != null && Expandable;

    /// <summary>
    /// Indicates whether this menu item is a submenu (has a parent menu item).
    /// </summary>
    protected bool IsSubMenu => ParentMenuItem != null;

    /// <summary>
    /// Determines if the navbar is horizontal and uses a dark background.
    /// Used for styling dropdowns.
    /// </summary>
    private bool NavbarIsHorizontalAndDark => Navbar?.Background == NavbarBackground.Dark && Navbar?.Direction == NavbarDirection.Horizontal;

    /// <summary>
    /// Determines if the dropdown should open to the end (right) in horizontal navbars.
    /// </summary>
    private bool IsDropEnd => Navbar?.Direction == NavbarDirection.Horizontal && ParentMenuItem?.IsDropdown == true;

    /// <summary>
    /// Tracks the current navigation path for active state detection.
    /// </summary>
    private string? _currentPath;

    /// <summary>
    /// Builds the CSS class string for the menu item based on its state and type.
    /// </summary>
    protected override string ClassNames => ClassBuilder
        .Add("nav-item")
        .Add("cursor-pointer")
        .AddIf("dropdown", IsDropdown && !IsDropEnd)
        .AddIf("dropend", IsDropdown && IsDropEnd)
        .AddIf("active", IsActive(BaseHref ?? Href ?? string.Empty))
        .ToString();

    /// <summary>
    /// Initializes the menu item, registers it with the navbar, and subscribes to navigation events.
    /// Expands the menu if it or any descendant is active.
    /// </summary>
    protected override void OnInitialized()
    {
        IsExpanded = Expanded || IsDescendantActive();
        Navbar?.AddNavbarMenuItem(this);

        if (NavigationManager != null)
        {
            _currentPath = $"/{NavigationManager.ToBaseRelativePath(NavigationManager.Uri)}";
            NavigationManager.LocationChanged += OnLocationChanged;

            // Expand the hierarchy if this item or its descendants are active
            if (IsDescendantActive())
                ExpandHierarchy();
        }
    }

    /// <summary>
    /// Closes the dropdown menu for this item.
    /// </summary>
    public void CloseDropdown()
    {
        IsExpanded = false;
    }

    /// <summary>
    /// Toggles the expanded/collapsed state of the dropdown menu.
    /// Closes all other top-level dropdowns if this is a top-level item.
    /// </summary>
    public void ToggleDropdown()
    {
        var expand = !IsExpanded;

        if (expand && IsTopMenuItem)
            Navbar?.CloseAll();

        IsExpanded = expand;
    }

    /// <summary>
    /// Unregisters the menu item from the navbar and unsubscribes from navigation events.
    /// </summary>
    public void Dispose()
    {
        Navbar?.RemoveNavbarMenuItem(this);

        if (NavigationManager != null)
            NavigationManager.LocationChanged -= OnLocationChanged;

        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Handles navigation changes to update the active state and expand the menu hierarchy if needed.
    /// </summary>
    private void OnLocationChanged(object? sender, LocationChangedEventArgs e)
    {
        if (NavigationManager != null)
        {
            _currentPath = $"/{NavigationManager.ToBaseRelativePath(e.Location)}";

            if (IsDescendantActive())
                ExpandHierarchy();

            StateHasChanged();
        }
    }

    /// <summary>
    /// Determines if the menu item is active based on the current navigation path.
    /// Supports both exact and prefix matching.
    /// </summary>
    /// <param name="matchingHref">The href to match against the current path.</param>
    /// <returns>True if the menu item is active; otherwise, false.</returns>
    private bool IsActive(string matchingHref)
    {
        if (_currentPath == null)
            return false;

        var isExactMatch = _currentPath == matchingHref;

        var isPrefixMatch = matchingHref.Length > 0 &&
                             !string.IsNullOrEmpty(matchingHref[1..]) &&
                             _currentPath[1..].StartsWith(matchingHref[1..]);

        return isExactMatch || isPrefixMatch;
    }

    /// <summary>
    /// Expands this menu item and all its parent menu items (for vertical navbars).
    /// </summary>
    private void ExpandHierarchy()
    {
        if (Direction == NavbarDirection.Horizontal) return;

        IsExpanded = true;

        ParentMenuItem?.ExpandHierarchy();
    }

    /// <summary>
    /// Determines if this menu item or any of its submenu descendants is active.
    /// </summary>
    /// <returns>True if this item or a descendant is active; otherwise, false.</returns>
    private bool IsDescendantActive()
    {
        if (IsActive(BaseHref ?? Href ?? string.Empty))
            return true;

        var submenuItems = Navbar?.GetSubmenuItems(this);
        return submenuItems?.Any(item => item.IsActive(BaseHref ?? Href ?? string.Empty)) ?? false;
    }
}