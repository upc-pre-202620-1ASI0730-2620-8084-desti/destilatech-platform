using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace DestilatechPlatform.API.Shared.Infrastructure.Interfaces.Asp.OpenApi;

/// <summary>
/// Declares the JWT bearer security scheme in the OpenAPI document, so Scalar can send the access token.
/// </summary>
public sealed class BearerSecuritySchemeTransformer : IOpenApiDocumentTransformer
{
    public const string SchemeName = "Bearer";

    public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context,
        CancellationToken cancellationToken)
    {
        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
        document.Components.SecuritySchemes[SchemeName] = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            Description = "Access token returned by POST /api/v1/tokens."
        };
        return Task.CompletedTask;
    }
}
