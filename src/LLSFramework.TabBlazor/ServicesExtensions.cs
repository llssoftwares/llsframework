using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;

namespace LLSFramework.TabBlazor;

/// <summary>
/// Provides extension methods for registering LLSFramework.TabBlazor services and mapping SignalR hubs.
/// </summary>
public static class ServicesExtensions
{
    /// <summary>
    /// Registers all core services required by LLSFramework.TabBlazor into the DI container.
    /// Includes authentication, SignalR, UI utilities, and framework-specific services.
    /// </summary>
    /// <param name="services">The service collection to add to.</param>
    /// <returns>The updated service collection for chaining.</returns>
    public static IServiceCollection AddLLSFrameworkTabBlazor(this IServiceCollection services)
    {
        // Register SignalR for real-time communication support.
        services.AddSignalR();

        // Register framework and utility services as scoped or singleton dependencies.
        return services
            .AddScoped<AuthenticationStateProvider, BlazorAuthenticationStateProvider>() // Provides authentication state using JWT and Blazor integration.
            .AddScoped<BlazorJwtTokenManager>()      // Manages JWT tokens in Blazor/local storage.
            .AddScoped<FavIconManager>()             // Allows dynamic favicon updates via JS interop.
            .AddScoped<LoadingState>()               // Tracks and notifies loading state for UI components.
            .AddScoped<UrlManager>()                 // Manages URL/query string parameters and filter binding.
            .AddScoped<BlazorMediator>()             // Implements mediator pattern for in-app notifications/events.
            .AddScoped<ActionHandler>()              // Handles async actions with loading, toast, and modal feedback.
            .AddSingleton<IUserIdProvider, CustomUserIdProvider>() // Custom SignalR user ID provider using claims.
            .AddTabler()                             // Registers Tabler UI services.
            .AddResizeListener()                     // Registers resize event listener for responsive UI.
            .AddMediaQueryService();                 // Registers media query service for responsive UI.
        //.AddApexCharts(); // Optionally register ApexCharts if needed.
    }

    /// <summary>
    /// Dynamically maps all SignalR hub types found in the provided assemblies to endpoint routes.
    /// Uses the configured base URL and supports WebSockets and LongPolling transports.
    /// </summary>
    /// <param name="endpoints">The endpoint route builder to map hubs on.</param>
    /// <param name="configuration">The application configuration (for SignalR base URL).</param>
    /// <param name="assemblies">Assemblies to scan for SignalR hub types.</param>
    public static void MapHubs(this IEndpointRouteBuilder endpoints, IConfiguration configuration, Assembly[] assemblies)
    {
        // Find all non-abstract types that inherit from SignalR Hub in the given assemblies.
        var hubTypes = assemblies
            .SelectMany(x => x.GetTypes())
            .Where(t => typeof(Hub).IsAssignableFrom(t) && !t.IsAbstract);

        // Get the base URL for SignalR hubs from configuration, defaulting to "/hubs".
        var hubsBaseUrl = configuration.GetSection("SignalR:HubsBaseUrl").Value ?? "/hubs";

        foreach (var hubType in hubTypes)
        {
            // Build the route for each hub (e.g., "/hubs/chat" for "ChatHub").
            var route = $"{hubsBaseUrl}/{hubType.Name.Replace("Hub", "").ToLower()}";

            // Find the generic MapHub<T> method with the correct signature.
            var method = typeof(HubEndpointRouteBuilderExtensions)
                .GetMethods(BindingFlags.Public | BindingFlags.Static)
                .FirstOrDefault(m =>
                    m.Name == "MapHub" &&
                    m.IsGenericMethod &&
                    m.GetParameters().Length == 3);

            var generic = method?.MakeGenericMethod(hubType);

            // Configure SignalR to support both WebSockets and LongPolling transports.
            Action<HttpConnectionDispatcherOptions> configureOptions = options =>
            {
                options.Transports = HttpTransportType.WebSockets | HttpTransportType.LongPolling;
            };

            // Dynamically invoke MapHub<T> for each hub type.
            generic?.Invoke(null, [endpoints, route, configureOptions]);
        }
    }
}
