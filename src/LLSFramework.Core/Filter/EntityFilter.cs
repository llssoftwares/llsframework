namespace LLSFramework.Core.Filter;

/// <summary>
/// Represents a base filter for entities, supporting pagination and sorting options.
/// Provides a method to check if the filter contains any meaningful criteria.
/// </summary>
public class EntityFilter
{
    /// <summary>
    /// Gets or sets the pagination options for the filter.
    /// </summary>
    public PaginationOptions PaginationOptions { get; set; } = new();

    /// <summary>
    /// Gets or sets the sorting options for the filter.
    /// </summary>
    public SortOptions SortOptions { get; set; } = new();

    /// <summary>
    /// Determines whether the filter contains any non-default or non-empty values.
    /// </summary>
    /// <returns>True if the filter is empty; otherwise, false.</returns>
    public bool IsEmpty()
    {
        return IsObjectEmpty(this);
    }

    /// <summary>
    /// Checks if all public instance properties of the given object are null, empty, or default.
    /// Strings must be null or empty, collections must be empty, and DateTime is considered non-empty.
    /// </summary>
    /// <param name="obj">The object to check.</param>
    /// <returns>True if the object is considered empty; otherwise, false.</returns>
    protected static bool IsObjectEmpty(object obj)
    {
        if (obj == null)
            return true;

        foreach (var property in obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            var value = property.GetValue(obj);

            if (value is null)
                continue;

            if (value is string str && !string.IsNullOrEmpty(str))
                return false;

            if (value is DateTime)
                return false;

            if (value is IEnumerable enumerable && enumerable.Cast<object>().Any())
                return false;
        }

        return true;
    }
}