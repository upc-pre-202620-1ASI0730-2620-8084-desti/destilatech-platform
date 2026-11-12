using System.Net.Mime;
using DestilatechPlatform.API.AlertsNotifications.Application.Services;
using DestilatechPlatform.API.AlertsNotifications.Domain.Model;
using DestilatechPlatform.API.AlertsNotifications.Domain.Model.Queries;
using DestilatechPlatform.API.AlertsNotifications.Domain.Model.ValueObjects;
using DestilatechPlatform.API.AlertsNotifications.Interfaces.Rest.Resources;
using DestilatechPlatform.API.AlertsNotifications.Interfaces.Rest.Transform;
using DestilatechPlatform.API.Resources;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using ProblemDetailsFactory = DestilatechPlatform.API.Shared.Interfaces.Rest.ProblemDetails.ProblemDetailsFactory;

namespace DestilatechPlatform.API.AlertsNotifications.Interfaces.Rest;

/// <summary>
/// Alerts of a tenant, as a sub-resource of the tenant: <c>/api/v1/tenants/{tenantId}/alerts</c>.
/// </summary>
[ApiController]
[Route("api/v1/tenants/{tenantId:int}/alerts")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Alerts")]
public class TenantAlertsController(
    IAlertQueryService alertQueryService,
    IStringLocalizer<SharedResource> localizer,
    ProblemDetailsFactory problemDetailsFactory,
    ILogger<TenantAlertsController> logger) : ControllerBase
{
    [HttpGet]
    [EndpointSummary("Get all alerts by tenant id")]
    [EndpointDescription("Optional filters: type (LOW_STOCK, ANOMALY) and status (PENDING, ATTENDED).")]
    [ProducesResponseType(typeof(IEnumerable<AlertResource>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAlertsByTenantId(int tenantId,
        [FromQuery] string? type = null,
        [FromQuery] string? status = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var getAllAlertsByAccountIdQuery = new GetAllAlertsByAccountIdQuery(
                new AccountId(tenantId),
                string.IsNullOrWhiteSpace(type) ? null : AlertValueAssembler.ToEnumFromValue<AlertType>(type),
                string.IsNullOrWhiteSpace(status) ? null : AlertValueAssembler.ToEnumFromValue<AlertStatus>(status));
            var result = await alertQueryService.Handle(getAllAlertsByAccountIdQuery, cancellationToken);
            return Ok(result.Select(AlertResourceFromEntityAssembler.ToResourceFromEntity));
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning(ex, "Validation failed while querying alerts for tenant {TenantId}", tenantId);
            return problemDetailsFactory.CreateProblemDetails(this, StatusCodes.Status400BadRequest,
                AlertsError.InvalidAlertData, localizer["InvalidAlertQuery"].Value);
        }
    }
}
