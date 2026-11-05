using DestilatechPlatform.API.Iam.Domain.Model.Aggregates;
using DestilatechPlatform.API.Iam.Domain.Repositories;
using DestilatechPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using DestilatechPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DestilatechPlatform.API.Iam.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

public class UserRepository(AppDbContext context) : BaseRepository<User>(context), IUserRepository
{
    public async Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await Context.Set<User>().AnyAsync(u => u.Email == email, cancellationToken);
    }
}