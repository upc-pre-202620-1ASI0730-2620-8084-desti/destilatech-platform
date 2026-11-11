using System.Net.Mime;
using DestilatechPlatform.API.AlertsNotifications.Application.Services;
using DestilatechPlatform.API.AlertsNotifications.Domain.Model;
using DestilatechPlatform.API.AlertsNotifications.Domain.Model.Commands;
using DestilatechPlatform.API.AlertsNotifications.Domain.Model.Queries;
using DestilatechPlatform.API.AlertsNotifications.Domain.Model.ValueObjects;
using DestilatechPlatform.API.AlertsNotifications.Interfaces.Rest.Resources;
using DestilatechPlatform.API.AlertsNotifications.Interfaces.Rest.Transform;
using DestilatechPlatform.API.Resources;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using ProblemDetailsFactory = DestilatechPlatform.API.Shared.Interfaces.Rest.ProblemDetails.ProblemDetailsFactory;

namespace DestilatechPlatform.API.AlertsNotifications.Interfaces.Rest;

[ApiController]
[Route("api/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Alerts")]
public class AlertsController(
    IAlertCommandService alertCommandService,
    IAlertQueryService alertQueryService,
    IStringLocalizer<SharedResource> localizer,
    ProblemDetailsFactory problemDetailsFactory,
    ILogger<AlertsController> logger)
    : ControllerBase
{

    [HttpPost]
    [ProducesResponseType(typeof(AlertResource), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> RaiseAlert([FromBody] RaiseAlertResource resource,
        CancellationToken cancellationToken)
    {
        try
        {
            var raiseAlertCommand = RaiseAlertCommandFromResourceAssembler.ToCommandFromResource(resource);
            var result = await alertCommandService.Handle(raiseAlertCommand, cancellationToken);
            return AlertsActionResultAssembler.ToActionResultFromAlertResult(
                this,
                result,
                localizer,
                problemDetailsFactory,
                raisedAlert => CreatedAtAction(nameof(GetAlertById), new { id = raisedAlert.Id.Value },
                    AlertResourceFromEntityAssembler.ToResourceFromEntity(raisedAlert)));
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning(ex, "Validation failed while raising an alert for account {AccountId}",
                resource.AccountId);
            return AlertsActionResultAssembler.ToProblemDetailsFromAlertsError(
                this, AlertsError.InvalidAlertData, localizer, problemDetailsFactory);
        }
    }
    
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<AlertResource>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAlertsFromQuery([FromQuery] int accountId,
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
            return problemDetailsFactory.CreateProblemDetails(this, StatusCodes.Status400BadRequest,
                AlertsError.InvalidAlertData, localizer["InvalidAlertQuery"].Value);
        }
    }
    
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(AlertResource), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAlertById(int id, CancellationToken cancellationToken = default)
    {
        var getAlertByIdQuery = new GetAlertByIdQuery(id);
        var alert = await alertQueryService.Handle(getAlertByIdQuery, cancellationToken);
        return AlertsActionResultAssembler.ToActionResultFromGetAlertByIdResult(
            this,
            alert,
            localizer,
            problemDetailsFactory,
            foundAlert => Ok(AlertResourceFromEntityAssembler.ToResourceFromEntity(foundAlert)));
    }
    
    [HttpPatch("{id:int}/attended")]
    [ProducesResponseType(typeof(AlertResource), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> MarkAlertAsAttended(int id, CancellationToken cancellationToken = default)
    {
        var markAlertAsAttendedCommand = new MarkAlertAsAttendedCommand(id);
        var result = await alertCommandService.Handle(markAlertAsAttendedCommand, cancellationToken);
        return AlertsActionResultAssembler.ToActionResultFromAlertResult(
            this,
            result,
            localizer,
            problemDetailsFactory,
            attendedAlert => Ok(AlertResourceFromEntityAssembler.ToResourceFromEntity(attendedAlert)));
    }
}
