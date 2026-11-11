using System.Net.Mime;
using DestilatechPlatform.API.ProductionMonitoring.Application.Services;
using DestilatechPlatform.API.ProductionMonitoring.Domain.Model;
using DestilatechPlatform.API.ProductionMonitoring.Domain.Model.Queries;
using DestilatechPlatform.API.ProductionMonitoring.Interfaces.Rest.Resources;
using DestilatechPlatform.API.ProductionMonitoring.Interfaces.Rest.Transform;
using Microsoft.AspNetCore.Mvc;

namespace DestilatechPlatform.API.ProductionMonitoring.Interfaces.Rest;

[ApiController]
[Route("api/v1/process-variables")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Production")]
public class ProcessVariablesController(
    IProcessMonitoringCommandService processMonitoringCommandService,
    IProductionQueryService productionQueryService) : ControllerBase
{
    [HttpGet]
    [EndpointSummary("List the process variables of an account")]
    [ProducesResponseType(typeof(IEnumerable<ProcessVariableResource>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> GetProcessVariables([FromQuery] int userId, CancellationToken cancellationToken)
    {
        if (userId <= 0) return UserIdRequired();

        var variables = await productionQueryService.Handle(new GetAllProcessVariablesByUserIdQuery(userId),
            cancellationToken);
        return Ok(variables.Select(ProcessVariableResourceFromEntityAssembler.ToResourceFromEntity));
    }

    [HttpPost]
    [EndpointSummary("Add a process variable to a batch")]
    [ProducesResponseType(typeof(ProcessVariableResource), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult> AddProcessVariable([FromQuery] int userId,
        [FromBody] CreateProcessVariableResource resource, CancellationToken cancellationToken)
    {
        if (userId <= 0) return UserIdRequired();

        var command = AddProcessVariableCommandFromResourceAssembler.ToCommandFromResource(resource, userId);
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
    public async Task<ActionResult> ConfigureVariableRange(int id, [FromQuery] int userId,
        [FromBody] ConfigureVariableRangeResource resource, CancellationToken cancellationToken)
    {
        if (userId <= 0) return UserIdRequired();

        var command = ConfigureVariableRangeCommandFromResourceAssembler.ToCommandFromResource(resource, id, userId);
        var result = await processMonitoringCommandService.Handle(command, cancellationToken);
        return result.Match<ActionResult>(
            variable => Ok(ProcessVariableResourceFromEntityAssembler.ToResourceFromEntity(variable)),
            error => ActionResultFromProductionMonitoringErrorAssembler.ToActionResultFromError(error, this));
    }

    private ActionResult UserIdRequired()
    {
        return ActionResultFromProductionMonitoringErrorAssembler.ToActionResultFromError(
            ProductionMonitoringError.InvalidData("The userId of the account is required."), this);
    }
}
