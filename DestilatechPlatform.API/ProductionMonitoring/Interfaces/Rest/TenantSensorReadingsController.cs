using System.Net.Mime;
using DestilatechPlatform.API.ProductionMonitoring.Application.Services;
using DestilatechPlatform.API.ProductionMonitoring.Domain.Model.Queries;
using DestilatechPlatform.API.ProductionMonitoring.Interfaces.Rest.Resources;
using DestilatechPlatform.API.ProductionMonitoring.Interfaces.Rest.Transform;
using Microsoft.AspNetCore.Mvc;

namespace DestilatechPlatform.API.ProductionMonitoring.Interfaces.Rest;

/// <summary>
/// Sensor readings of a tenant, as a sub-resource of the tenant:
/// <c>/api/v1/tenants/{tenantId}/sensor-readings</c>.
/// </summary>
[ApiController]
[Route("api/v1/tenants/{tenantId:int}/sensor-readings")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Production")]
public class TenantSensorReadingsController(IProductionQueryService productionQueryService) : ControllerBase
{
    [HttpGet]
    [EndpointSummary("Get the latest sensor readings by tenant id")]
    [EndpointDescription("Latest readings first; limit is the maximum number of readings (all of them by default).")]
    [ProducesResponseType(typeof(IEnumerable<SensorReadingResource>), StatusCodes.Status200OK)]
    public async Task<ActionResult> GetSensorReadingsByTenantId(int tenantId, [FromQuery] int? limit,
        CancellationToken cancellationToken)
    {
        var readings = await productionQueryService.Handle(new GetLatestSensorReadingsByUserIdQuery(tenantId, limit),
            cancellationToken);
        return Ok(readings.Select(SensorReadingResourceFromEntityAssembler.ToResourceFromEntity));
    }
}
