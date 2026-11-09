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
    [EndpointSummary("Get payments by user id or by reference (most recent first)")]
    [ProducesResponseType(typeof(IEnumerable<PaymentResource>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetPayments([FromQuery] int? userId, [FromQuery] string? reference,
        CancellationToken cancellationToken)
    {
        // Con ?reference= devuelve una lista de 0 o 1 elementos, como json-server
        if (!string.IsNullOrWhiteSpace(reference))
        {
            var payment = await paymentQueryService.Handle(new GetPaymentByReferenceQuery(reference),
                cancellationToken);
            var found = new List<PaymentResource>();
            if (payment is not null) found.Add(PaymentResourceFromEntityAssembler.ToResourceFromEntity(payment));
            return Ok(found);
        }

        if (userId.HasValue)
        {
            var payments = await paymentQueryService.Handle(new GetPaymentsByUserIdQuery(userId.Value),
                cancellationToken);
            return Ok(payments.Select(PaymentResourceFromEntityAssembler.ToResourceFromEntity));
        }

        return Problem("Indica userId o reference.", statusCode: StatusCodes.Status400BadRequest,
            title: "Missing filter");
    }
}