using System.Net.Mime;
using DestilatechPlatform.API.OrdersReplenishment.Application.Services;
using DestilatechPlatform.API.OrdersReplenishment.Domain.Model;
using DestilatechPlatform.API.OrdersReplenishment.Interfaces.Rest.Resources;
using DestilatechPlatform.API.OrdersReplenishment.Interfaces.Rest.Transform;
using Microsoft.AspNetCore.Mvc;

namespace DestilatechPlatform.API.OrdersReplenishment.Interfaces.Rest;

/// <summary>
/// Customers of a producer or trader (US17).
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Customers")]
public class CustomersController(ICustomerCommandService customerCommandService) : ControllerBase
{
    [HttpPost]
    [EndpointSummary("Register a customer")]
    [EndpointDescription("Registers a customer. It needs a name and at least a phone number or an email.")]
    [ProducesResponseType(typeof(CustomerResource), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> RegisterCustomer([FromBody] CreateCustomerResource resource,
        CancellationToken cancellationToken = default)
    {
        var accountId = resource.TenantId;
        if (accountId <= 0)
        {
            return TenantIdRequired();
        }

        try
        {
            var command = RegisterCustomerCommandFromResourceAssembler.ToCommandFromResource(resource, accountId);
            var result = await customerCommandService.Handle(command, cancellationToken);
            return result.Match<ActionResult>(
                customer => CreatedAtAction(nameof(TenantCustomersController.GetCustomerById), "TenantCustomers",
                    new { tenantId = accountId, customerId = customer.Id },
                    CustomerResourceFromEntityAssembler.ToResourceFromEntity(customer)),
                error => ActionResultFromOrdersReplenishmentErrorAssembler.ToActionResultFromError(error, this));
        }
        catch (ArgumentException exception)
        {
            return ActionResultFromOrdersReplenishmentErrorAssembler.ToActionResultFromError(
                OrdersReplenishmentError.InvalidData(exception.Message), this);
        }
    }

    [HttpPut("{id:int}")]
    [EndpointSummary("Update a customer")]
    [ProducesResponseType(typeof(CustomerResource), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> UpdateCustomer(int id, [FromBody] UpdateCustomerResource resource,
        CancellationToken cancellationToken = default)
    {
        var accountId = resource.TenantId;
        if (accountId <= 0)
        {
            return TenantIdRequired();
        }

        try
        {
            var command = UpdateCustomerCommandFromResourceAssembler.ToCommandFromResource(id, resource, accountId);
            var result = await customerCommandService.Handle(command, cancellationToken);
            return result.Match<ActionResult>(
                customer => Ok(CustomerResourceFromEntityAssembler.ToResourceFromEntity(customer)),
                error => ActionResultFromOrdersReplenishmentErrorAssembler.ToActionResultFromError(error, this));
        }
        catch (ArgumentException exception)
        {
            return ActionResultFromOrdersReplenishmentErrorAssembler.ToActionResultFromError(
                OrdersReplenishmentError.InvalidData(exception.Message), this);
        }
    }

    private ActionResult TenantIdRequired()
    {
        return ActionResultFromOrdersReplenishmentErrorAssembler.ToActionResultFromError(
            OrdersReplenishmentError.InvalidData("The tenantId is required."), this);
    }
}
