using System.Net.Mime;
using DestilatechPlatform.API.AlertsNotifications.Application.Services;
using DestilatechPlatform.API.AlertsNotifications.Domain.Model.Commands;
using DestilatechPlatform.API.AlertsNotifications.Domain.Model.Queries;
using DestilatechPlatform.API.AlertsNotifications.Domain.Model.ValueObjects;
using DestilatechPlatform.API.AlertsNotifications.Interfaces.Rest.Resources;
using DestilatechPlatform.API.AlertsNotifications.Interfaces.Rest.Transform;
using DestilatechPlatform.API.Resources;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace DestilatechPlatform.API.AlertsNotifications.Interfaces.Rest;


[ApiController]
[Route("api/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Alerts")]
public class AlertsController(
    IAlertCommandService alertCommandService,
    IAlertQueryService alertQueryService,
    IStringLocalizer<SharedResource> localizer,
    ILogger<AlertsController> logger)
    : ControllerBase
{

    [HttpPost]
    [ProducesResponseType(typeof(AlertResource), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> RaiseAlert([FromBody] RaiseAlertResource resource,
        CancellationToken cancellationToken)
    {
        try
        {
            var raiseAlertCommand = RaiseAlertCommandFromResourceAssembler.ToCommandFromResource(resource);
            var result = await alertCommandService.Handle(raiseAlertCommand, cancellationToken);
            return ActionResultFromRaiseAlertResultAssembler.ToActionResultFromRaiseAlertResult(
                result,
                this,
                localizer,
                nameof(GetAlertById));
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning(ex, "Validation failed while raising an alert for account {AccountId}",
                resource.AccountId);
            return BadRequest(localizer["InvalidAlertRequest"].Value);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error while raising an alert for account {AccountId}",
                resource.AccountId);
            return Problem(
                title: localizer["UnexpectedServerError"].Value,
                detail: localizer["UnexpectedErrorRaisingAlert"].Value,
                statusCode: 500);
        }
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<AlertResource>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> GetAlertsFromQuery([FromQuery] int accountId,
        [FromQuery] string? type = null,
        [FromQuery] string? status = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var getAllAlertsByAccountIdQuery = new GetAllAlertsByAccountIdQuery(
                new AccountId(accountId),
                string.IsNullOrWhiteSpace(type) ? null : AlertValueAssembler.ToEnumFromValue<AlertType>(type),
                string.IsNullOrWhiteSpace(status) ? null : AlertValueAssembler.ToEnumFromValue<AlertStatus>(status));
            var result = await alertQueryService.Handle(getAllAlertsByAccountIdQuery, cancellationToken);
            var resources = result.Select(AlertResourceFromEntityAssembler.ToResourceFromEntity);
            return Ok(resources);
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning(ex, "Validation failed while querying alerts for account {AccountId}", accountId);
            return BadRequest(localizer["InvalidAlertQuery"].Value);
        }
    }
    
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(AlertResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetAlertById(int id, CancellationToken cancellationToken = default)
    {
        var getAlertByIdQuery = new GetAlertByIdQuery(id);
        var result = await alertQueryService.Handle(getAlertByIdQuery, cancellationToken);
        if (result is null) return NotFound();
        var resource = AlertResourceFromEntityAssembler.ToResourceFromEntity(result);
        return Ok(resource);
    }
    
    [HttpPatch("{id}/attended")]
    [ProducesResponseType(typeof(AlertResource), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> MarkAlertAsAttended(int id, CancellationToken cancellationToken = default)
    {
        var markAlertAsAttendedCommand = new MarkAlertAsAttendedCommand(id);
        var result = await alertCommandService.Handle(markAlertAsAttendedCommand, cancellationToken);
        return ActionResultFromMarkAlertAsAttendedResultAssembler.ToActionResultFromMarkAlertAsAttendedResult(
            result,
            this,
            localizer);
    }
}
