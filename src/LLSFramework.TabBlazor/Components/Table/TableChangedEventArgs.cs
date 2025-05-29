namespace LLSFramework.TabBlazor.Components.Table;

public class TableChangedEventArgs
{
    public int PageNumber { get; set; }

    public int PageSize { get; set; }

    public string SortColumn { get; set; } = string.Empty;

    public SortDirection SortDirection { get; set; }
}