using DestilatechPlatform.API.Iam.Domain.Model.Aggregates;
using DestilatechPlatform.API.Shared.Domain.Repositories;

namespace DestilatechPlatform.API.Iam.Domain.Repositories;

public interface IUserRepository : IBaseRepository<User>
{
    Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default);
}