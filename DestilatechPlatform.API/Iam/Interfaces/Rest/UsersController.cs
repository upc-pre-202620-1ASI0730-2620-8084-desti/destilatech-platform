using System.Net.Mime;
using DestilatechPlatform.API.Iam.Application.CommandServices;
using DestilatechPlatform.API.Iam.Application.QueryServices;
using DestilatechPlatform.API.Iam.Domain.Model.Errors;
using DestilatechPlatform.API.Iam.Domain.Model.Queries;
using DestilatechPlatform.API.Iam.Interfaces.Rest.Resources;
using DestilatechPlatform.API.Iam.Interfaces.Rest.Transform;
using Microsoft.AspNetCore.Mvc;

namespace DestilatechPlatform.API.Iam.Interfaces.Rest;

[ApiController]
[Route("api/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Available User endpoints")]
public class UsersController(IUserCommandService userCommandService, IUserQueryService userQueryService)
    : ControllerBase
{
    [HttpPost]
    [EndpointSummary("Register user")]
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
            _ => ToProblem((IamError)result.Error!));
    }

    [HttpGet]
    [EndpointSummary("Get all users")]
    [ProducesResponseType(typeof(IEnumerable<UserResource>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllUsers(CancellationToken cancellationToken)
    {
        var users = await userQueryService.Handle(new GetAllUsersQuery(), cancellationToken);
        return Ok(users.Select(UserResourceFromEntityAssembler.ToResourceFromEntity));
    }

    [HttpGet("{id:int}")]
    [EndpointSummary("Get user by id")]
    [ProducesResponseType(typeof(UserResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUserById(int id, CancellationToken cancellationToken)
    {
        var user = await userQueryService.Handle(new GetUserByIdQuery(id), cancellationToken);
        if (user is null) return ToProblem(IamError.UserNotFound);
        return Ok(UserResourceFromEntityAssembler.ToResourceFromEntity(user));
    }

    private ObjectResult ToProblem(IamError error)
    {
        return error switch
        {
            IamError.InvalidData => Problem("Datos inválidos: revisa nombre, correo, negocio y tipo de usuario.",
                statusCode: StatusCodes.Status400BadRequest, title: "Invalid data"),
            IamError.EmailAlreadyExists => Problem("Ya existe un usuario con ese correo.",
                statusCode: StatusCodes.Status409Conflict, title: "Email already exists"),
            IamError.UserNotFound => Problem("Usuario no encontrado.",
                statusCode: StatusCodes.Status404NotFound, title: "User not found"),
            _ => Problem(statusCode: StatusCodes.Status500InternalServerError)
        };
    }
}