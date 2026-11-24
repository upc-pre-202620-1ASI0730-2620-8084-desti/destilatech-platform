using System.Net.Mime;
using DestilatechPlatform.API.Iam.Application.CommandServices;
using DestilatechPlatform.API.Iam.Domain.Model.Errors;
using DestilatechPlatform.API.Iam.Infrastructure.Pipeline.Middleware.Attributes;
using DestilatechPlatform.API.Iam.Interfaces.Rest.Resources;
using DestilatechPlatform.API.Iam.Interfaces.Rest.Transform;
using Microsoft.AspNetCore.Mvc;

namespace DestilatechPlatform.API.Iam.Interfaces.Rest;

/// <summary>
/// Signing in creates an access token resource (ADR-008 of the course reference, learning-center-platform).
/// </summary>
[Authorize]
[ApiController]
[Route("api/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Available Token endpoints")]
public class TokensController(IUserCommandService userCommandService) : ControllerBase
{
    [HttpPost]
    [AllowAnonymous]
    [EndpointSummary("Create an access token (sign in)")]
    [EndpointDescription("Verifies the email and the password and returns the account, including its free trial, with a JWT bearer token.")]
    [ProducesResponseType(typeof(AuthenticatedUserResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CreateToken([FromBody] SignInResource resource,
        CancellationToken cancellationToken)
    {
        var command = SignInCommandFromResourceAssembler.ToCommandFromResource(resource);
        var result = await userCommandService.Handle(command, cancellationToken);
        return result.Match<IActionResult>(
            signedIn => Ok(AuthenticatedUserResourceFromEntityAssembler.ToResourceFromEntity(signedIn.User,
                signedIn.Token)),
            _ => this.ToIamProblem((IamError)result.Error!));
    }
}
