namespace LLSFramework.TabBlazor.Components.Navbar;

public partial class LLSNavbar : TablerBaseComponent, IDisposable
{
    [Inject] private NavigationManager? NavigationManager { get; set; }

    [Parameter] public NavbarBackground Background { get; set; }

    [Parameter] public NavbarDirection Direction { get; set; }

    public bool IsExpanded = false;

    protected static string HtmlTag => "div";

    private readonly List<LLSNavbarMenuItem> _navbarItems = [];

    protected override void OnInitialized()
    {
        if (NavigationManager != null)
            NavigationManager.LocationChanged += LocationChanged;

        base.OnInitialized();
    }

    private void LocationChanged(object? sender, LocationChangedEventArgs e)
    {
        if (Direction == NavbarDirection.Horizontal)
            CloseAll();
    }

    protected override string ClassNames => ClassBuilder
        .Add("navbar navbar-expand-md")
        .AddIf("navbar-vertical", Direction == NavbarDirection.Vertical)
        .ToString();

    public void ToogleExpand()
    {
        IsExpanded = !IsExpanded;
    }

    public void CloseAll()
    {
        foreach (var item in _navbarItems.Where(e => e.IsTopMenuItem))
        {
            item.CloseDropdown();
        }

        StateHasChanged();
    }

    public void AddNavbarMenuItem(LLSNavbarMenuItem item)
    {
        if (!_navbarItems.Contains(item))
            _navbarItems.Add(item);
    }

    public void RemoveNavbarMenuItem(LLSNavbarMenuItem item)
    {
        _navbarItems.Remove(item);
    }

    public IEnumerable<LLSNavbarMenuItem> GetSubmenuItems(LLSNavbarMenuItem parentMenuItem)
    {
        return _navbarItems.Where(item => item.ParentMenuItem == parentMenuItem);
    }

    public void Dispose()
    {
        if (NavigationManager != null)
            NavigationManager.LocationChanged -= LocationChanged;

        GC.SuppressFinalize(this);
    }
}