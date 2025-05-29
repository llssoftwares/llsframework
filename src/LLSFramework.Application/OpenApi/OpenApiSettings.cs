namespace LLSFramework.Application.OpenApi;

/// <summary>
/// Represents OpenAPI configuration settings, including the list of server definitions
/// to be included in the generated OpenAPI document.
/// </summary>
public class OpenApiSettings
{
    /// <summary>
    /// Gets or sets the collection of OpenAPI server definitions.
    /// Each <see cref="OpenApiServer"/> describes a server that provides API endpoints.
    /// </summary>
    public List<OpenApiServer> Servers { get; set; } = [];
}