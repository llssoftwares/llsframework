namespace LLSFramework.TabBlazor.Components.Navbar;

/// <summary>
/// Represents a collapsible menu section within a navigation bar.
/// Integrates with <see cref="LLSNavbar"/> and supports both horizontal and vertical layouts.
/// </summary>
public partial class LLSNavbarMenu : TablerBaseComponent
{
    /// <summary>
    /// The parent navbar component, provided via cascading parameter.
    /// Used to access navbar state and methods.
    /// </summary>
    [CascadingParameter(Name = "Navbar")] LLSNavbar? Navbar { get; set; }

    /// <summary>
    /// The direction (horizontal or vertical) of the navbar, provided via cascading parameter.
    /// </summary>
    [CascadingParameter(Name = "Direction")] public NavbarDirection Direction { get; set; }

    /// <summary>
    /// The HTML tag used for rendering the menu container.
    /// </summary>
    protected static string HtmlTag => "div";

    /// <summary>
    /// Builds the CSS class string for the menu section based on its expanded state.
    /// Adds "collapse" if the menu is not expanded.
    /// </summary>
    protected override string ClassNames => ClassBuilder
          .Add("navbar-collapse")
          .AddIf("collapse", !IsExpanded)
          .ToString();

    /// <summary>
    /// Indicates whether the menu is currently expanded, based on the parent navbar's state.
    /// </summary>
    private bool IsExpanded => Navbar?.IsExpanded ?? false;

    /// <summary>
    /// Toggles the expanded/collapsed state of the parent navbar.
    /// </summary>
    public void ToogleExpanded()
    {
        Navbar?.ToogleExpand();
    }
}