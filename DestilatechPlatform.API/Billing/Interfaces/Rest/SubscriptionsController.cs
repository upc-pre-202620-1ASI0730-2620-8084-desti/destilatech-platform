using DestilatechPlatform.API.Billing.Domain.Model.Errors;
using System.Net.Mime;
using DestilatechPlatform.API.Billing.Application.CommandServices;
using DestilatechPlatform.API.Billing.Application.QueryServices;
using DestilatechPlatform.API.Billing.Domain.Model.Commands;
using DestilatechPlatform.API.Billing.Domain.Model.Queries;
using DestilatechPlatform.API.Billing.Interfaces.Rest.Resources;
using DestilatechPlatform.API.Billing.Interfaces.Rest.Transform;
using Microsoft.AspNetCore.Mvc;

namespace DestilatechPlatform.API.Billing.Interfaces.Rest;

[ApiController]
[Route("api/v1/subscriptions")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Available Subscription endpoints")]

public class SubscriptionsController(
    ISubscriptionQueryService subscriptionQueryService,
    ISubscriptionCommandService subscriptionCommandService) : ControllerBase
{
    [HttpGet]
    [EndpointSummary("Get subscriptions by user id (most recent first)")]
    [ProducesResponseType(typeof(IEnumerable<SubscriptionResource>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSubscriptionsByUserId([FromQuery] int userId,
        CancellationToken cancellationToken)
    {
        var subscriptions =
            await subscriptionQueryService.Handle(new GetSubscriptionsByUserIdQuery(userId), cancellationToken);
        return Ok(subscriptions.Select(SubscriptionResourceFromEntityAssembler.ToResourceFromEntity));
    }

    [HttpPatch("{id:int}")]
    [EndpointSummary("Cancel a subscription")]
    [ProducesResponseType(typeof(SubscriptionResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateSubscriptionStatus(int id,
        [FromBody] UpdateSubscriptionStatusResource resource, CancellationToken cancellationToken)
    {
        // El front solo manda { "status": "CANCELLED" }
        if (!string.Equals(resource.Status, "CANCELLED", StringComparison.OrdinalIgnoreCase))
            return Problem("Solo se permite el estado CANCELLED.", statusCode: StatusCodes.Status400BadRequest,
                title: "Invalid status");

        var result = await subscriptionCommandService.Handle(new CancelSubscriptionCommand(id), cancellationToken);
        return result.Match<IActionResult>(
            subscription => Ok(SubscriptionResourceFromEntityAssembler.ToResourceFromEntity(subscription)),
            _ => this.ToProblem((BillingError)result.Error!));
    }
}
