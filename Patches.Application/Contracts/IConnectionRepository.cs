using System.Linq.Expressions;
using Patches.Domain.Entities;

namespace Patches.Application.Contracts;

public interface IConnectionRepository
{
    void Add(Connection entity);
    void Remove(Connection entity);
    IQueryable<Connection> FindByCondition(Expression<Func<Connection, bool>> condition, bool trackChanges = false);
    Task<Connection?> FindByInputIdAsync(int inputId, bool trackChanges = false, CancellationToken ct = default);
}
