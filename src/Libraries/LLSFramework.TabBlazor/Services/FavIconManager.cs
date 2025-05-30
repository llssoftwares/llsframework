namespace LLSFramework.TabBlazor.Services;

/// <summary>
/// Provides functionality to update the browser's favicon dynamically from Blazor components.
/// Uses JavaScript interop to set the favicon URL at runtime.
/// </summary>
public class FavIconManager(IJSRuntime jSRuntime)
{
    /// <summary>
    /// Sets the browser's favicon to the specified URL using JavaScript interop.
    /// Calls the JavaScript function 'favIcon.set' with the provided favicon URL.
    /// </summary>
    /// <param name="faviconUrl">The URL of the favicon image to set.</param>
    public async Task SetAsync(string faviconUrl)
    {
        try
        {
            await jSRuntime.InvokeVoidAsync("favIcon.set", faviconUrl);
        }
        catch
        {
            // Swallow exceptions to avoid breaking the application if JS interop fails.
        }
    }
}