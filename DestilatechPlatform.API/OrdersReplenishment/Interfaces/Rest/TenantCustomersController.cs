using System.Net.Mime;
using DestilatechPlatform.API.OrdersReplenishment.Application.Services;
using DestilatechPlatform.API.OrdersReplenishment.Domain.Model.Queries;
using DestilatechPlatform.API.OrdersReplenishment.Interfaces.Rest.Resources;
using DestilatechPlatform.API.OrdersReplenishment.Interfaces.Rest.Transform;
using Microsoft.AspNetCore.Mvc;

namespace DestilatechPlatform.API.OrdersReplenishment.Interfaces.Rest;

/// <summary>
/// Customers of a tenant, as a sub-resource of the tenant: <c>/api/v1/tenants/{tenantId}/customers</c>.
/// </summary>
[ApiController]
[Route("api/v1/tenants/{tenantId:int}/customers")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Customers")]
public class TenantCustomersController(
    ICustomerQueryService customerQueryService,
    IOrderQueryService orderQueryService) : ControllerBase
{
    [HttpGet]
    [EndpointSummary("Get all customers by tenant id")]
    [EndpointDescription("Returns the customers of the tenant sorted by name.")]
    [ProducesResponseType(typeof(IEnumerable<CustomerResource>), StatusCodes.Status200OK)]
    public async Task<ActionResult> GetCustomersByTenantId(int tenantId, CancellationToken cancellationToken)
    {
        var customers = await customerQueryService.Handle(new GetAllCustomersByUserIdQuery(tenantId),
            cancellationToken);
        return Ok(customers.Select(CustomerResourceFromEntityAssembler.ToResourceFromEntity));
    }

    [HttpGet("{customerId:int}")]
    [EndpointSummary("Get a customer of the tenant")]
    [ProducesResponseType(typeof(CustomerResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetCustomerById(int tenantId, int customerId, CancellationToken cancellationToken)
    {
        var customer = await customerQueryService.Handle(new GetCustomerByIdQuery(customerId, tenantId),
            cancellationToken);
        if (customer is null) return NotFound();
        return Ok(CustomerResourceFromEntityAssembler.ToResourceFromEntity(customer));
    }

    [HttpGet("{customerId:int}/orders")]
    [EndpointSummary("Get all orders by customer id")]
    [EndpointDescription("Returns the orders of a customer of the tenant, newest first.")]
    [ProducesResponseType(typeof(IEnumerable<OrderResource>), StatusCodes.Status200OK)]
    public async Task<ActionResult> GetOrdersByCustomerId(int tenantId, int customerId,
        CancellationToken cancellationToken)
    {
        var orders = await orderQueryService.Handle(new GetOrdersByCustomerIdQuery(customerId, tenantId),
            cancellationToken);
        return Ok(orders.Select(OrderResourceFromEntityAssembler.ToResourceFromEntity));
    }
}
