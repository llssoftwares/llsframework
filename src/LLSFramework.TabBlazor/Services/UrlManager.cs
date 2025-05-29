using LLSFramework.Core.Parsers;

namespace LLSFramework.TabBlazor.Services;

/// <summary>
/// Provides utility methods for manipulating the browser URL and query string parameters
/// using JavaScript interop. Supports adding, removing, and reading parameters, as well as
/// binding filter models to and from the URL.
/// </summary>
public class UrlManager(IJSRuntime jSRuntime)
{
    /// <summary>
    /// Adds or updates query string parameters in the current URL.
    /// If a value is null or empty, the parameter is removed.
    /// The browser URL is updated using JavaScript interop without reloading the page.
    /// </summary>
    /// <param name="parameters">Key-value pairs to add or update in the query string.</param>
    public async Task AddParametersAsync(params (string Key, object? Value)[] parameters)
    {
        var currentUrl = await jSRuntime.InvokeAsync<string>("urlManager.getCurrentUrl");

        var uri = new Uri(currentUrl);

        var queryString = HttpUtility.ParseQueryString(uri.Query);

        foreach (var (Key, Value) in parameters)
        {
            var stringValue = Value?.ToString();

            if (string.IsNullOrEmpty(stringValue))
                queryString.Remove(Key);
            else
            {
                queryString[Key] = stringValue;
            }
        }

        var newUrl = uri.GetLeftPart(UriPartial.Path);

        if (queryString.Count > 0)
            newUrl += "?" + queryString;

        if (currentUrl != newUrl)
            await jSRuntime.InvokeVoidAsync("urlManager.changeUrl", newUrl);
    }

    /// <summary>
    /// Removes specified query string parameters from the current URL.
    /// The browser URL is updated using JavaScript interop without reloading the page.
    /// </summary>
    /// <param name="keys">The parameter keys to remove from the query string.</param>
    public async Task RemoveParametersAsync(params string[] keys)
    {
        var currentUrl = await jSRuntime.InvokeAsync<string>("urlManager.getCurrentUrl");

        var uri = new Uri(currentUrl);

        var queryString = HttpUtility.ParseQueryString(uri.Query);

        foreach (var key in keys)
        {
            queryString.Remove(key);
        }

        var pagePathWithoutQueryString = uri.GetLeftPart(UriPartial.Path);

        var newUrl = queryString.Count > 0
            ? string.Format("{0}?{1}", pagePathWithoutQueryString, queryString)
            : pagePathWithoutQueryString;

        if (currentUrl != newUrl)
            await jSRuntime.InvokeVoidAsync("urlManager.changeUrl", newUrl);
    }

    /// <summary>
    /// Retrieves a query string parameter from the current URL and parses it to the specified type.
    /// Returns the default value of T if the parameter is not present or cannot be parsed.
    /// </summary>
    /// <typeparam name="T">The type to parse the parameter value to.</typeparam>
    /// <param name="key">The query string key to retrieve.</param>
    /// <returns>The parsed value, or default if not found or invalid.</returns>
    public async Task<T?> GetParameterAsync<T>(string key)
    {
        var currentUrl = await jSRuntime.InvokeAsync<string>("urlManager.getCurrentUrl");

        var uri = new Uri(currentUrl);

        return !QueryHelpers.ParseQuery(uri.Query).TryGetValue(key, out var valueFromQueryString)
            ? default
            : GenericParser.Parse<T>(valueFromQueryString.ToString());
    }

    /// <summary>
    /// Binds query string parameters from the current URL to the properties of an <see cref="EntityFilter"/> model.
    /// Only properties not declared in the base <see cref="EntityFilter"/> are considered.
    /// Properties marked with <c>FilterComputedAttribute</c> are ignored.
    /// </summary>
    /// <typeparam name="T">The filter model type, must inherit from <see cref="EntityFilter"/>.</typeparam>
    /// <param name="model">The filter model instance to bind values to.</param>
    public async Task BindFilterAsync<T>(T model) where T : EntityFilter
    {
        foreach (var property in typeof(T).GetProperties().Where(p => p.DeclaringType != typeof(EntityFilter)))
        {
            var propertyType = property.PropertyType;
            var getParameterMethod = typeof(UrlManager).GetMethod("GetParameterAsync")?.MakeGenericMethod(propertyType);

            if (getParameterMethod == null)
                continue;

            var filterQueryStringParamAttribute = property.GetCustomAttribute<FilterQueryStringParameterAttribute>();

            var filterComputedAttribute = property.GetCustomAttribute<FilterComputedAttribute>();

            if (filterComputedAttribute != null)
                continue;

            var paramName = !string.IsNullOrEmpty(filterQueryStringParamAttribute?.Name)
                ? filterQueryStringParamAttribute.Name
                : property.Name;

            var task = (Task)getParameterMethod.Invoke(this, [paramName])!;

            await task.ConfigureAwait(false);
            var resultProperty = task.GetType().GetProperty("Result");
            var value = resultProperty?.GetValue(task);

            if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(List<>))
                value ??= Activator.CreateInstance(propertyType);

            property.SetValue(model, value);
        }

        await BindFilterOptions(model);
    }

    /// <summary>
    /// Binds pagination and sorting options from the URL to the filter model.
    /// Looks for "p" (page number), "sc" (sort column), and "sd" (sort direction) parameters.
    /// </summary>
    /// <typeparam name="T">The filter model type.</typeparam>
    /// <param name="model">The filter model instance to update.</param>
    private async Task BindFilterOptions<T>(T model) where T : EntityFilter
    {
        var pageNumber = await GetParameterAsync<int?>("p") ?? 1;
        var sortColumn = await GetParameterAsync<string?>("sc");
        var sortDirection = await GetParameterAsync<SortDirection?>("sd") ?? SortDirection.Ascending;

        model.PaginationOptions = new PaginationOptions(pageNumber, PageSize: 5);

        if (!string.IsNullOrEmpty(sortColumn))
            model.SortOptions = new SortOptions(SortColumn: sortColumn, sortDirection);
    }

    /// <summary>
    /// Adds all filter parameters from an <see cref="EntityFilter"/> model to the URL query string.
    /// </summary>
    /// <typeparam name="T">The filter model type, must inherit from <see cref="EntityFilter"/>.</typeparam>
    /// <param name="model">The filter model instance to extract parameters from.</param>
    public async Task AddParametersFromFilterAsync<T>(T model) where T : EntityFilter
    {
        var parameters = model.GetParametersFromFilter();

        await AddParametersAsync([.. parameters]);
    }
}