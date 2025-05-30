namespace LLSFramework.Core.Pagination;

/// <summary>
/// Represents sorting options for queries, including the column to sort by and the sort direction.
/// </summary>
/// <param name="SortColumn">The name of the column to sort by. Defaults to an empty string (no sorting).</param>
/// <param name="SortDirection">The direction in which to sort the column. Defaults to <see cref="SortDirection.Ascending"/>.</param>
public record SortOptions(string SortColumn = "", SortDirection SortDirection = SortDirection.Ascending);