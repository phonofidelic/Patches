using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Patches.Application.Contracts;
using Patches.Domain.Entities;
using Patches.Infrastructure.Data;

namespace Patches.Infrastructure.Repositories;

public class ModuleRepository(ApplicationDbContext context) : IRepository<Module, Guid>
{
    private readonly ApplicationDbContext context = context;
    public void Add(Module entity)
    {
        context.Modules.Add(entity);
    }

    public void Remove(Module entity)
    {
        context.Modules.Remove(entity);
    }

    public IQueryable<Module> FindByCondition(
        Expression<Func<Module, bool>> condition, 
        bool trackChanges = false)
    {
        throw new NotImplementedException();
    }

    public async Task<Module?> FindByIdAsync(
        Guid id, 
        bool trackChanges = false, 
        CancellationToken ct = default)
    {
        return await (!trackChanges 
            ? context.Modules.AsNoTracking()
            : context.Modules)
                .Include(m => m.ConnectionPoints)
                .Include(m => m.Vendor)
                .FirstOrDefaultAsync(m => m.Id == id, ct);
    }

    public IEnumerable<Module> GetAll()
    {
        return context.Modules.AsNoTracking();
    }
}
