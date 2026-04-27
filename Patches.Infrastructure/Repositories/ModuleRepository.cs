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

    public Task<Module?> FindByConditionAsync(Expression<Func<Module, bool>> condition, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public async Task<Module?> FindByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await context.Modules
            .Include(m => m.ConnectionPoints)
            .Include(m => m.Vendor)
            .FirstOrDefaultAsync(m => m.Id == id, ct);
    }

    public IEnumerable<Module> GetAll()
    {
        return context.Modules.AsNoTracking();
    }
}
