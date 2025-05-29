namespace LLSFramework.Application.Auth;

/// <summary>
/// Provides functionality for generating and validating JWT (JSON Web Token) tokens
/// using application-specific settings.
/// </summary>
public class JwtTokenManager(IOptions<JwtSettings> jwtSettings)
{
    /// <summary>
    /// Generates a new JWT token for the specified <see cref="ClaimsPrincipal"/>.
    /// </summary>
    /// <param name="claimsPrincipal">The claims principal containing user claims to embed in the token.</param>
    /// <param name="persistent">
    /// If <c>true</c>, uses the persistent expiration time; otherwise, uses the standard expiration time.
    /// </param>
    /// <returns>The generated JWT token as a string.</returns>
    public string GenerateNewToken(ClaimsPrincipal claimsPrincipal, bool persistent)
    {
        var expirationMinutes = persistent
            ? jwtSettings.Value.PersistentExpirationMinutes
            : jwtSettings.Value.ExpirationMinutes;

        var tokenValidationParameters = GetTokenValidationParameters(jwtSettings.Value);

        var token = new JwtSecurityToken(
            issuer: tokenValidationParameters.ValidIssuer,
            audience: tokenValidationParameters.ValidAudience,
            claims: claimsPrincipal.Claims,
            expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
            signingCredentials: new SigningCredentials(tokenValidationParameters.IssuerSigningKey, SecurityAlgorithms.HmacSha256)
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    /// <summary>
    /// Validates the specified JWT token and returns the associated <see cref="ClaimsPrincipal"/> if valid.
    /// </summary>
    /// <param name="token">The JWT token to validate.</param>
    /// <returns>The <see cref="ClaimsPrincipal"/> extracted from the token if validation succeeds.</returns>
    /// <exception cref="SecurityTokenException">Thrown if the token is invalid or expired.</exception>
    public ClaimsPrincipal ValidateToken(string token)
    {
        var tokenValidationParameters = GetTokenValidationParameters(jwtSettings.Value);

        return new JwtSecurityTokenHandler().ValidateToken(token, tokenValidationParameters, out _);
    }

    /// <summary>
    /// Creates and returns <see cref="TokenValidationParameters"/> based on the provided <see cref="JwtSettings"/>.
    /// </summary>
    /// <param name="jwtSettings">The JWT settings to use for validation parameters.</param>
    /// <returns>A configured <see cref="TokenValidationParameters"/> instance.</returns>
    /// <exception cref="Exception">Thrown if the secret key is null.</exception>
    public static TokenValidationParameters GetTokenValidationParameters(JwtSettings jwtSettings)
    {
        return new TokenValidationParameters
        {
            ValidIssuer = jwtSettings.ValidIssuer,
            ValidAudience = jwtSettings.ValidAudience,
            ClockSkew = TimeSpan.Zero,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey
                ?? throw new Exception("JwtSettings:SecretKey is null")))
        };
    }
}