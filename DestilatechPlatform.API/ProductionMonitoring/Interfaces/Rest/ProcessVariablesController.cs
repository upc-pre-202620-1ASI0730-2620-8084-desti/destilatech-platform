using System.Net.Mime;
using DestilatechPlatform.API.ProductionMonitoring.Application.Services;
using DestilatechPlatform.API.ProductionMonitoring.Domain.Model;
using DestilatechPlatform.API.ProductionMonitoring.Interfaces.Rest.Resources;
using DestilatechPlatform.API.ProductionMonitoring.Interfaces.Rest.Transform;
using Microsoft.AspNetCore.Mvc;

namespace DestilatechPlatform.API.ProductionMonitoring.Interfaces.Rest;

/// <summary>
/// Write endpoints of the process variables. The variables of a tenant are listed in
/// <see cref="TenantProcessVariablesController"/>.
/// </summary>
[ApiController]
[Route("api/v1/process-variables")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Production")]
public class ProcessVariablesController(IProcessMonitoringCommandService processMonitoringCommandService)
    : ControllerBase
{
    [HttpPost]
    [EndpointSummary("Add a process variable to a batch")]
    [ProducesResponseType(typeof(ProcessVariableResource), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult> AddProcessVariable([FromBody] CreateProcessVariableResource resource,
        CancellationToken cancellationToken)
    {
        if (resource.TenantId <= 0) return TenantIdRequired();

        var command = AddProcessVariableCommandFromResourceAssembler.ToCommandFromResource(resource,
            resource.TenantId);
        var result = await processMonitoringCommandService.Handle(command, cancellationToken);
        return result.Match<ActionResult>(
            variable => StatusCode(StatusCodes.Status201Created,
                ProcessVariableResourceFromEntityAssembler.ToResourceFromEntity(variable)),
            error => ActionResultFromProductionMonitoringErrorAssembler.ToActionResultFromError(error, this));
    }

    [HttpPatch("{id:int}")]
    [EndpointSummary("Configure the range of a process variable")]
    [ProducesResponseType(typeof(ProcessVariableResource), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> ConfigureVariableRange(int id, [FromBody] ConfigureVariableRangeResource resource,
        CancellationToken cancellationToken)
    {
        if (resource.TenantId <= 0) return TenantIdRequired();

        var command = ConfigureVariableRangeCommandFromResourceAssembler.ToCommandFromResource(resource, id,
            resource.TenantId);
        var result = await processMonitoringCommandService.Handle(command, cancellationToken);
        return result.Match<ActionResult>(
            variable => Ok(ProcessVariableResourceFromEntityAssembler.ToResourceFromEntity(variable)),
            error => ActionResultFromProductionMonitoringErrorAssembler.ToActionResultFromError(error, this));
    }

    private ActionResult TenantIdRequired()
    {
        return ActionResultFromProductionMonitoringErrorAssembler.ToActionResultFromError(
            ProductionMonitoringError.InvalidData("The tenantId is required."), this);
    }
}
