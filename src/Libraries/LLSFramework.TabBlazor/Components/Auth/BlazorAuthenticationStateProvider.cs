namespace LLSFramework.TabBlazor.Components.Auth;

/// <summary>
/// Provides an <see cref="AuthenticationStateProvider"/> implementation for Blazor
/// that uses JWT tokens managed via <see cref="BlazorJwtTokenManager"/>.
/// Handles authentication state retrieval, token validation, and token refresh logic.
/// </summary>
public class BlazorAuthenticationStateProvider(BlazorJwtTokenManager blazorJwtTokenManager) : AuthenticationStateProvider
{
    /// <summary>
    /// Asynchronously gets the current authentication state.
    /// Retrieves the JWT token from storage, validates it, refreshes the token,
    /// and returns the corresponding <see cref="AuthenticationState"/>.
    /// If the token is missing or invalid, returns an unauthenticated state.
    /// </summary>
    /// <returns>
    /// A <see cref="Task{AuthenticationState}"/> representing the asynchronous operation.
    /// </returns>
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            // Attempt to retrieve the JWT token from local storage.
            var token = await blazorJwtTokenManager.GetTokenAsync();
            if (token == null) return new AuthenticationState(new());

            // Validate the token and extract the user's claims.
            var claimsPrincipal = blazorJwtTokenManager.ValidateToken(token);

            // Optionally refresh the token to extend its validity.
            var refreshedToken = blazorJwtTokenManager.GenerateNewToken(claimsPrincipal, false);
            await blazorJwtTokenManager.SetTokenAsync(refreshedToken);

            // Return the authenticated state with the user's claims.
            return new AuthenticationState(claimsPrincipal);
        }
        catch
        {
            // If token validation fails, remove the token and return an unauthenticated state.
            await blazorJwtTokenManager.DeleteTokenAsync();
            return new AuthenticationState(new());
        }
    }

    /// <summary>
    /// Notifies the authentication system that the authentication state has changed.
    /// Triggers re-evaluation of the current authentication state.
    /// </summary>
    public void Notify()
    {
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }
}
