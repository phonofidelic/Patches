using Patches.Application.Contracts;
using Patches.Domain.Entities;
using Patches.Infrastructure.Data;

namespace Patches.Infrastructure.Repositories;

public class UnitOfWork(
    IRepository<Module> moduleRepository,
    ApplicationDbContext context) : IUnitOfWork
{
    private readonly ApplicationDbContext context = context;
    public IRepository<Module> Modules { get; set; } = moduleRepository;

    public async Task SaveChangesAsync()
    {
        await context.SaveChangesAsync();
    }
}
