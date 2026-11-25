using DestilatechPlatform.API.Iam.Infrastructure.Pipeline.Middleware.Attributes;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace DestilatechPlatform.API.Shared.Infrastructure.Interfaces.Asp.OpenApi;

/// <summary>
/// Marks the operations protected with <see cref="AuthorizeAttribute"/> as requiring the bearer token in the
/// OpenAPI document; the <see cref="AllowAnonymousAttribute"/> operations stay public.
/// </summary>
public sealed class BearerSecurityOperationTransformer : IOpenApiOperationTransformer
{
    public Task TransformAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context,
        CancellationToken cancellationToken)
    {
        var metadata = context.Description.ActionDescriptor.EndpointMetadata;
        if (metadata.OfType<AllowAnonymousAttribute>().Any() || !metadata.OfType<AuthorizeAttribute>().Any())
            return Task.CompletedTask;

        operation.Security ??= [];
        operation.Security.Add(new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference(BearerSecuritySchemeTransformer.SchemeName, context.Document)] = []
        });
        return Task.CompletedTask;
    }
}
