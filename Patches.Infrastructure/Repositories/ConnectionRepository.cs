using System;
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

    public async Task<Connection?> FindByIdAsync(int id, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Connection> GetAll()
    {
        throw new NotImplementedException();
    }
}
