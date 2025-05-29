namespace LLSFramework.TabBlazor.Components.Auth;

public class BlazorAuthenticationStateProvider(BlazorJwtTokenManager blazorJwtTokenManager) : AuthenticationStateProvider
{
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            var token = await blazorJwtTokenManager.GetTokenAsync();
            if (token == null) return new AuthenticationState(new());

            var claimsPrincipal = blazorJwtTokenManager.ValidateToken(token);
            var refreshedToken = blazorJwtTokenManager.GenerateNewToken(claimsPrincipal, false);
            await blazorJwtTokenManager.SetTokenAsync(refreshedToken);

            return new AuthenticationState(claimsPrincipal);
        }
        catch
        {
            await blazorJwtTokenManager.DeleteTokenAsync();
            return new AuthenticationState(new());
        }
    }

    public void Notify()
    {
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }
}