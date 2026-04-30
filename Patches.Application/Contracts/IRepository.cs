using System.Linq.Expressions;

namespace Patches.Application.Contracts;

public interface IRepository<TEntity, TId>
{
    void Add(TEntity entity);

    void Remove(TEntity entity);

    IEnumerable<TEntity> GetAll();
    
    Task<TEntity?> FindByIdAsync(TId id, bool trackChanges = false, CancellationToken ct = default);

    IQueryable<TEntity> FindByCondition(Expression<Func<TEntity,bool>> condition, bool trackChanges = false);
}
