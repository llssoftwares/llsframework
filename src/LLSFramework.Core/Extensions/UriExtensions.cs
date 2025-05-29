namespace LLSFramework.Core.Extensions;

/// <summary>
/// Provides extension methods for manipulating query parameters in <see cref="Uri"/> instances.
/// </summary>
public static class UriExtensions
{
    /// <summary>
    /// Retrieves the value of a specified query parameter from the URI.
    /// </summary>
    /// <param name="url">The URI to search.</param>
    /// <param name="paramName">The name of the query parameter.</param>
    /// <returns>The value of the parameter if found; otherwise, null.</returns>
    public static string? GetParameter(this Uri url, string paramName)
    {
        if (paramName == null) return null;

        var uriBuilder = new UriBuilder(url);
        var query = HttpUtility.ParseQueryString(uriBuilder.Query);

        return query.AllKeys.Contains(paramName) ? query[paramName] : null;
    }

    /// <summary>
    /// Adds a new query parameter to the URI. If the parameter already exists, it is added again.
    /// </summary>
    /// <param name="url">The original URI.</param>
    /// <param name="paramName">The name of the parameter to add.</param>
    /// <param name="paramValue">The value of the parameter to add.</param>
    /// <returns>A new URI with the added parameter.</returns>
    public static Uri AddParameter(this Uri url, string paramName, object paramValue)
    {
        if (paramValue == null) return url;

        var uriBuilder = new UriBuilder(url);
        var query = HttpUtility.ParseQueryString(uriBuilder.Query);
        query.Add(paramName, paramValue.ToString());

        uriBuilder.Query = query.ToString();

        return uriBuilder.Uri;
    }

    /// <summary>
    /// Sets the value of an existing query parameter in the URI.
    /// If the parameter does not exist, no changes are made.
    /// </summary>
    /// <param name="url">The original URI.</param>
    /// <param name="paramName">The name of the parameter to set.</param>
    /// <param name="paramValue">The new value for the parameter.</param>
    /// <returns>A new URI with the updated parameter value.</returns>
    public static Uri SetParameter(this Uri url, string paramName, object paramValue)
    {
        if (paramValue == null) return url;

        var uriBuilder = new UriBuilder(url);
        var query = HttpUtility.ParseQueryString(uriBuilder.Query);

        if (query.AllKeys.Contains(paramName))
            query[paramName] = paramValue.ToString();

        uriBuilder.Query = query.ToString();

        return uriBuilder.Uri;
    }

    /// <summary>
    /// Removes a query parameter from the URI.
    /// </summary>
    /// <param name="url">The original URI.</param>
    /// <param name="paramName">The name of the parameter to remove.</param>
    /// <returns>A new URI without the specified parameter.</returns>
    public static Uri RemoveParameter(this Uri url, string paramName)
    {
        if (paramName == null) return url;

        var uriBuilder = new UriBuilder(url);
        var query = HttpUtility.ParseQueryString(uriBuilder.Query);

        query.Remove(paramName);

        uriBuilder.Query = query.ToString();

        return uriBuilder.Uri;
    }
}