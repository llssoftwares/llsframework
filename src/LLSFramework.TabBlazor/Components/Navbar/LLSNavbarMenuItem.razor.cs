namespace LLSFramework.TabBlazor.Components.Navbar;

public partial class LLSNavbarMenuItem : TablerBaseComponent, IDisposable
{
    [Inject] NavigationManager? NavigationManager { get; set; }

    [CascadingParameter(Name = "Navbar")] LLSNavbar? Navbar { get; set; }

    [CascadingParameter(Name = "Parent")] public LLSNavbarMenuItem? ParentMenuItem { get; set; }

    [Parameter] public string? BaseHref { get; set; }

    [Parameter] public string? Href { get; set; }

    [Parameter] public string? Text { get; set; }

    [Parameter] public RenderFragment? MenuItemIcon { get; set; }

    [Parameter] public RenderFragment? SubMenu { get; set; }

    [Parameter] public bool Expanded { get; set; }

    [Parameter] public bool Expandable { get; set; } = true;

    [CascadingParameter(Name = "Direction")] public NavbarDirection Direction { get; set; }

    public bool IsTopMenuItem => ParentMenuItem == null;

    protected static string HtmlTag => "li";

    protected bool IsExpanded { get; set; }

    protected bool IsDropdown => SubMenu != null && Expandable;

    protected bool IsSubMenu => ParentMenuItem != null;

    private bool NavbarIsHorizontalAndDark => Navbar?.Background == NavbarBackground.Dark && Navbar?.Direction == NavbarDirection.Horizontal;

    private bool IsDropEnd => Navbar?.Direction == NavbarDirection.Horizontal && ParentMenuItem?.IsDropdown == true;

    private string? _currentPath;

    protected override string ClassNames => ClassBuilder
        .Add("nav-item")
        .Add("cursor-pointer")
        .AddIf("dropdown", IsDropdown && !IsDropEnd)
        .AddIf("dropend", IsDropdown && IsDropEnd)
        .AddIf("active", IsActive(BaseHref ?? Href ?? string.Empty))
        .ToString();

    protected override void OnInitialized()
    {
        IsExpanded = Expanded || IsDescendantActive();
        Navbar?.AddNavbarMenuItem(this);

        if (NavigationManager != null)
        {
            _currentPath = $"/{NavigationManager.ToBaseRelativePath(NavigationManager.Uri)}";
            NavigationManager.LocationChanged += OnLocationChanged;

            // Expande a hierarquia se o item ou seus descendentes estiverem ativos
            if (IsDescendantActive())
                ExpandHierarchy();
        }
    }

    public void CloseDropdown()
    {
        IsExpanded = false;
    }

    public void ToggleDropdown()
    {
        var expand = !IsExpanded;

        if (expand && IsTopMenuItem)
            Navbar?.CloseAll();

        IsExpanded = expand;
    }

    public void Dispose()
    {
        Navbar?.RemoveNavbarMenuItem(this);

        if (NavigationManager != null)
            NavigationManager.LocationChanged -= OnLocationChanged;

        GC.SuppressFinalize(this);
    }

    private void OnLocationChanged(object? sender, LocationChangedEventArgs e)
    {
        if (NavigationManager != null)
        {
            _currentPath = $"/{NavigationManager.ToBaseRelativePath(e.Location)}";

            // Expande a hierarquia se o item ou seus descendentes estiverem ativos
            if (IsDescendantActive())
                ExpandHierarchy();

            StateHasChanged();
        }
    }

    private bool IsActive(string matchingHref)
    {
        if (_currentPath == null)
            return false;

        // Verifica se o caminho atual é exatamente igual ao href fornecido
        var isExactMatch = _currentPath == matchingHref;

        // Verifica se o caminho atual começa com o href fornecido (ignora o primeiro caractere '/')
        var isPrefixMatch = matchingHref.Length > 0 &&
                             !string.IsNullOrEmpty(matchingHref[1..]) &&
                             _currentPath[1..].StartsWith(matchingHref[1..]);

        return isExactMatch || isPrefixMatch;
    }

    private void ExpandHierarchy()
    {
        // Não expande se a direção for horizontal
        if (Direction == NavbarDirection.Horizontal) return;

        // Expande o item atual
        IsExpanded = true;

        // Propaga a expansão para o pai
        ParentMenuItem?.ExpandHierarchy();
    }

    private bool IsDescendantActive()
    {
        // Verifica se o próprio item está ativo
        if (IsActive(BaseHref ?? Href ?? string.Empty))
            return true;

        // Verifica se algum item no submenu está ativo
        var submenuItems = Navbar?.GetSubmenuItems(this);
        return submenuItems?.Any(item => item.IsActive(BaseHref ?? Href ?? string.Empty)) ?? false;
    }
}

