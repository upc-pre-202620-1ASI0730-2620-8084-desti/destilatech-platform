using System.Net.Mime;
using DestilatechPlatform.API.Billing.Application.QueryServices;
using DestilatechPlatform.API.Billing.Domain.Model.Queries;
using DestilatechPlatform.API.Billing.Interfaces.Rest.Resources;
using DestilatechPlatform.API.Billing.Interfaces.Rest.Transform;
using Microsoft.AspNetCore.Mvc;
using DestilatechPlatform.API.Iam.Infrastructure.Pipeline.Middleware.Attributes;

namespace DestilatechPlatform.API.Billing.Interfaces.Rest;

[Authorize]
[ApiController]
[Route("api/v1/plans")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Available Plan endpoints")]

public class PlansController(IPlanQueryService planQueryService) : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    [EndpointSummary("Get all plans")]
    [ProducesResponseType(typeof(IEnumerable<PlanResource>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllPlans(CancellationToken cancellationToken)
    {
        var plans = await planQueryService.Handle(new GetAllPlansQuery(), cancellationToken);
        return Ok(plans.Select(PlanResourceFromEntityAssembler.ToResourceFromEntity));
    }
}