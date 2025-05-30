namespace LLSFramework.TabBlazor.Components.Table;

/// <summary>
/// Represents event data for table state changes such as paging and sorting.
/// Used to communicate the current table view state (page, size, sort column, and direction)
/// between UI components and data providers.
/// </summary>
public class TableChangedEventArgs
{
    /// <summary>
    /// Gets or sets the current page number being viewed.
    /// </summary>
    public int PageNumber { get; set; }

    /// <summary>
    /// Gets or sets the number of items displayed per page.
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// Gets or sets the name of the column by which the table is sorted.
    /// </summary>
    public string SortColumn { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the direction in which the table is sorted (ascending or descending).
    /// </summary>
    public SortDirection SortDirection { get; set; }
}
