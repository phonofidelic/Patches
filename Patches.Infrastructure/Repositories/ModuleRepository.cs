using Microsoft.EntityFrameworkCore;
using Patches.Application.Contracts;
using Patches.Domain.Entities;
using Patches.Infrastructure.Data;

namespace Patches.Infrastructure.Repositories;

public class ModuleRepository(ApplicationDbContext context) : IRepository<Module>
{
    private readonly ApplicationDbContext context = context;
    public void Add(Module entity)
    {
        context.Modules.Add(entity);
    }

    public IEnumerable<Module> GetAll()
    {
        return context.Modules.AsNoTracking();
    }
}
