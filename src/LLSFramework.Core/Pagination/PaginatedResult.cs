namespace LLSFramework.Core.Pagination;

/// <summary>
/// Represents a paginated result set, containing a list of items and the total count of items available.
/// </summary>
/// <typeparam name="TSource">The type of the items in the result list.</typeparam>
public class PaginatedResult<TSource>
{
    /// <summary>
    /// Gets the list of items for the current page.
    /// </summary>
    public List<TSource> List { get; init; } = [];

    /// <summary>
    /// Gets the total number of items available (across all pages).
    /// </summary>
    public int Total { get; init; }

    /// <summary>
    /// Maps the items in the current paginated result to a new type using the specified selector function.
    /// The total count is preserved.
    /// </summary>
    /// <typeparam name="TDestination">The type to map the items to.</typeparam>
    /// <param name="selector">A function to map each item from <typeparamref name="TSource"/> to <typeparamref name="TDestination"/>.</param>
    /// <returns>A new <see cref="PaginatedResult{TDestination}"/> with the mapped items and the same total count.</returns>
    public PaginatedResult<TDestination> Map<TDestination>(Func<TSource, TDestination> selector)
    {
        return new PaginatedResult<TDestination>
        {
            List = [.. List.Select(selector)],
            Total = Total
        };
    }
}