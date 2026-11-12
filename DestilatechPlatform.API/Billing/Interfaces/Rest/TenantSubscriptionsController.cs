using System.Net.Mime;
using DestilatechPlatform.API.Billing.Application.QueryServices;
using DestilatechPlatform.API.Billing.Domain.Model.Queries;
using DestilatechPlatform.API.Billing.Interfaces.Rest.Resources;
using DestilatechPlatform.API.Billing.Interfaces.Rest.Transform;
using Microsoft.AspNetCore.Mvc;

namespace DestilatechPlatform.API.Billing.Interfaces.Rest;

/// <summary>
/// Subscriptions of a tenant, as a sub-resource of the tenant: <c>/api/v1/tenants/{tenantId}/subscriptions</c>.
/// </summary>
[ApiController]
[Route("api/v1/tenants/{tenantId:int}/subscriptions")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Available Subscription endpoints")]
public class TenantSubscriptionsController(ISubscriptionQueryService subscriptionQueryService) : ControllerBase
{
    [HttpGet]
    [EndpointSummary("Get all subscriptions by tenant id (most recent first)")]
    [ProducesResponseType(typeof(IEnumerable<SubscriptionResource>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSubscriptionsByTenantId(int tenantId, CancellationToken cancellationToken)
    {
        var subscriptions =
            await subscriptionQueryService.Handle(new GetSubscriptionsByUserIdQuery(tenantId), cancellationToken);
        return Ok(subscriptions.Select(SubscriptionResourceFromEntityAssembler.ToResourceFromEntity));
    }
}
