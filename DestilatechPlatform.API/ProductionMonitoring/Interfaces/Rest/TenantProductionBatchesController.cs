using System.Net.Mime;
using DestilatechPlatform.API.ProductionMonitoring.Application.Services;
using DestilatechPlatform.API.ProductionMonitoring.Domain.Model.Queries;
using DestilatechPlatform.API.ProductionMonitoring.Interfaces.Rest.Resources;
using DestilatechPlatform.API.ProductionMonitoring.Interfaces.Rest.Transform;
using Microsoft.AspNetCore.Mvc;

namespace DestilatechPlatform.API.ProductionMonitoring.Interfaces.Rest;

/// <summary>
/// Production batches of a tenant, as a sub-resource of the tenant: <c>/api/v1/tenants/{tenantId}/batches</c>.
/// </summary>
[ApiController]
[Route("api/v1/tenants/{tenantId:int}/batches")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Production")]
public class TenantProductionBatchesController(IProductionQueryService productionQueryService) : ControllerBase
{
    [HttpGet]
    [EndpointSummary("Get all batches by tenant id")]
    [ProducesResponseType(typeof(IEnumerable<ProductionBatchResource>), StatusCodes.Status200OK)]
    public async Task<ActionResult> GetBatchesByTenantId(int tenantId, CancellationToken cancellationToken)
    {
        var batches = await productionQueryService.Handle(new GetAllProductionBatchesByUserIdQuery(tenantId),
            cancellationToken);
        return Ok(batches.Select(ProductionBatchResourceFromEntityAssembler.ToResourceFromEntity));
    }

    [HttpGet("{batchId:int}/readings")]
    [EndpointSummary("Get the readings of a batch of the tenant")]
    [EndpointDescription("Optional filters: variable (TEMPERATURE, PH...), from and to (dates).")]
    [ProducesResponseType(typeof(IEnumerable<SensorReadingResource>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetBatchReadings(int tenantId, int batchId, [FromQuery] string? variable,
        [FromQuery] DateTimeOffset? from, [FromQuery] DateTimeOffset? to, CancellationToken cancellationToken)
    {
        var result = await productionQueryService.Handle(
            new GetBatchSensorReadingsQuery(tenantId, batchId, variable, from, to), cancellationToken);
        return result.Match<ActionResult>(
            readings => Ok(readings.Select(SensorReadingResourceFromEntityAssembler.ToResourceFromEntity)),
            error => ActionResultFromProductionMonitoringErrorAssembler.ToActionResultFromError(error, this));
    }
}
