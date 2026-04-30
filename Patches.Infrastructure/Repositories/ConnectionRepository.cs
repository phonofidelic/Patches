using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Patches.Application.Contracts;
using Patches.Domain.Entities;
using Patches.Infrastructure.Data;

namespace Patches.Infrastructure.Repositories;

public class ConnectionRepository(ApplicationDbContext context) : IRepository<Connection, int>
{
    private readonly ApplicationDbContext context = context;
    public void Add(Connection entity)
    {
        context.Connections.Add(entity);
    }

    public IQueryable<Connection> FindByCondition(
        Expression<Func<Connection, bool>> condition, 
        bool trackChanges = false)
    {
        return (!trackChanges 
            ? context.Connections.AsNoTracking()
            : context.Connections)
                .Include(c => c.Input)
                .Include(c => c.Output)
                .Where(condition);
    }

    public async Task<Connection?> FindByIdAsync(
        int id,
        bool trackChanges = false,
        CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Connection> GetAll()
    {
        throw new NotImplementedException();
    }
}
