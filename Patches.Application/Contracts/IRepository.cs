namespace Patches.Application.Contracts;

public interface IRepository<TEntity, TId>
{
    void Add(TEntity entity);
    IEnumerable<TEntity> GetAll();
    
    Task<TEntity?> FindByIdAsync(TId id, CancellationToken ct = default);
}
