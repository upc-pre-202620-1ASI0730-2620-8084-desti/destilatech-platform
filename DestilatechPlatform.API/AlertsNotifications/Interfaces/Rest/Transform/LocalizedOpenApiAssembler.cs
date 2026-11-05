using DestilatechPlatform.API.AlertsNotifications.Interfaces.Rest.Resources;
using DestilatechPlatform.API.Resources;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.Extensions.Localization;
using Microsoft.OpenApi;

namespace DestilatechPlatform.API.AlertsNotifications.Interfaces.Rest.Transform;

public static class LocalizedOpenApiAssembler
{
    private static readonly Dictionary<string, (string Summary, string Description,
        IReadOnlyDictionary<string, string> Responses)> OperationResources =
        new Dictionary<string, (string, string, IReadOnlyDictionary<string, string>)>
        {
            ["RaiseAlert"] = (
                "RaiseAlertSummary",
                "RaiseAlertDescription",
                new Dictionary<string, string>
                {
                    ["201"] = "RaiseAlertCreated",
                    ["400"] = "RaiseAlertBadRequest",
                    ["409"] = "RaiseAlertConflict",
                    ["500"] = "RaiseAlertServerError"
                }),
            ["GetAlertsFromQuery"] = (
                "GetAlertsQuerySummary",
                "GetAlertsQueryDescription",
                new Dictionary<string, string>
                {
                    ["200"] = "GetAlertsQuerySuccess",
                    ["400"] = "GetAlertsQueryBadRequest"
                }),
            ["GetAlertById"] = (
                "GetAlertByIdSummary",
                "GetAlertByIdDescription",
                new Dictionary<string, string>
                {
                    ["200"] = "GetAlertByIdSuccess",
                    ["404"] = "GetAlertByIdNotFound"
                }),
            ["MarkAlertAsAttended"] = (
                "MarkAlertAsAttendedSummary",
                "MarkAlertAsAttendedDescription",
                new Dictionary<string, string>
                {
                    ["200"] = "MarkAlertAsAttendedSuccess",
                    ["404"] = "MarkAlertAsAttendedNotFound",
                    ["409"] = "MarkAlertAsAttendedConflict",
                    ["500"] = "MarkAlertAsAttendedServerError"
                })
        };

    private static readonly Dictionary<string, string> PropertyResources = new()
    {
        ["id"] = "AlertId",
        ["accountId"] = "AlertAccountId",
        ["type"] = "AlertType",
        ["sourceId"] = "AlertSourceId",
        ["message"] = "AlertMessage",
        ["severity"] = "AlertSeverity",
        ["status"] = "AlertStatus",
        ["createdAt"] = "AlertCreatedAt",
        ["attendedAt"] = "AlertAttendedAt"
    };

    public static Task TransformOperationAsync(
        OpenApiOperation operation,
        OpenApiOperationTransformerContext context,
        CancellationToken cancellationToken)
    {
        if (context.Description.ActionDescriptor is not ControllerActionDescriptor action ||
            !OperationResources.TryGetValue(action.ActionName, out var resources))
            return Task.CompletedTask;

        var localizer = context.ApplicationServices.GetRequiredService<IStringLocalizer<SharedResource>>();
        operation.OperationId = action.ActionName;
        operation.Summary = localizer[resources.Summary].Value;
        operation.Description = localizer[resources.Description].Value;
        if (operation.Responses is null) return Task.CompletedTask;

        foreach (var (statusCode, resourceKey) in resources.Responses)
        {
            if (operation.Responses.TryGetValue(statusCode, out var response) &&
                response is OpenApiResponse openApiResponse)
                openApiResponse.Description = localizer[resourceKey].Value;
        }

        return Task.CompletedTask;
    }
    
    public static Task TransformSchemaAsync(
        OpenApiSchema schema,
        OpenApiSchemaTransformerContext context,
        CancellationToken cancellationToken)
    {
        var localizer = context.ApplicationServices.GetRequiredService<IStringLocalizer<SharedResource>>();
        var type = context.JsonTypeInfo.Type;
        if (type == typeof(RaiseAlertResource))
            schema.Description = localizer["RaiseAlertSchema"].Value;
        else if (type == typeof(AlertResource))
            schema.Description = localizer["AlertSchema"].Value;

        var property = context.JsonPropertyInfo;
        if (property is null ||
            (property.DeclaringType != typeof(RaiseAlertResource) && property.DeclaringType != typeof(AlertResource)))
            return Task.CompletedTask;

        if (PropertyResources.TryGetValue(property.Name, out var propertyResourceKey))
            schema.Description = localizer[propertyResourceKey].Value;

        return Task.CompletedTask;
    }
}
