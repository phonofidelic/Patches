using System;
using Microsoft.EntityFrameworkCore;
using Patches.Application.Contracts;
using Patches.Domain.Entities;
using Patches.Infrastructure.Data;

namespace Patches.Infrastructure.Repositories;

public class PatchRepository(ApplicationDbContext context) : IRepository<Patch, int>
{
    private readonly ApplicationDbContext context = context;
    public void Add(Patch entity)
    {
        context.Patches.Add(entity);
    }

    public async Task<Patch?> FindByIdAsync(int id, CancellationToken ct = default)
    {
        return await context.Patches
            .Include(p => p.Modules)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id, ct);
    }

    public IEnumerable<Patch> GetAll()
    {
        return context.Patches.AsNoTracking();
    }
}
