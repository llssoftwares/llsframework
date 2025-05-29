namespace LLSFramework.Application.Auth;

/// <summary>
/// Represents the identity and authentication details of an application user.
/// </summary>
public class AppIdentity
{
    /// <summary>
    /// Gets or sets the unique identifier for the user.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the display name of the user.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the email address associated with the user.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the date and time of the user's last access.
    /// </summary>
    public DateTime LastAccess { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the user is authenticated.
    /// </summary>
    public bool IsAuthenticated { get; set; }

    /// <summary>
    /// Gets or sets the list of roles assigned to the user.
    /// </summary>
    public List<string> Roles { get; set; } = [];
}