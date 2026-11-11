using DestilatechPlatform.API.Iam.Application.QueryServices;
using DestilatechPlatform.API.Iam.Domain.Model.Aggregates;
using DestilatechPlatform.API.Iam.Domain.Model.Queries;
using DestilatechPlatform.API.Iam.Domain.Repositories;

namespace DestilatechPlatform.API.Iam.Application.Internal.QueryServices;

public class UserQueryService(IUserRepository userRepository) : IUserQueryService
{
    public async Task<IEnumerable<User>> Handle(GetAllUsersQuery query, CancellationToken cancellationToken = default)
    {
        return await userRepository.ListAsync(cancellationToken);
    }

    public async Task<User?> Handle(GetUserByIdQuery query, CancellationToken cancellationToken = default)
    {
        return await userRepository.FindByIdAsync(query.UserId, cancellationToken);
    }
}