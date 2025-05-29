using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;

namespace LLSFramework.TabBlazor.Components.SignalR;

public abstract class SignalRComponentBase<THub> : ComponentBase, IAsyncDisposable where THub : Hub
{
    protected HubConnection? HubConnection { get; private set; }

    [Inject] protected NavigationManager InternalNavigationManager { get; set; } = default!;

    [Inject] protected IConfiguration Configuration { get; set; } = default!;

    [Inject] protected BlazorJwtTokenManager BlazorJwtTokenManager { get; set; } = default!;

    [Parameter] public string? GroupId { get; set; }

    protected virtual void ConfigureHubConnection(IHubConnectionBuilder builder)
    {
    }

    protected virtual Task OnConnectedAsync() => Task.CompletedTask;

    protected virtual Task OnDisconnectedAsync(Exception? ex) => Task.CompletedTask;

    protected override async Task OnInitializedAsync()
    {
        var hubEndpoint = await GetHubEndpointAsync();

        var builder = new HubConnectionBuilder()
            .WithUrl(InternalNavigationManager.ToAbsoluteUri(hubEndpoint))
            .WithAutomaticReconnect();

        ConfigureHubConnection(builder);

        HubConnection = builder.Build();

        HubConnection.Reconnecting += error =>
        {
            Console.WriteLine("Reconnecting...");
            return Task.CompletedTask;
        };

        HubConnection.Reconnected += connectionId =>
        {
            Console.WriteLine("Reconnected");
            return OnConnectedAsync();
        };

        HubConnection.Closed += error =>
        {
            Console.WriteLine("Disconnected");
            return OnDisconnectedAsync(error);
        };

        await HubConnection.StartAsync();
        await OnConnectedAsync();
    }

    public virtual async ValueTask DisposeAsync()
    {
        if (HubConnection is not null)
            await HubConnection.DisposeAsync();
    }

    protected void On(string methodName, Action handler)
    {
        HubConnection?.On(methodName, handler);
    }

    protected void On<T>(string methodName, Action<T> handler)
    {
        HubConnection?.On(methodName, handler);
    }

    protected void On<T1, T2>(string methodName, Action<T1, T2> handler)
    {
        HubConnection?.On(methodName, handler);
    }

    protected Task SendAsync(string methodName, params object[] args)
    {
        return HubConnection?.SendAsync(methodName, args) ?? Task.CompletedTask;
    }

    private async Task<string> GetHubEndpointAsync()
    {
        var hubName = typeof(THub).Name.Replace("Hub", "").ToLower();
        var hubsBaseUrl = Configuration.GetSection("SignalR:HubsBaseUrl").Value ?? "/hubs";
        var jwtToken = await BlazorJwtTokenManager.GetTokenAsync();

        return $"{hubsBaseUrl}/{hubName}?access_token={jwtToken}&groupId={GroupId}";
    }
}

