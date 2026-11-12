using System.Net.Mime;
using DestilatechPlatform.API.ProductionMonitoring.Application.Services;
using DestilatechPlatform.API.ProductionMonitoring.Domain.Model;
using DestilatechPlatform.API.ProductionMonitoring.Interfaces.Rest.Resources;
using DestilatechPlatform.API.ProductionMonitoring.Interfaces.Rest.Transform;
using Microsoft.AspNetCore.Mvc;

namespace DestilatechPlatform.API.ProductionMonitoring.Interfaces.Rest;

/// <summary>
/// Write endpoints of the production batches. The tenant travels in the request body; the batches of a tenant
/// and their readings are listed in <see cref="TenantProductionBatchesController"/>.
/// </summary>
[ApiController]
[Route("api/v1/batches")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Production")]
public class ProductionBatchesController(
    IProductionBatchCommandService productionBatchCommandService,
    IProcessMonitoringCommandService processMonitoringCommandService) : ControllerBase
{
    [HttpPost]
    [EndpointSummary("Register a batch")]
    [ProducesResponseType(typeof(ProductionBatchResource), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult> CreateBatch([FromBody] CreateProductionBatchResource resource,
        CancellationToken cancellationToken)
    {
        if (resource.TenantId <= 0) return TenantIdRequired();

        var command = CreateProductionBatchCommandFromResourceAssembler.ToCommandFromResource(resource,
            resource.TenantId);
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
    public async Task<ActionResult> UpdateBatch(int id, [FromBody] UpdateProductionBatchResource resource,
        CancellationToken cancellationToken)
    {
        if (resource.TenantId <= 0) return TenantIdRequired();

        var command = UpdateProductionBatchCommandFromResourceAssembler.ToCommandFromResource(resource, id,
            resource.TenantId);
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
    public async Task<ActionResult> AdvanceBatchStage(int id, [FromBody] UpdateBatchStatusResource resource,
        CancellationToken cancellationToken)
    {
        if (resource.TenantId <= 0) return TenantIdRequired();

        var command = AdvanceBatchStageCommandFromResourceAssembler.ToCommandFromResource(resource, id,
            resource.TenantId);
        var result = await productionBatchCommandService.Handle(command, cancellationToken);
        return result.Match<ActionResult>(
            batch => Ok(ProductionBatchResourceFromEntityAssembler.ToResourceFromEntity(batch)),
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
    public async Task<ActionResult> IngestBatchReading(int id, [FromBody] IngestBatchReadingResource resource,
        CancellationToken cancellationToken)
    {
        if (resource.TenantId <= 0) return TenantIdRequired();

        if (string.IsNullOrWhiteSpace(resource.SourceId) || string.IsNullOrWhiteSpace(resource.SourceKey))
            return ActionResultFromProductionMonitoringErrorAssembler.ToActionResultFromError(
                ProductionMonitoringError.SourceRequired(), this);

        var command = RecordSensorReadingCommandFromResourceAssembler.ToCommandFromResource(resource, id,
            resource.TenantId);
        var result = await processMonitoringCommandService.Handle(command, cancellationToken);
        return result.Match<ActionResult>(
            recorded => StatusCode(recorded.Replayed ? StatusCodes.Status200OK : StatusCodes.Status201Created,
                SensorReadingResourceFromEntityAssembler.ToResourceFromEntity(recorded.Reading)),
            error => ActionResultFromProductionMonitoringErrorAssembler.ToActionResultFromError(error, this));
    }

    private ActionResult TenantIdRequired()
    {
        return ActionResultFromProductionMonitoringErrorAssembler.ToActionResultFromError(
            ProductionMonitoringError.InvalidData("The tenantId is required."), this);
    }
}
