using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;

namespace LLSFramework.TabBlazor.Components.SignalR;

/// <summary>
/// Abstract base class for Blazor components that interact with a SignalR hub.
/// Handles hub connection lifecycle, authentication, and event wiring for derived components.
/// </summary>
/// <typeparam name="THub">The SignalR hub type this component connects to.</typeparam>
public abstract class SignalRComponentBase<THub> : ComponentBase, IAsyncDisposable where THub : Hub
{
    /// <summary>
    /// The SignalR hub connection instance for this component.
    /// </summary>
    protected HubConnection? HubConnection { get; private set; }

    /// <summary>
    /// Provides navigation and URL management for the application.
    /// </summary>
    [Inject] protected NavigationManager InternalNavigationManager { get; set; } = default!;

    /// <summary>
    /// Provides access to application configuration settings.
    /// </summary>
    [Inject] protected IConfiguration Configuration { get; set; } = default!;

    /// <summary>
    /// Provides JWT token management for authenticating SignalR connections.
    /// </summary>
    [Inject] protected BlazorJwtTokenManager BlazorJwtTokenManager { get; set; } = default!;

    /// <summary>
    /// Optional group identifier to join a specific SignalR group.
    /// </summary>
    [Parameter] public string? GroupId { get; set; }

    /// <summary>
    /// Allows derived classes to further configure the hub connection builder (e.g., add handlers).
    /// </summary>
    /// <param name="builder">The hub connection builder to configure.</param>
    protected virtual void ConfigureHubConnection(IHubConnectionBuilder builder)
    {
    }

    /// <summary>
    /// Called when the hub connection is established.
    /// Override to perform actions after connecting.
    /// </summary>
    protected virtual Task OnConnectedAsync() => Task.CompletedTask;

    /// <summary>
    /// Called when the hub connection is closed or lost.
    /// Override to perform actions after disconnecting.
    /// </summary>
    /// <param name="ex">The exception that caused the disconnect, if any.</param>
    protected virtual Task OnDisconnectedAsync(Exception? ex) => Task.CompletedTask;

    /// <summary>
    /// Initializes the SignalR hub connection, configures authentication, and starts the connection.
    /// </summary>
    protected override async Task OnInitializedAsync()
    {
        var hubEndpoint = await GetHubEndpointAsync();

        var builder = new HubConnectionBuilder()
            .WithUrl(InternalNavigationManager.ToAbsoluteUri(hubEndpoint))
            .WithAutomaticReconnect();

        ConfigureHubConnection(builder);

        HubConnection = builder.Build();

        // Register connection lifecycle event handlers
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

    /// <summary>
    /// Disposes the hub connection and releases resources.
    /// </summary>
    public virtual async ValueTask DisposeAsync()
    {
        if (HubConnection is not null)
            await HubConnection.DisposeAsync();

        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Registers a handler for a SignalR hub method with no parameters.
    /// </summary>
    /// <param name="methodName">The name of the hub method.</param>
    /// <param name="handler">The action to invoke when the method is called.</param>
    protected void On(string methodName, Action handler)
    {
        HubConnection?.On(methodName, handler);
    }

    /// <summary>
    /// Registers a handler for a SignalR hub method with one parameter.
    /// </summary>
    /// <typeparam name="T">The type of the parameter.</typeparam>
    /// <param name="methodName">The name of the hub method.</param>
    /// <param name="handler">The action to invoke when the method is called.</param>
    protected void On<T>(string methodName, Action<T> handler)
    {
        HubConnection?.On(methodName, handler);
    }

    /// <summary>
    /// Registers a handler for a SignalR hub method with two parameters.
    /// </summary>
    /// <typeparam name="T1">The type of the first parameter.</typeparam>
    /// <typeparam name="T2">The type of the second parameter.</typeparam>
    /// <param name="methodName">The name of the hub method.</param>
    /// <param name="handler">The action to invoke when the method is called.</param>
    protected void On<T1, T2>(string methodName, Action<T1, T2> handler)
    {
        HubConnection?.On(methodName, handler);
    }

    /// <summary>
    /// Invokes a hub method on the server with the specified arguments.
    /// </summary>
    /// <param name="methodName">The name of the hub method to invoke.</param>
    /// <param name="args">Arguments to pass to the hub method.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    protected Task SendAsync(string methodName, params object[] args)
    {
        return HubConnection?.SendAsync(methodName, args) ?? Task.CompletedTask;
    }

    /// <summary>
    /// Constructs the SignalR hub endpoint URL, including JWT authentication and group ID if provided.
    /// </summary>
    /// <returns>The full hub endpoint URL as a string.</returns>
    private async Task<string> GetHubEndpointAsync()
    {
        var hubName = typeof(THub).Name.Replace("Hub", "").ToLower();
        var hubsBaseUrl = Configuration.GetSection("SignalR:HubsBaseUrl").Value ?? "/hubs";
        var jwtToken = await BlazorJwtTokenManager.GetTokenAsync();

        return $"{hubsBaseUrl}/{hubName}?access_token={jwtToken}&groupId={GroupId}";
    }
}