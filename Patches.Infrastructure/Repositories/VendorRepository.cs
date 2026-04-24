using Microsoft.EntityFrameworkCore;
using Patches.Application.Contracts;
using Patches.Domain.Entities;
using Patches.Infrastructure.Data;

namespace Patches.Infrastructure.Repositories;

public class VendorRepository(ApplicationDbContext context) : IRepository<Vendor>
{
    private readonly ApplicationDbContext context = context;

    public void Add(Vendor entity)
    {
        context.Vendors.Add(entity);
    }

    public IEnumerable<Vendor> GetAll()
    {
        return context.Vendors.AsNoTracking();
    }
}
