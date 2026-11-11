using System.Net.Mime;
using DestilatechPlatform.API.ProductionMonitoring.Application.Services;
using DestilatechPlatform.API.ProductionMonitoring.Domain.Model;
using DestilatechPlatform.API.ProductionMonitoring.Domain.Model.Queries;
using DestilatechPlatform.API.ProductionMonitoring.Interfaces.Rest.Resources;
using DestilatechPlatform.API.ProductionMonitoring.Interfaces.Rest.Transform;
using Microsoft.AspNetCore.Mvc;

namespace DestilatechPlatform.API.ProductionMonitoring.Interfaces.Rest;

[ApiController]
[Route("api/v1/sensor-readings")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Production")]
public class SensorReadingsController(
    IProcessMonitoringCommandService processMonitoringCommandService,
    IProductionQueryService productionQueryService) : ControllerBase
{
    [HttpGet]
    [EndpointSummary("List the latest sensor readings of an account")]
    [ProducesResponseType(typeof(IEnumerable<SensorReadingResource>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> GetLatestSensorReadings([FromQuery] int userId,
        [FromQuery(Name = "_limit")] int? limit, CancellationToken cancellationToken)
    {
        if (userId <= 0) return UserIdRequired();

        var readings = await productionQueryService.Handle(new GetLatestSensorReadingsByUserIdQuery(userId, limit),
            cancellationToken);
        return Ok(readings.Select(SensorReadingResourceFromEntityAssembler.ToResourceFromEntity));
    }

    [HttpPost]
    [EndpointSummary("Record a sensor reading")]
    [EndpointDescription("Returns 201 for a new reading and 200 if a reading with the same source key was already recorded. A reading out of range raises an anomaly alert.")]
    [ProducesResponseType(typeof(SensorReadingResource), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(SensorReadingResource), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult> RecordSensorReading([FromQuery] int userId,
        [FromBody] RecordSensorReadingResource resource, CancellationToken cancellationToken)
    {
        if (userId <= 0) return UserIdRequired();

        var command = RecordSensorReadingCommandFromResourceAssembler.ToCommandFromResource(resource, userId);
        var result = await processMonitoringCommandService.Handle(command, cancellationToken);
        return result.Match<ActionResult>(
            recorded => StatusCode(recorded.Replayed ? StatusCodes.Status200OK : StatusCodes.Status201Created,
                SensorReadingResourceFromEntityAssembler.ToResourceFromEntity(recorded.Reading)),
            error => ActionResultFromProductionMonitoringErrorAssembler.ToActionResultFromError(error, this));
    }

    private ActionResult UserIdRequired()
    {
        return ActionResultFromProductionMonitoringErrorAssembler.ToActionResultFromError(
            ProductionMonitoringError.InvalidData("The userId of the account is required."), this);
    }
}
