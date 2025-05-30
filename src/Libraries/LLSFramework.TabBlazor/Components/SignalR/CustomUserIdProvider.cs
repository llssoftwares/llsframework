using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace LLSFramework.TabBlazor.Components.SignalR;

/// <summary>
/// Provides a custom user ID provider for SignalR hubs.
/// Uses the value of the ClaimTypes.Sid claim as the unique user identifier.
/// </summary>
public class CustomUserIdProvider : IUserIdProvider
{
    /// <summary>
    /// Returns the user identifier for the current SignalR connection.
    /// Uses the SID (Security Identifier) claim from the user's claims principal.
    /// </summary>
    /// <param name="connection">The SignalR hub connection context.</param>
    /// <returns>
    /// The value of the ClaimTypes.Sid claim if present; otherwise, null.
    /// </returns>
    public string? GetUserId(HubConnectionContext connection)
    {
        return connection.User?.FindFirst(ClaimTypes.Sid)?.Value;
    }
}