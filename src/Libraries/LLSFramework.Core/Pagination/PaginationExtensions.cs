namespace LLSFramework.Core.Pagination;

/// <summary>
/// Provides extension methods for sorting, paginating, and converting query results to paginated results.
/// </summary>
public static class PaginationExtensions
{
    /// <summary>
    /// Applies dynamic sorting to an <see cref="IQueryable{T}"/> based on the specified <see cref="SortOptions"/>.
    /// If the sort column is not specified or does not exist, the original query is returned.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the queryable.</typeparam>
    /// <param name="queryable">The queryable to sort.</param>
    /// <param name="sortOptions">The sorting options, including column and direction.</param>
    /// <returns>The sorted <see cref="IQueryable{T}"/> if possible; otherwise, the original queryable.</returns>
    public static IQueryable<T> Sort<T>(this IQueryable<T> queryable, SortOptions sortOptions)
    {
        if (string.IsNullOrEmpty(sortOptions?.SortColumn)) return queryable;

        var propertyInfo = typeof(T).GetProperty(sortOptions.SortColumn);

        if (propertyInfo == null)
            return queryable;

        var parameter = Expression.Parameter(typeof(T));
        var propertyAccess = Expression.Property(parameter, propertyInfo);
        var lambda = Expression.Lambda(propertyAccess, parameter);
        
        var methodName = sortOptions.SortDirection == SortDirection.Descending 
            ? "OrderByDescending" 
            : "OrderBy";

        var orderByExpression = Expression.Call(
            typeof(Queryable),
            methodName,
            [typeof(T), propertyInfo.PropertyType],
            queryable.Expression,
            Expression.Quote(lambda)
        );

        return queryable.Provider.CreateQuery<T>(orderByExpression);
    }

    /// <summary>
    /// Applies pagination to an <see cref="IQueryable{T}"/> using the specified <see cref="PaginationOptions"/>.
    /// Skips and takes the appropriate number of items based on the page number and page size.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the queryable.</typeparam>
    /// <param name="queryable">The queryable to paginate.</param>
    /// <param name="paginationOptions">The pagination options, including page number and page size.</param>
    /// <returns>The paginated <see cref="IQueryable{T}"/>.</returns>
    public static IQueryable<T> Paginate<T>(this IQueryable<T> queryable, PaginationOptions? paginationOptions)
    {
        if (paginationOptions == null) return queryable;

        queryable = queryable.Skip((paginationOptions.PageNumber - 1) * paginationOptions.PageSize);
        return paginationOptions.PageSize > 0
            ? queryable.Take(paginationOptions.PageSize)
            : queryable;
    }

    /// <summary>
    /// Converts an <see cref="IEnumerable{T}"/> and a total count into a <see cref="PaginatedResult{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the items in the result.</typeparam>
    /// <param name="enumerable">The items for the current page.</param>
    /// <param name="total">The total number of items available (across all pages).</param>
    /// <returns>A <see cref="PaginatedResult{T}"/> containing the items and total count.</returns>
    public static PaginatedResult<T> ToPaginatedResult<T>(this IEnumerable<T> enumerable, int total)
        => new() { List = [.. enumerable], Total = total };
}