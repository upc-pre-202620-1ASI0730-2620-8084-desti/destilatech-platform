using System.Net.Mail;
using DestilatechPlatform.API.Iam.Application.CommandServices;
using DestilatechPlatform.API.Iam.Application.Internal.OutboundServices;
using DestilatechPlatform.API.Iam.Domain.Model.Aggregates;
using DestilatechPlatform.API.Iam.Domain.Model.Commands;
using DestilatechPlatform.API.Iam.Domain.Model.Errors;
using DestilatechPlatform.API.Iam.Domain.Model.ValueObjects;
using DestilatechPlatform.API.Iam.Domain.Repositories;
using DestilatechPlatform.API.Shared.Application.Model;
using DestilatechPlatform.API.Shared.Domain.Repositories;

namespace DestilatechPlatform.API.Iam.Application.Internal.CommandServices;

public class UserCommandService(
    IUserRepository userRepository,
    IHashingService hashingService,
    ITokenService tokenService,
    IUnitOfWork unitOfWork) : IUserCommandService
{
    public const int MinimumPasswordLength = 8;

    public async Task<Result<User>> Handle(RegisterUserCommand command,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(command.FullName) ||
            string.IsNullOrWhiteSpace(command.BusinessName) ||
            string.IsNullOrWhiteSpace(command.Email) ||
            !MailAddress.TryCreate(command.Email.Trim(), out _) ||
            !Enum.TryParse<EBusinessType>(command.BusinessType, true, out var userType) ||
            !Enum.IsDefined(userType))
            return Result<User>.Failure(IamError.InvalidData, "The user data is invalid.");

        if (string.IsNullOrWhiteSpace(command.Password) || command.Password.Length < MinimumPasswordLength)
            return Result<User>.Failure(IamError.WeakPassword,
                $"The password must have at least {MinimumPasswordLength} characters.");

        var email = command.Email.Trim().ToLowerInvariant();
        if (await userRepository.ExistsByEmailAsync(email, cancellationToken))
            return Result<User>.Failure(IamError.EmailAlreadyExists, "A user with this email already exists.");

        var passwordHash = hashingService.HashPassword(command.Password);
        var user = new User(command.FullName, email, passwordHash, command.BusinessName, userType,
            command.PreferredPlanCode);
        await userRepository.AddAsync(user, cancellationToken);
        await unitOfWork.CompleteAsync(cancellationToken);
        return Result<User>.Success(user);
    }

    public async Task<Result<(User User, AccessToken Token)>> Handle(SignInCommand command,
        CancellationToken cancellationToken = default)
    {
        var email = command.Email?.Trim().ToLowerInvariant() ?? string.Empty;
        var user = string.IsNullOrEmpty(email) ? null : await userRepository.FindByEmailAsync(email, cancellationToken);

        // The same answer for an unknown email and a wrong password, so the API does not reveal which accounts exist.
        if (user is null || !hashingService.VerifyPassword(command.Password ?? string.Empty, user.PasswordHash))
            return Result<(User User, AccessToken Token)>.Failure(IamError.InvalidCredentials,
                "The email or the password is not correct.");

        return Result<(User User, AccessToken Token)>.Success((user, tokenService.GenerateToken(user)));
    }
}
