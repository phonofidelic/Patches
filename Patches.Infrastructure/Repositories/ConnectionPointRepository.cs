using System;
using System.IO.Compression;
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

    public async Task<ConnectionPoint?> FindByIdAsync(int id, CancellationToken ct = default)
    {
        return await context.ConnectionPoints
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id, ct);
    }

    public IEnumerable<ConnectionPoint> GetAll()
    {
        return context.ConnectionPoints
            .Include(c => c.Module)
            .AsNoTracking();
    }
}
