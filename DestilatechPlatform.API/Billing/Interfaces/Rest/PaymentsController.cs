using System.Net.Mime;
using DestilatechPlatform.API.Billing.Application.QueryServices;
using DestilatechPlatform.API.Billing.Domain.Model.Queries;
using DestilatechPlatform.API.Billing.Interfaces.Rest.Resources;
using DestilatechPlatform.API.Billing.Interfaces.Rest.Transform;
using Microsoft.AspNetCore.Mvc;

namespace DestilatechPlatform.API.Billing.Interfaces.Rest;

[ApiController]
[Route("api/v1/payments")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Available Payment endpoints")]

public class PaymentsController(IPaymentQueryService paymentQueryService) : ControllerBase
{
    [HttpGet]
    [EndpointSummary("Get a payment by its reference")]
    [ProducesResponseType(typeof(IEnumerable<PaymentResource>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetPaymentsByReference([FromQuery] string? reference,
        CancellationToken cancellationToken)
    {
        // Returns a list of 0 or 1 payments; the payments of a tenant are listed in TenantPaymentsController.
        if (!string.IsNullOrWhiteSpace(reference))
        {
            var payment = await paymentQueryService.Handle(new GetPaymentByReferenceQuery(reference),
                cancellationToken);
            var found = new List<PaymentResource>();
            if (payment is not null) found.Add(PaymentResourceFromEntityAssembler.ToResourceFromEntity(payment));
            return Ok(found);
        }

        return Problem("Indica reference.", statusCode: StatusCodes.Status400BadRequest,
            title: "Missing filter");
    }
}