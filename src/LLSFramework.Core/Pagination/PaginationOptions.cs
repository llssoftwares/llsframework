namespace LLSFramework.Core.Pagination;

/// <summary>
/// Represents pagination settings for queries, including the page number and page size.
/// </summary>
/// <param name="PageNumber">The current page number (1-based). Defaults to 1.</param>
/// <param name="PageSize">The number of items per page. Defaults to 5.</param>
public record PaginationOptions(int PageNumber = 1, int PageSize = 5);