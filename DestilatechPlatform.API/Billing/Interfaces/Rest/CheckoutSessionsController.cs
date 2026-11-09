using System.Net.Mime;
using DestilatechPlatform.API.Billing.Application.CommandServices;
using DestilatechPlatform.API.Billing.Domain.Model.Commands;
using DestilatechPlatform.API.Billing.Interfaces.Rest.Resources;
using DestilatechPlatform.API.Billing.Interfaces.Rest.Transform;
using Microsoft.AspNetCore.Mvc;

namespace DestilatechPlatform.API.Billing.Interfaces.Rest;

[ApiController]
[Route("api/v1/checkout-sessions")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Available Checkout endpoints")]
public class CheckoutSessionsController(ICheckoutCommandService checkoutCommandService) : ControllerBase
{
    [HttpPost]
    [EndpointSummary("Create a Stripe checkout session for a plan")]
    [ProducesResponseType(typeof(CheckoutSessionResource), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateCheckoutSession([FromBody] CreateCheckoutSessionResource resource,
        CancellationToken cancellationToken)
    {
        var command = new CreateCheckoutSessionCommand(resource.UserId, resource.PlanId);
        var result = await checkoutCommandService.Handle(command, cancellationToken);

        return result.Fold<IActionResult>(
            info => StatusCode(StatusCodes.Status201Created, new CheckoutSessionResource(info.SessionId, info.Url)),
            error => this.ToProblem(error));
    }

    [HttpPost("{sessionId}/confirmation")]
    [EndpointSummary("Confirm a paid checkout session (idempotent)")]
    [ProducesResponseType(typeof(CheckoutConfirmationResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status402PaymentRequired)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> ConfirmCheckoutSession(string sessionId, CancellationToken cancellationToken)
    {
        var result = await checkoutCommandService.Handle(new ConfirmCheckoutSessionCommand(sessionId),
            cancellationToken);

        return result.Fold<IActionResult>(
            confirmation => Ok(new CheckoutConfirmationResource(
                PaymentResourceFromEntityAssembler.ToResourceFromEntity(confirmation.Payment),
                SubscriptionResourceFromEntityAssembler.ToResourceFromEntity(confirmation.Subscription))),
            error => this.ToProblem(error));
    }
}