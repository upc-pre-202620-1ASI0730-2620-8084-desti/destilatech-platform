using System.Net.Mime;
using DestilatechPlatform.API.Billing.Application.QueryServices;
using DestilatechPlatform.API.Billing.Domain.Model.Queries;
using DestilatechPlatform.API.Billing.Interfaces.Rest.Resources;
using DestilatechPlatform.API.Billing.Interfaces.Rest.Transform;
using Microsoft.AspNetCore.Mvc;

namespace DestilatechPlatform.API.Billing.Interfaces.Rest;

[ApiController]
[Route("api/v1/plans")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Available Plan endpoints")]

public class PlansController(IPlanQueryService planQueryService) : ControllerBase
{
    [HttpGet]
    [EndpointSummary("Get all plans")]
    [ProducesResponseType(typeof(IEnumerable<PlanResource>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllPlans(CancellationToken cancellationToken)
    {
        var plans = await planQueryService.Handle(new GetAllPlansQuery(), cancellationToken);
        return Ok(plans.Select(PlanResourceFromEntityAssembler.ToResourceFromEntity));
    }
}