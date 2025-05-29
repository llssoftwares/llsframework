namespace LLSFramework.TabBlazor.Components.Auth;

public class BlazorJwtTokenManager(IOptions<JwtSettings> jwtSettings, ILocalStorage localStorage) : JwtTokenManager(jwtSettings)
{
    private readonly JwtSettings _jwtSettings = jwtSettings.Value;

    public async Task<string?> GetTokenAsync()
    {
        try
        {
            return await localStorage.GetAsync<string?>(_jwtSettings.TokenName);
        }
        catch
        {
            return null;
        }
    }

    public async Task SetTokenAsync(string token)
    {
        try
        {
            await localStorage.SetAsync(_jwtSettings.TokenName, token);
        }
        catch { }
    }

    public async Task DeleteTokenAsync()
    {
        try
        {
            await localStorage.DeleteAsync(_jwtSettings.TokenName);
        }
        catch { }
    }
}