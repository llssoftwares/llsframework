using LLSFramework.Application;
using LLSFramework.Application.Auth;
using LLSFramework.Application.OpenApi;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Scalar.AspNetCore;

namespace LLSFramework.Application
{
    /// <summary>
    /// Provides extension methods for registering core LLSFramework.Application services,
    /// including authentication, authorization, and OpenAPI documentation.
    /// </summary>
    public static class ServicesExtensions
    {
        /// <summary>
        /// Registers all core services required by LLSFramework.Application into the DI container.
        /// Includes JWT authentication, authorization, and OpenAPI/Swagger documentation.
        /// </summary>
        /// <param name="services">The service collection to add to.</param>
        /// <param name="configuration">The application configuration (for JWT and SignalR settings).</param>
        /// <returns>The updated service collection for chaining.</returns>
        public static IServiceCollection AddLLSFrameworkApplication(this IServiceCollection services, IConfiguration configuration)
        {
            return services
                .AddScoped<JwtTokenManager>() // Provides JWT token generation and validation.
                .Configure<JwtSettings>(configuration.GetSection("JwtSettings")) // Binds JWT settings from configuration.
                .AddJwtAuthentication(configuration) // Adds JWT authentication and SignalR token support.
                .AddAuthorization() // Adds default authorization services.
                .AddOpenApi(); // Adds OpenAPI/Swagger documentation with custom transformers.
        }

        /// <summary>
        /// Maps OpenAPI and Scalar API reference endpoints for API documentation and reference UI.
        /// </summary>
        /// <param name="endpoints">The endpoint route builder to map endpoints on.</param>
        /// <param name="url">The URL for the Scalar API reference UI (default: "/api/docs").</param>
        /// <returns>The updated endpoint route builder for chaining.</returns>
        public static IEndpointRouteBuilder MapLLSFrameworkApiReference(this IEndpointRouteBuilder endpoints, string url = "/api/docs")
        {
            endpoints.MapOpenApi(); // Maps the OpenAPI/Swagger endpoint.
            endpoints.MapScalarApiReference(url); // Maps the Scalar API reference UI endpoint.

            return endpoints;
        }

        /// <summary>
        /// Configures JWT Bearer authentication using settings from configuration.
        /// Supports SignalR integration by extracting tokens from query string for hub endpoints.
        /// </summary>
        /// <param name="services">The service collection to add authentication to.</param>
        /// <param name="configuration">The application configuration (for JWT and SignalR settings).</param>
        /// <returns>The updated service collection for chaining.</returns>
        private static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettings>()
                ?? throw new Exception("JwtSettings is null");

            var signalRHubsBaseUrl = configuration.GetSection("SignalR:HubsBaseUrl").Value;

            services
                .AddAuthentication(x =>
                {
                    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(x =>
                {
                    // Set token validation parameters from JwtSettings.
                    x.TokenValidationParameters = JwtTokenManager.GetTokenValidationParameters(jwtSettings);
                    // Support SignalR: extract access_token from query string for hub endpoints.
                    if (!string.IsNullOrEmpty(signalRHubsBaseUrl))
                    {
                        x.Events = new JwtBearerEvents
                        {
                            OnMessageReceived = context =>
                            {
                                var accessToken = context.Request.Query["access_token"];
                                var path = context.HttpContext.Request.Path;

                                if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments(signalRHubsBaseUrl))
                                    context.Token = accessToken;

                                return Task.CompletedTask;
                            }
                        };
                    }
                });

            return services;
        }

        /// <summary>
        /// Registers OpenAPI/Swagger documentation with a custom document transformer.
        /// </summary>
        /// <param name="services">The service collection to add OpenAPI to.</param>
        /// <returns>The updated service collection for chaining.</returns>
        private static IServiceCollection AddOpenApi(this IServiceCollection services)
        {
            return services.AddOpenApi(options =>
            {
                options.AddDocumentTransformer<OpenApiDocumentTransformer>();
            });
        }
    }
}