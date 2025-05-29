namespace LLSFramework.TabBlazor.Components.Navbar;

public partial class LLSNavbarMenu : TablerBaseComponent
{
    [CascadingParameter(Name = "Navbar")] LLSNavbar? Navbar { get; set; }

    [CascadingParameter(Name = "Direction")] public NavbarDirection Direction { get; set; }

    protected static string HtmlTag => "div";

    protected override string ClassNames => ClassBuilder
          .Add("navbar-collapse")
          .AddIf("collapse", !IsExpanded)
          .ToString();

    private bool IsExpanded => Navbar?.IsExpanded ?? false;

    public void ToogleExpanded()
    {
        Navbar?.ToogleExpand();
    }
}