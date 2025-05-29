using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace LLSFramework.TabBlazor.Components.SignalR;

public class CustomUserIdProvider : IUserIdProvider
{
    public string? GetUserId(HubConnectionContext connection)
    {
        return connection.User?.FindFirst(ClaimTypes.Sid)?.Value;
    }
}