using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Patches.Application.Contracts;
using Patches.Domain.Entities;
using Patches.Infrastructure.Data;

namespace Patches.Infrastructure.Repositories;

public class ConnectionRepository(ApplicationDbContext context) : IConnectionRepository
{
    private readonly ApplicationDbContext context = context;
    public void Add(Connection entity)
    {
        context.Connections.Add(entity);
    }

    public void Remove(Connection entity)
    {
        context.Connections.Remove(entity);
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

    public async Task<Connection?> FindByInputIdAsync(
        int inputId,
        bool trackChanges = false,
        CancellationToken ct = default)
    {
        return await FindByCondition(c => c.InputId == inputId, trackChanges)
            .FirstOrDefaultAsync(ct);
    }
}
