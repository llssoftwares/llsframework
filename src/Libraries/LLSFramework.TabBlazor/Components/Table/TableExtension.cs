namespace LLSFramework.TabBlazor.Components.Table;

/// <summary>
/// Extension methods for converting <see cref="TableChangedEventArgs"/> to pagination and sorting options.
/// These helpers simplify mapping UI table state to backend query/filter objects.
/// </summary>
public static class TableExtensions
{
    /// <summary>
    /// Converts <see cref="TableChangedEventArgs"/> to a <see cref="PaginationOptions"/> instance.
    /// Maps the current page number and page size from the table event.
    /// </summary>
    /// <param name="e">The table change event arguments.</param>
    /// <returns>A <see cref="PaginationOptions"/> object with the current paging state.</returns>
    public static PaginationOptions ToPaginationOptions(this TableChangedEventArgs e)
    {
        return new(e.PageNumber, e.PageSize);
    }

    /// <summary>
    /// Converts <see cref="TableChangedEventArgs"/> to a <see cref="SortOptions"/> instance.
    /// Maps the current sort column and sort direction from the table event.
    /// </summary>
    /// <param name="e">The table change event arguments.</param>
    /// <returns>A <see cref="SortOptions"/> object with the current sorting state.</returns>
    public static SortOptions ToSortOptions(this TableChangedEventArgs e)
    {
        return new(e.SortColumn, e.SortDirection);
    }
}