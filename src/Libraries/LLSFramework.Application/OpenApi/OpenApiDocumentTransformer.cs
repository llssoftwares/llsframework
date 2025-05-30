namespace LLSFramework.Application.OpenApi;

/// <summary>
/// Transforms the OpenAPI document at runtime by applying server settings and security requirements
/// based on application configuration and endpoint metadata.
/// </summary>
public class OpenApiDocumentTransformer(EndpointDataSource endpointDataSource, IConfiguration configuration) : IOpenApiDocumentTransformer
{
    /// <summary>
    /// Modifies the OpenAPI document by updating server information and applying security schemes
    /// to operations that require authorization.
    /// </summary>
    /// <param name="document">The OpenAPI document to transform.</param>
    /// <param name="context">The transformation context.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A completed task.</returns>
    public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        // Retrieve OpenAPI settings from configuration
        var openApiSettings = configuration.GetSection("OpenApiSettings").Get<OpenApiSettings>();

        if (openApiSettings != null)
        {
            // Clear existing servers and add those defined in configuration
            document.Servers.Clear();

            foreach (var server in openApiSettings.Servers)
            {
                document.Servers.Add(server);
            }
        }

        // Define the Bearer security scheme for JWT authentication
        document.Components = new OpenApiComponents()
        {
            SecuritySchemes = new Dictionary<string, OpenApiSecurityScheme>()
            {
                {
                    "Bearer",
                    new OpenApiSecurityScheme
                    {
                        Type = SecuritySchemeType.Http,
                        Scheme = "bearer",
                        In = ParameterLocation.Header
                    }
                }
            }
        };

        // Iterate over all API paths and operations
        foreach (var pathItem in document.Paths)
        {
            foreach (var operation in pathItem.Value.Operations)
            {
                // Find the corresponding endpoint for the operation
                var endpoint = endpointDataSource.Endpoints
                    .OfType<RouteEndpoint>()
                    .FirstOrDefault(e => MatchesOperation(e, pathItem.Key, operation.Key));

                if (endpoint != null)
                {
                    // Check if the endpoint requires authorization
                    var hasAuthorizeAttribute = endpoint.Metadata
                        .OfType<AuthorizeAttribute>()
                        .Any();

                    if (hasAuthorizeAttribute)
                    {
                        // Add the Bearer security requirement to the operation
                        operation.Value.Security.Add(new OpenApiSecurityRequirement
                        {
                            {
                                new OpenApiSecurityScheme
                                {
                                    Reference = new OpenApiReference
                                    {
                                        Id = "Bearer",
                                        Type = ReferenceType.SecurityScheme
                                    }
                                },
                                []
                            }
                        });
                    }
                }
            }
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Determines if the given endpoint matches the specified OpenAPI path and operation type.
    /// </summary>
    /// <param name="endpoint">The route endpoint to check.</param>
    /// <param name="path">The OpenAPI path string (e.g., "/api/values").</param>
    /// <param name="operationType">The OpenAPI operation type (e.g., GET, POST).</param>
    /// <returns><c>true</c> if the endpoint matches the operation; otherwise, <c>false</c>.</returns>
    private static bool MatchesOperation(RouteEndpoint endpoint, string path, OperationType operationType)
    {
        var httpMethods = endpoint.Metadata
            .OfType<HttpMethodMetadata>()
            .FirstOrDefault()?.HttpMethods;

        // Compare HTTP method and route pattern to determine a match
        return httpMethods != null &&
               httpMethods.Any(method => method.Equals(operationType.ToString(), StringComparison.OrdinalIgnoreCase)) &&
               endpoint.RoutePattern.RawText == path[1..];
    }
}