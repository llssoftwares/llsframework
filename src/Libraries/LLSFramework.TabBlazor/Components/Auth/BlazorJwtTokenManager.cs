namespace LLSFramework.TabBlazor.Components.Auth;

/// <summary>
/// Extends <see cref="JwtTokenManager"/> to provide JWT token management
/// integrated with Blazor's local storage abstraction.
/// Handles storing, retrieving, and deleting JWT tokens in local storage.
/// </summary>
public class BlazorJwtTokenManager(IOptions<JwtSettings> jwtSettings, ILocalStorage localStorage) : JwtTokenManager(jwtSettings)
{
    // Holds the JWT settings used for token operations.
    private readonly JwtSettings _jwtSettings = jwtSettings.Value;

    /// <summary>
    /// Asynchronously retrieves the JWT token from local storage.
    /// </summary>
    /// <returns>
    /// The JWT token as a string if found; otherwise, <c>null</c>.
    /// </returns>
    public async Task<string?> GetTokenAsync()
    {
        try
        {
            // Attempt to get the token from local storage using the configured token name.
            return await localStorage.GetAsync<string?>(_jwtSettings.TokenName);
        }
        catch
        {
            // Return null if retrieval fails (e.g., storage unavailable).
            return null;
        }
    }

    /// <summary>
    /// Asynchronously stores the specified JWT token in local storage.
    /// </summary>
    /// <param name="token">The JWT token to store.</param>
    public async Task SetTokenAsync(string token)
    {
        try
        {
            // Store the token in local storage under the configured token name.
            await localStorage.SetAsync(_jwtSettings.TokenName, token);
        }
        catch { /* Ignore storage errors */ }
    }

    /// <summary>
    /// Asynchronously deletes the JWT token from local storage.
    /// </summary>
    public async Task DeleteTokenAsync()
    {
        try
        {
            // Remove the token from local storage using the configured token name.
            await localStorage.DeleteAsync(_jwtSettings.TokenName);
        }
        catch { /* Ignore storage errors */ }
    }
}
