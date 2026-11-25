using System.Net.Mime;
using DestilatechPlatform.API.Iam.Application.CommandServices;
using DestilatechPlatform.API.Iam.Application.QueryServices;
using DestilatechPlatform.API.Iam.Domain.Model.Errors;
using DestilatechPlatform.API.Iam.Domain.Model.Queries;
using DestilatechPlatform.API.Iam.Infrastructure.Pipeline.Middleware.Attributes;
using DestilatechPlatform.API.Iam.Infrastructure.Pipeline.Middleware.Extensions;
using DestilatechPlatform.API.Iam.Interfaces.Rest.Resources;
using DestilatechPlatform.API.Iam.Interfaces.Rest.Transform;
using Microsoft.AspNetCore.Mvc;

namespace DestilatechPlatform.API.Iam.Interfaces.Rest;

[Authorize]
[ApiController]
[Route("api/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Available User endpoints")]
public class UsersController(IUserCommandService userCommandService, IUserQueryService userQueryService)
    : ControllerBase
{
    [HttpPost]
    [AllowAnonymous]
    [EndpointSummary("Register user (sign up)")]
    [EndpointDescription("Creates an account with a 14-day free trial. Sign in with POST /api/v1/tokens to get an access token.")]
    [ProducesResponseType(typeof(UserResource), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> RegisterUser([FromBody] RegisterUserResource resource,
        CancellationToken cancellationToken)
    {
        var command = RegisterUserCommandFromResourceAssembler.ToCommandFromResource(resource);
        var result = await userCommandService.Handle(command, cancellationToken);

        return result.Match<IActionResult>(
            user => CreatedAtAction(nameof(GetUserById), new { id = user.Id },
                UserResourceFromEntityAssembler.ToResourceFromEntity(user)),
            _ => this.ToIamProblem((IamError)result.Error!));
    }

    [HttpGet]
    [EndpointSummary("Get all users")]
    [EndpointDescription("Each account is a tenant, so the list only contains the signed-in account.")]
    [ProducesResponseType(typeof(IEnumerable<UserResource>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult GetAllUsers()
    {
        var user = HttpContext.SignedInUser()!;
        return Ok(new[] { UserResourceFromEntityAssembler.ToResourceFromEntity(user) });
    }

    [HttpGet("{id:int}")]
    [EndpointSummary("Get user by id")]
    [ProducesResponseType(typeof(UserResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUserById(int id, CancellationToken cancellationToken)
    {
        // Another account is answered as not found, so the API does not reveal which ids exist.
        if (HttpContext.SignedInUser()?.Id != id) return this.ToIamProblem(IamError.UserNotFound);
        var user = await userQueryService.Handle(new GetUserByIdQuery(id), cancellationToken);
        if (user is null) return this.ToIamProblem(IamError.UserNotFound);
        return Ok(UserResourceFromEntityAssembler.ToResourceFromEntity(user));
    }
}
