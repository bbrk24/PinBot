using DataOnion.db;
using System.Linq.Expressions;

namespace PinBot.Util;

public static class EFCoreServiceExtensions
{
    public static async Task<TEntity> UpsertAsync<TEntity, TIndex, TContext>(
        this IEFCoreService<TContext> efCoreService,
        TIndex id,
        Action<TEntity> applyChanges
    )
        where TEntity : class, IEntity<TIndex>, new()
    {
        var existing = await efCoreService.FetchAsync<TEntity, TIndex>(id);

        if (existing is null)
        {
            var created = new TEntity();
            applyChanges(created);
            return await efCoreService.CreateAsync(created);
        }
        else
        {
            applyChanges(existing);
            return await efCoreService.UpdateAsync(id, existing);
        }
    }

    public static async Task<TEntity> UpsertAsync<TEntity, TIndex, TContext>(
        this IEFCoreService<TContext> efCoreService,
        Expression<Func<TEntity, bool>> matcher,
        Action<TEntity> applyChanges
    )
        where TEntity : class, IEntity<TIndex>, new()
    {
        var existing = await efCoreService.FetchAsync(matcher);

        if (existing is null)
        {
            var created = new TEntity();
            applyChanges(created);
            return await efCoreService.CreateAsync(created);
        }
        else
        {
            applyChanges(existing);
            return await efCoreService.UpdateAsync(existing.Id, existing);
        }
    }
}
