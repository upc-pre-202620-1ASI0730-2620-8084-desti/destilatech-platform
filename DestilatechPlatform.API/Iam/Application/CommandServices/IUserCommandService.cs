using DestilatechPlatform.API.Iam.Application.Internal.OutboundServices;
using DestilatechPlatform.API.Iam.Domain.Model.Aggregates;
using DestilatechPlatform.API.Iam.Domain.Model.Commands;
using DestilatechPlatform.API.Shared.Application.Model;

namespace DestilatechPlatform.API.Iam.Application.CommandServices;

public interface IUserCommandService
{
    /// <summary>
    /// Registers an account with a hashed password; the 14-day free trial starts with the registration.
    /// </summary>
    Task<Result<User>> Handle(RegisterUserCommand command, CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifies the credentials and issues an access token.
    /// </summary>
    Task<Result<(User User, AccessToken Token)>> Handle(SignInCommand command,
        CancellationToken cancellationToken = default);
}
