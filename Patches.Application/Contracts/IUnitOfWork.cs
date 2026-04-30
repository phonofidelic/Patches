
using Patches.Domain.Entities;

namespace Patches.Application.Contracts;

public interface IUnitOfWork
{
    IRepository<Module, Guid> Modules { get; }
    IRepository<Vendor, Guid> Vendors { get; }
    IRepository<ConnectionPoint, int> ConnectionPoints { get; }
    IRepository<Connection, int> Connections { get; }
    IRepository<Patch, int> Patches { get; }
    Task SaveChangesAsync();
}
