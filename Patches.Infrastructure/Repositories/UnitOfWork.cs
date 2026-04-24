using Patches.Application.Contracts;
using Patches.Domain.Entities;
using Patches.Infrastructure.Data;

namespace Patches.Infrastructure.Repositories;

public class UnitOfWork(
    IRepository<Module> moduleRepository,
    IRepository<Vendor> vendorRepository,
    ApplicationDbContext context) : IUnitOfWork
{
    private readonly ApplicationDbContext context = context;
    public IRepository<Module> Modules { get; set; } = moduleRepository;
    public IRepository<Vendor> Vendors { get; set; } = vendorRepository;

    public async Task SaveChangesAsync()
    {
        await context.SaveChangesAsync();
    }
}
