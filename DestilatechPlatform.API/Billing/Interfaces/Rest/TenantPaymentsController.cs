using System.Net.Mime;
using DestilatechPlatform.API.Billing.Application.QueryServices;
using DestilatechPlatform.API.Billing.Domain.Model.Queries;
using DestilatechPlatform.API.Billing.Interfaces.Rest.Resources;
using DestilatechPlatform.API.Billing.Interfaces.Rest.Transform;
using Microsoft.AspNetCore.Mvc;

namespace DestilatechPlatform.API.Billing.Interfaces.Rest;

/// <summary>
/// Payments of a tenant, as a sub-resource of the tenant: <c>/api/v1/tenants/{tenantId}/payments</c>.
/// </summary>
[ApiController]
[Route("api/v1/tenants/{tenantId:int}/payments")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Available Payment endpoints")]
public class TenantPaymentsController(IPaymentQueryService paymentQueryService) : ControllerBase
{
    [HttpGet]
    [EndpointSummary("Get all payments by tenant id (most recent first)")]
    [ProducesResponseType(typeof(IEnumerable<PaymentResource>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPaymentsByTenantId(int tenantId, CancellationToken cancellationToken)
    {
        var payments = await paymentQueryService.Handle(new GetPaymentsByUserIdQuery(tenantId), cancellationToken);
        return Ok(payments.Select(PaymentResourceFromEntityAssembler.ToResourceFromEntity));
    }
}
