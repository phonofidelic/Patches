
using Patches.Domain.Entities;

namespace Patches.Application.Contracts;

public interface IUnitOfWork
{
    IRepository<Module> Modules { get; set; }
    IRepository<Vendor> Vendors { get; set; }
    Task SaveChangesAsync();
}
