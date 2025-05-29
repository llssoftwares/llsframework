using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;

namespace LLSFramework.TabBlazor;

public static class ServicesExtensions
{
    public static IServiceCollection AddLLSSoftwaresFrameworkBlazor(this IServiceCollection services)
    {
        services.AddSignalR();

        return services
            .AddScoped<AuthenticationStateProvider, BlazorAuthenticationStateProvider>()
            .AddScoped<BlazorJwtTokenManager>()
            .AddScoped<FavIconManager>()
            .AddScoped<LoadingState>()
            .AddScoped<UrlManager>()
            .AddScoped<BlazorMediator>()
            .AddScoped<ActionHandler>()
            .AddSingleton<IUserIdProvider, CustomUserIdProvider>()
            .AddTabler()
            .AddResizeListener()
            .AddMediaQueryService();
        //.AddApexCharts();
    }

    public static void MapHubs(this IEndpointRouteBuilder endpoints, IConfiguration configuration, Assembly[] assemblies)
    {
        var hubTypes = assemblies
            .SelectMany(x => x.GetTypes())
            .Where(t => typeof(Hub).IsAssignableFrom(t) && !t.IsAbstract);

        var hubsBaseUrl = configuration.GetSection("SignalR:HubsBaseUrl").Value ?? "/hubs";

        foreach (var hubType in hubTypes)
        {
            var route = $"{hubsBaseUrl}/{hubType.Name.Replace("Hub", "").ToLower()}";

            var method = typeof(HubEndpointRouteBuilderExtensions)
                .GetMethods(BindingFlags.Public | BindingFlags.Static)
                .FirstOrDefault(m =>
                    m.Name == "MapHub" &&
                    m.IsGenericMethod &&
                    m.GetParameters().Length == 3);

            var generic = method?.MakeGenericMethod(hubType);

            Action<HttpConnectionDispatcherOptions> configureOptions = options =>
            {
                options.Transports = HttpTransportType.WebSockets | HttpTransportType.LongPolling;
            };

            generic?.Invoke(null, [endpoints, route, configureOptions]);
        }
    }
}