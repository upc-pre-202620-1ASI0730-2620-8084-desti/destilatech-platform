using System.Linq.Expressions;
using DestilatechPlatform.API.Shared.Domain.Repositories;
using DestilatechPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using Microsoft.EntityFrameworkCore;

namespace DestilatechPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

public class BaseRepository<TEntity>(AppDbContext context) : IBaseRepository<TEntity> where TEntity : class
{
    protected readonly AppDbContext Context = context;

    public async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await Context.Set<TEntity>().AddAsync(entity, cancellationToken);
    }

    public async Task<TEntity?> FindByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await Context.Set<TEntity>().FindAsync([ToKeyValue(id)], cancellationToken);
    }

    public async Task<TEntity?> FindByIdAsNoTrackingAsync(int id, CancellationToken cancellationToken = default)
    {
        return await Context.Set<TEntity>()
            .AsNoTracking()
            .FirstOrDefaultAsync(BuildKeyPredicate(id), cancellationToken);
    }

    public void Update(TEntity entity)
    {
        Context.Set<TEntity>().Update(entity);
    }

    public void Remove(TEntity entity)
    {
        Context.Set<TEntity>().Remove(entity);
    }

    public async Task<IEnumerable<TEntity>> ListAsync(CancellationToken cancellationToken = default)
    {
        return await Context.Set<TEntity>().ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<TEntity>> ListAsNoTrackingAsync(CancellationToken cancellationToken = default)
    {
        return await Context.Set<TEntity>()
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    private object ToKeyValue(int id)
    {
        var keyType = Context.Model.FindEntityType(typeof(TEntity))!.FindPrimaryKey()!.Properties[0].ClrType;
        return keyType == typeof(int) ? id : Activator.CreateInstance(keyType, id)!;
    }

    private Expression<Func<TEntity, bool>> BuildKeyPredicate(int id)
    {
        var keyProperty = Context.Model.FindEntityType(typeof(TEntity))!.FindPrimaryKey()!.Properties[0];
        var parameter = Expression.Parameter(typeof(TEntity), "e");
        var body = Expression.Equal(
            Expression.Property(parameter, keyProperty.Name),
            Expression.Constant(ToKeyValue(id), keyProperty.ClrType));
        return Expression.Lambda<Func<TEntity, bool>>(body, parameter);
    }
}
