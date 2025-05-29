using LLSFramework.Application;
using LLSFramework.Application.Auth;
using LLSFramework.Application.OpenApi;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Scalar.AspNetCore;

namespace LLSFramework.Application
{
    public static class ServicesExtensions
    {
        public static IServiceCollection AddLLSFrameworkApplication(this IServiceCollection services, IConfiguration configuration)
        {
            return services
                .AddScoped<JwtTokenManager>()
                .Configure<JwtSettings>(configuration.GetSection("JwtSettings"))
                .AddJwtAuthentication(configuration)
                .AddAuthorization()
                .AddOpenApi();
        }

        public static IEndpointRouteBuilder MapLLSFrameworkApiReference(this IEndpointRouteBuilder endpoints, string url = "/api/docs")
        {
            endpoints.MapOpenApi();
            endpoints.MapScalarApiReference(url);

            return endpoints;
        }

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
                    x.TokenValidationParameters = JwtTokenManager.GetTokenValidationParameters(jwtSettings);
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

        private static IServiceCollection AddOpenApi(this IServiceCollection services)
        {
            return services.AddOpenApi(options =>
            {
                options.AddDocumentTransformer<OpenApiDocumentTransformer>();
            });
        }
    }
}