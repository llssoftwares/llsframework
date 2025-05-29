namespace LLSFramework.TabBlazor.Components.Table;

public static class TableExtensions
{
    public static PaginationOptions ToPaginationOptions(this TableChangedEventArgs e)
    {
        return new(e.PageNumber, e.PageSize);
    }

    public static SortOptions ToSortOptions(this TableChangedEventArgs e)
    {
        return new(e.SortColumn, e.SortDirection);
    }
}