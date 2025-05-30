namespace LLSFramework.Application.Auth;

/// <summary>
/// Represents configuration settings for JWT (JSON Web Token) authentication.
/// </summary>
public class JwtSettings
{
    /// <summary>
    /// Gets or sets the name of the token, typically used as the cookie or header name.
    /// Default is "auth_token".
    /// </summary>
    public string TokenName { get; set; } = "auth_token";

    /// <summary>
    /// Gets or sets the valid issuer for the JWT.
    /// This should match the issuer configured in the token generation logic.
    /// </summary>
    public string? ValidIssuer { get; set; }

    /// <summary>
    /// Gets or sets the valid audience for the JWT.
    /// This should match the audience expected by the application.
    /// </summary>
    public string? ValidAudience { get; set; }

    /// <summary>
    /// Gets or sets the secret key used to sign the JWT.
    /// This value should be kept secure and never exposed publicly.
    /// </summary>
    public string? SecretKey { get; set; }

    /// <summary>
    /// Gets or sets the expiration time in minutes for the JWT.
    /// Determines how long a token is valid before requiring renewal.
    /// </summary>
    public int ExpirationMinutes { get; set; } = 30;

    /// <summary>
    /// Gets or sets the expiration time in minutes for persistent JWT tokens.
    /// Used for "remember me" or long-lived authentication scenarios.
    /// </summary>
    public int PersistentExpirationMinutes { get; set; } = 30;
}