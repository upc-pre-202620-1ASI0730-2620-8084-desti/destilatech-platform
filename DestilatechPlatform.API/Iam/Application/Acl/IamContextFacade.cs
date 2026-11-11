using DestilatechPlatform.API.Iam.Domain.Model.ValueObjects;
using DestilatechPlatform.API.Iam.Domain.Repositories;
using DestilatechPlatform.API.Iam.Interfaces.Acl;

namespace DestilatechPlatform.API.Iam.Application.Acl;

public class IamContextFacade(IUserRepository userRepository) : IIamContextFacade
{
    public async Task<bool> UserExistsAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await userRepository.FindByIdAsync(userId, cancellationToken) is not null;
    }

    public async Task<bool> ExtendUserAccessAsync(int userId, DateTimeOffset until,
        CancellationToken cancellationToken = default)
    {
        var user = await userRepository.FindByIdAsync(userId, cancellationToken);
        if (user is null) return false;

        user.ExtendAccess(until);
        userRepository.Update(user);
        // No guarda: el commit lo hace quien llama (Billing), en una sola transacción.
        return true;
    }

    public async Task<string?> FetchBusinessTypeAsync(int userId, CancellationToken cancellationToken = default)
    {
        var user = await userRepository.FindByIdAsync(userId, cancellationToken);
        return user?.BusinessType switch
        {
            EBusinessType.Producer => "PRODUCER",
            EBusinessType.Retailer => "RETAILER",
            _ => null
        };
    }
}
