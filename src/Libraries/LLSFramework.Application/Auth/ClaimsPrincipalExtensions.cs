namespace LLSFramework.Application.Auth;

/// <summary>
/// Provides extension methods for <see cref="ClaimsPrincipal"/> to simplify authentication checks.
/// </summary>
public static class ClaimsPrincipalExtensions
{
    /// <summary>
    /// Determines whether the specified <see cref="ClaimsPrincipal"/> is authenticated.
    /// </summary>
    /// <param name="claimsPrincipal">The claims principal to check.</param>
    /// <returns>
    /// <c>true</c> if the principal is not null, has an identity, and the identity is authenticated; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsAuthenticated(this ClaimsPrincipal? claimsPrincipal)
    {
        return claimsPrincipal?.Identity != null && claimsPrincipal.Identity.IsAuthenticated;
    }
}