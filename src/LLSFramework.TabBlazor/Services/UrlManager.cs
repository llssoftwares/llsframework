using LLSFramework.Core.Parsers;

namespace LLSFramework.TabBlazor.Services;

public class UrlManager(IJSRuntime jSRuntime)
{    
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

    // Remove parâmetros da URL atual
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
    
    public async Task<T?> GetParameterAsync<T>(string key)
    {
        var currentUrl = await jSRuntime.InvokeAsync<string>("urlManager.getCurrentUrl");

        var uri = new Uri(currentUrl);

        return !QueryHelpers.ParseQuery(uri.Query).TryGetValue(key, out var valueFromQueryString)
            ? default
            : GenericParser.Parse<T>(valueFromQueryString.ToString());
    }
    
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
    
    private async Task BindFilterOptions<T>(T model) where T : EntityFilter
    {
        var pageNumber = await GetParameterAsync<int?>("p") ?? 1;
        var sortColumn = await GetParameterAsync<string?>("sc");
        var sortDirection = await GetParameterAsync<SortDirection?>("sd") ?? SortDirection.Ascending;

        model.PaginationOptions = new PaginationOptions(pageNumber, PageSize: 5);

        if (!string.IsNullOrEmpty(sortColumn))
            model.SortOptions = new SortOptions(SortColumn: sortColumn, sortDirection);
    }
    
    public async Task AddParametersFromFilterAsync<T>(T model) where T : EntityFilter
    {
        var parameters = model.GetParametersFromFilter();

        await AddParametersAsync([.. parameters]);
    }
}