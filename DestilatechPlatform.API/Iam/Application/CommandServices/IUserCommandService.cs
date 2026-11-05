using DestilatechPlatform.API.Iam.Domain.Model.Aggregates;
using DestilatechPlatform.API.Iam.Domain.Model.Commands;
using DestilatechPlatform.API.Iam.Domain.Model.Errors;
using DestilatechPlatform.API.Shared.Application.Patterns;

namespace DestilatechPlatform.API.Iam.Application.CommandServices;

public interface IUserCommandService
{
    Task<Result<User, IamError>> Handle(RegisterUserCommand command, CancellationToken cancellationToken = default);

}