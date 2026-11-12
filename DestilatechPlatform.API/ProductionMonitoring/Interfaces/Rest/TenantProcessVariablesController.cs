using System.Net.Mime;
using DestilatechPlatform.API.ProductionMonitoring.Application.Services;
using DestilatechPlatform.API.ProductionMonitoring.Domain.Model.Queries;
using DestilatechPlatform.API.ProductionMonitoring.Interfaces.Rest.Resources;
using DestilatechPlatform.API.ProductionMonitoring.Interfaces.Rest.Transform;
using Microsoft.AspNetCore.Mvc;

namespace DestilatechPlatform.API.ProductionMonitoring.Interfaces.Rest;

/// <summary>
/// Process variables of a tenant, as a sub-resource of the tenant:
/// <c>/api/v1/tenants/{tenantId}/process-variables</c>.
/// </summary>
[ApiController]
[Route("api/v1/tenants/{tenantId:int}/process-variables")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Production")]
public class TenantProcessVariablesController(IProductionQueryService productionQueryService) : ControllerBase
{
    [HttpGet]
    [EndpointSummary("Get all process variables by tenant id")]
    [ProducesResponseType(typeof(IEnumerable<ProcessVariableResource>), StatusCodes.Status200OK)]
    public async Task<ActionResult> GetProcessVariablesByTenantId(int tenantId, CancellationToken cancellationToken)
    {
        var variables = await productionQueryService.Handle(new GetAllProcessVariablesByUserIdQuery(tenantId),
            cancellationToken);
        return Ok(variables.Select(ProcessVariableResourceFromEntityAssembler.ToResourceFromEntity));
    }
}
