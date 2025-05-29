namespace LLSFramework.TabBlazor.Components.Table;

public static class EntityFilterExtensions
{
    public static void SetOptions(this EntityFilter domainFilter, TableChangedEventArgs eventArgs)
    {
        domainFilter.PaginationOptions = eventArgs.ToPaginationOptions();
        domainFilter.SortOptions = eventArgs.ToSortOptions();
    }
}