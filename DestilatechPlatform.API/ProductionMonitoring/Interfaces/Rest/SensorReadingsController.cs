using System.Net.Mime;
using DestilatechPlatform.API.ProductionMonitoring.Application.Services;
using DestilatechPlatform.API.ProductionMonitoring.Domain.Model;
using DestilatechPlatform.API.ProductionMonitoring.Interfaces.Rest.Resources;
using DestilatechPlatform.API.ProductionMonitoring.Interfaces.Rest.Transform;
using Microsoft.AspNetCore.Mvc;

namespace DestilatechPlatform.API.ProductionMonitoring.Interfaces.Rest;

/// <summary>
/// Write endpoint of the sensor readings. The latest readings of a tenant are listed in
/// <see cref="TenantSensorReadingsController"/>.
/// </summary>
[ApiController]
[Route("api/v1/sensor-readings")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Production")]
public class SensorReadingsController(IProcessMonitoringCommandService processMonitoringCommandService)
    : ControllerBase
{
    [HttpPost]
    [EndpointSummary("Record a sensor reading")]
    [EndpointDescription("Returns 201 for a new reading and 200 if a reading with the same source key was already recorded. A reading out of range raises an anomaly alert.")]
    [ProducesResponseType(typeof(SensorReadingResource), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(SensorReadingResource), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult> RecordSensorReading([FromBody] RecordSensorReadingResource resource,
        CancellationToken cancellationToken)
    {
        if (resource.TenantId <= 0)
            return ActionResultFromProductionMonitoringErrorAssembler.ToActionResultFromError(
                ProductionMonitoringError.InvalidData("The tenantId is required."), this);

        var command = RecordSensorReadingCommandFromResourceAssembler.ToCommandFromResource(resource,
            resource.TenantId);
        var result = await processMonitoringCommandService.Handle(command, cancellationToken);
        return result.Match<ActionResult>(
            recorded => StatusCode(recorded.Replayed ? StatusCodes.Status200OK : StatusCodes.Status201Created,
                SensorReadingResourceFromEntityAssembler.ToResourceFromEntity(recorded.Reading)),
            error => ActionResultFromProductionMonitoringErrorAssembler.ToActionResultFromError(error, this));
    }
}
