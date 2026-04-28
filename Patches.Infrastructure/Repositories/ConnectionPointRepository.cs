using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Patches.Application.Contracts;
using Patches.Domain.Entities;
using Patches.Infrastructure.Data;

namespace Patches.Infrastructure.Repositories;

public class ConnectionPointRepository(ApplicationDbContext context) : IRepository<ConnectionPoint, int>
{
    private readonly ApplicationDbContext context = context;

    public void Add(ConnectionPoint entity)
    {
        context.ConnectionPoints.Add(entity);
    }

    public IQueryable<ConnectionPoint> FindByCondition(
        Expression<Func<ConnectionPoint, bool>> condition, 
        bool trackChanges = false)
    {
        return (!trackChanges 
            ? context.ConnectionPoints.AsNoTracking() 
            : context.ConnectionPoints)
                .Include(c => c.Module)
                .Include(c => c.Type)
                .Where(condition);
    }

    public async Task<ConnectionPoint?> FindByIdAsync(int id, bool trackChanges = false, CancellationToken ct = default)
    {
        return await(!trackChanges 
            ? context.ConnectionPoints.AsNoTracking()
            : context.ConnectionPoints)
                .Include(c => c.Module)
                .Include(c => c.Type)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id, ct);
    }

    public IEnumerable<ConnectionPoint> GetAll()
    {
        return context.ConnectionPoints
            .Include(c => c.Module)
            .Include(c => c.Type)
            .AsNoTracking();
    }
}
