namespace Patches.Application.Contracts;

public interface IRepository<TEntity>
{
    void Add(TEntity entity);
    IEnumerable<TEntity> GetAll();
}
