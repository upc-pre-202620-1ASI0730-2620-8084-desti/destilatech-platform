using System.Net.Mime;
using DestilatechPlatform.API.ProductionMonitoring.Application.Services;
using DestilatechPlatform.API.ProductionMonitoring.Domain.Model;
using DestilatechPlatform.API.ProductionMonitoring.Domain.Model.Queries;
using DestilatechPlatform.API.ProductionMonitoring.Interfaces.Rest.Resources;
using DestilatechPlatform.API.ProductionMonitoring.Interfaces.Rest.Transform;
using Microsoft.AspNetCore.Mvc;

namespace DestilatechPlatform.API.ProductionMonitoring.Interfaces.Rest;

[ApiController]
[Route("api/v1/batches")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Production")]
public class ProductionBatchesController(
    IProductionBatchCommandService productionBatchCommandService,
    IProcessMonitoringCommandService processMonitoringCommandService,
    IProductionQueryService productionQueryService) : ControllerBase
{
    [HttpGet]
    [EndpointSummary("List the batches of an account")]
    [ProducesResponseType(typeof(IEnumerable<ProductionBatchResource>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> GetBatches([FromQuery] int userId, CancellationToken cancellationToken)
    {
        if (userId <= 0) return UserIdRequired();

        var batches = await productionQueryService.Handle(new GetAllProductionBatchesByUserIdQuery(userId),
            cancellationToken);
        return Ok(batches.Select(ProductionBatchResourceFromEntityAssembler.ToResourceFromEntity));
    }

    [HttpPost]
    [EndpointSummary("Register a batch")]
    [ProducesResponseType(typeof(ProductionBatchResource), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult> CreateBatch([FromQuery] int userId,
        [FromBody] CreateProductionBatchResource resource, CancellationToken cancellationToken)
    {
        if (userId <= 0) return UserIdRequired();

        var command = CreateProductionBatchCommandFromResourceAssembler.ToCommandFromResource(resource, userId);
        var result = await productionBatchCommandService.Handle(command, cancellationToken);
        return result.Match<ActionResult>(
            batch => StatusCode(StatusCodes.Status201Created,
                ProductionBatchResourceFromEntityAssembler.ToResourceFromEntity(batch)),
            error => ActionResultFromProductionMonitoringErrorAssembler.ToActionResultFromError(error, this));
    }

    [HttpPut("{id:int}")]
    [EndpointSummary("Update a batch")]
    [EndpointDescription("Updates the batch details and, if the stage is the next one, advances the batch.")]
    [ProducesResponseType(typeof(ProductionBatchResource), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult> UpdateBatch(int id, [FromQuery] int userId,
        [FromBody] UpdateProductionBatchResource resource, CancellationToken cancellationToken)
    {
        if (userId <= 0) return UserIdRequired();

        var command = UpdateProductionBatchCommandFromResourceAssembler.ToCommandFromResource(resource, id, userId);
        var result = await productionBatchCommandService.Handle(command, cancellationToken);
        return result.Match<ActionResult>(
            batch => Ok(ProductionBatchResourceFromEntityAssembler.ToResourceFromEntity(batch)),
            error => ActionResultFromProductionMonitoringErrorAssembler.ToActionResultFromError(error, this));
    }

    [HttpPatch("{id:int}/status")]
    [EndpointSummary("Advance the stage of a batch")]
    [EndpointDescription("Bottling adds the bottles to the stock of the selected product.")]
    [ProducesResponseType(typeof(ProductionBatchResource), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult> AdvanceBatchStage(int id, [FromQuery] int userId,
        [FromBody] UpdateBatchStatusResource resource, CancellationToken cancellationToken)
    {
        if (userId <= 0) return UserIdRequired();

        var command = AdvanceBatchStageCommandFromResourceAssembler.ToCommandFromResource(resource, id, userId);
        var result = await productionBatchCommandService.Handle(command, cancellationToken);
        return result.Match<ActionResult>(
            batch => Ok(ProductionBatchResourceFromEntityAssembler.ToResourceFromEntity(batch)),
            error => ActionResultFromProductionMonitoringErrorAssembler.ToActionResultFromError(error, this));
    }

    [HttpGet("{id:int}/readings")]
    [EndpointSummary("List the readings of a batch")]
    [ProducesResponseType(typeof(IEnumerable<SensorReadingResource>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetBatchReadings(int id, [FromQuery] int userId, [FromQuery] string? variable,
        [FromQuery] DateTimeOffset? from, [FromQuery] DateTimeOffset? to, CancellationToken cancellationToken)
    {
        if (userId <= 0) return UserIdRequired();

        var result = await productionQueryService.Handle(new GetBatchSensorReadingsQuery(userId, id, variable, from, to),
            cancellationToken);
        return result.Match<ActionResult>(
            readings => Ok(readings.Select(SensorReadingResourceFromEntityAssembler.ToResourceFromEntity)),
            error => ActionResultFromProductionMonitoringErrorAssembler.ToActionResultFromError(error, this));
    }

    [HttpPost("{id:int}/readings")]
    [EndpointSummary("Ingest a reading of an IoT sensor of the batch")]
    [EndpointDescription("Returns 201 for a new reading and 200 if a reading with the same source_key was already recorded.")]
    [ProducesResponseType(typeof(SensorReadingResource), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(SensorReadingResource), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult> IngestBatchReading(int id, [FromQuery] int userId,
        [FromBody] IngestBatchReadingResource resource, CancellationToken cancellationToken)
    {
        if (userId <= 0) return UserIdRequired();

        if (string.IsNullOrWhiteSpace(resource.SourceId) || string.IsNullOrWhiteSpace(resource.SourceKey))
            return ActionResultFromProductionMonitoringErrorAssembler.ToActionResultFromError(
                ProductionMonitoringError.SourceRequired(), this);

        var command = RecordSensorReadingCommandFromResourceAssembler.ToCommandFromResource(resource, id, userId);
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
