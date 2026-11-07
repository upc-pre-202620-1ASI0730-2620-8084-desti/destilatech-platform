using System.Net.Mime;
using DestilatechPlatform.API.OrdersReplenishment.Application.Services;
using DestilatechPlatform.API.OrdersReplenishment.Domain.Model;
using DestilatechPlatform.API.OrdersReplenishment.Domain.Model.Queries;
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
public class CustomersController(
    ICustomerCommandService customerCommandService,
    ICustomerQueryService customerQueryService) : ControllerBase
{
    [HttpGet]
    [EndpointSummary("List the customers of an account")]
    [EndpointDescription("Returns the customers of the account sorted by name.")]
    [ProducesResponseType(typeof(IEnumerable<CustomerResource>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> GetCustomers([FromQuery] int userId, CancellationToken cancellationToken)
    {
        if (userId <= 0)
        {
            return UserIdRequired();
        }

        var customers = await customerQueryService.Handle(new GetAllCustomersByUserIdQuery(userId), cancellationToken);
        return Ok(customers.Select(CustomerResourceFromEntityAssembler.ToResourceFromEntity));
    }

    [HttpGet("{id:int}")]
    [EndpointSummary("Get a customer")]
    [ProducesResponseType(typeof(CustomerResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetCustomerById(int id, [FromQuery] int userId,
        CancellationToken cancellationToken)
    {
        var customer = await customerQueryService.Handle(new GetCustomerByIdQuery(id, userId), cancellationToken);
        if (customer is null)
        {
            return NotFound();
        }

        return Ok(CustomerResourceFromEntityAssembler.ToResourceFromEntity(customer));
    }

    [HttpPost]
    [EndpointSummary("Register a customer")]
    [EndpointDescription("Registers a customer. It needs a name and at least a phone number or an email.")]
    [ProducesResponseType(typeof(CustomerResource), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> RegisterCustomer([FromBody] CreateCustomerResource resource,
        [FromQuery] int userId = 0, CancellationToken cancellationToken = default)
    {
        var accountId = userId > 0 ? userId : resource.UserId ?? 0;
        if (accountId <= 0)
        {
            return UserIdRequired();
        }

        try
        {
            var command = RegisterCustomerCommandFromResourceAssembler.ToCommandFromResource(resource, accountId);
            var result = await customerCommandService.Handle(command, cancellationToken);
            return result.Fold<ActionResult>(
                customer => CreatedAtAction(nameof(GetCustomerById), new { id = customer.Id, userId = accountId },
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
        [FromQuery] int userId = 0, CancellationToken cancellationToken = default)
    {
        var accountId = userId > 0 ? userId : resource.UserId ?? 0;
        if (accountId <= 0)
        {
            return UserIdRequired();
        }

        try
        {
            var command = UpdateCustomerCommandFromResourceAssembler.ToCommandFromResource(id, resource, accountId);
            var result = await customerCommandService.Handle(command, cancellationToken);
            return result.Fold<ActionResult>(
                customer => Ok(CustomerResourceFromEntityAssembler.ToResourceFromEntity(customer)),
                error => ActionResultFromOrdersReplenishmentErrorAssembler.ToActionResultFromError(error, this));
        }
        catch (ArgumentException exception)
        {
            return ActionResultFromOrdersReplenishmentErrorAssembler.ToActionResultFromError(
                OrdersReplenishmentError.InvalidData(exception.Message), this);
        }
    }

    private ActionResult UserIdRequired()
    {
        return ActionResultFromOrdersReplenishmentErrorAssembler.ToActionResultFromError(
            OrdersReplenishmentError.InvalidData("The userId of the account is required."), this);
    }
}
