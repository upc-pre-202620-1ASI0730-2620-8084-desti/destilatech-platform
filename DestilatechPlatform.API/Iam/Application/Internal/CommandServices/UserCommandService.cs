using System.Net.Mail;
using DestilatechPlatform.API.Iam.Application.CommandServices;
using DestilatechPlatform.API.Iam.Domain.Model.Aggregates;
using DestilatechPlatform.API.Iam.Domain.Model.Commands;
using DestilatechPlatform.API.Iam.Domain.Model.Errors;
using DestilatechPlatform.API.Iam.Domain.Model.ValueObjects;
using DestilatechPlatform.API.Iam.Domain.Repositories;
using DestilatechPlatform.API.Shared.Application.Patterns;
using DestilatechPlatform.API.Shared.Domain.Repositories;

namespace DestilatechPlatform.API.Iam.Application.Internal.CommandServices;

public class UserCommandService(IUserRepository userRepository, IUnitOfWork unitOfWork) : IUserCommandService
{
    public async Task<Result<User, IamError>> Handle(RegisterUserCommand command,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(command.FullName) ||
            string.IsNullOrWhiteSpace(command.BusinessName) ||
            string.IsNullOrWhiteSpace(command.Email) ||
            !MailAddress.TryCreate(command.Email.Trim(), out _) ||
            !Enum.TryParse<EBusinessType>(command.BusinessType, true, out var userType) ||
            !Enum.IsDefined(userType))
            return new Result<User, IamError>.Failure(IamError.InvalidData);

        var email = command.Email.Trim().ToLowerInvariant();
        if (await userRepository.ExistsByEmailAsync(email, cancellationToken))
            return new Result<User, IamError>.Failure(IamError.EmailAlreadyExists);

        var user = new User(command.FullName, email, command.BusinessName, userType, command.PreferredPlanCode);
        await userRepository.AddAsync(user, cancellationToken);
        await unitOfWork.CompleteAsync(cancellationToken);

        return new Result<User, IamError>.Success(user);
    }
}