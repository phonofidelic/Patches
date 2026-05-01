using Patches.Application.Contracts;
using Patches.Domain.Entities;
using Patches.Infrastructure.Data;

namespace Patches.Infrastructure.Repositories;

public class UnitOfWork(
    IRepository<Module, Guid> moduleRepository,
    IRepository<Vendor, Guid> vendorRepository,
    IRepository<ConnectionPoint, int> connectionPointRepository,
    IConnectionRepository connectionRepository,
    IRepository<Patch, int> patchRepository,
    ApplicationDbContext context) : IUnitOfWork
{
    private readonly ApplicationDbContext context = context;

    public IRepository<Module, Guid> Modules { get; } = moduleRepository;

    public IRepository<Vendor, Guid> Vendors { get; } = vendorRepository;

    public IRepository<ConnectionPoint, int> ConnectionPoints { get; } = connectionPointRepository;

    public IConnectionRepository Connections { get; } = connectionRepository;

    public IRepository<Patch, int> Patches { get; } = patchRepository;

    public async Task SaveChangesAsync()
    {
        await context.SaveChangesAsync();
    }
}
