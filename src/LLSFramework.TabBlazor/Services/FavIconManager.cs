namespace LLSFramework.TabBlazor.Services;

public class FavIconManager(IJSRuntime jSRuntime)
{
    public async Task SetAsync(string faviconUrl)
    {
        try
        {
            await jSRuntime.InvokeVoidAsync("favIcon.set", faviconUrl);
        }
        catch { }
    }
}