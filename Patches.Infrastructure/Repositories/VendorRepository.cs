using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Patches.Application.Contracts;
using Patches.Domain.Entities;
using Patches.Infrastructure.Data;

namespace Patches.Infrastructure.Repositories;

public class VendorRepository(ApplicationDbContext context) : IRepository<Vendor, Guid>
{
    private readonly ApplicationDbContext context = context;

    public void Add(Vendor entity)
    {
        context.Vendors.Add(entity);
    }

    public void Remove(Vendor entity)
    {
        context.Vendors.Remove(entity);
    }

    public IQueryable<Vendor> FindByCondition(Expression<Func<Vendor, bool>> condition, bool trackChanges = false)
    {
        throw new NotImplementedException();
    }

    public Task<Vendor?> FindByIdAsync(
        Guid id, 
        bool trackChanges = false,
        CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Vendor> GetAll()
    {
        return context.Vendors.AsNoTracking();
    }
}
