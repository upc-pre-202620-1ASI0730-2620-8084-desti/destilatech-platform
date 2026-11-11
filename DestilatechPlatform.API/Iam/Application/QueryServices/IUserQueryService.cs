using DestilatechPlatform.API.Iam.Domain.Model.Aggregates;
using DestilatechPlatform.API.Iam.Domain.Model.Queries;

namespace DestilatechPlatform.API.Iam.Application.QueryServices;

public interface IUserQueryService
{
    Task<IEnumerable<User>> Handle(GetAllUsersQuery query, CancellationToken cancellationToken = default);
    Task<User?> Handle(GetUserByIdQuery query, CancellationToken cancellationToken = default);
}