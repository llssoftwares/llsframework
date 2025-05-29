namespace LLSFramework.TabBlazor.Components.Table;

/// <summary>
/// Extension methods for <see cref="EntityFilter"/> to integrate with table events.
/// </summary>
public static class EntityFilterExtensions
{
    /// <summary>
    /// Updates the <see cref="EntityFilter"/>'s pagination and sorting options
    /// based on the provided <see cref="TableChangedEventArgs"/>.
    /// This allows the filter to reflect the current table state (page, size, sort).
    /// </summary>
    /// <param name="domainFilter">The filter to update.</param>
    /// <param name="eventArgs">The table event arguments containing pagination and sorting info.</param>
    public static void SetOptions(this EntityFilter domainFilter, TableChangedEventArgs eventArgs)
    {
        domainFilter.PaginationOptions = eventArgs.ToPaginationOptions();
        domainFilter.SortOptions = eventArgs.ToSortOptions();
    }
}