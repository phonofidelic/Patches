
using Patches.Domain.Entities;

namespace Patches.Application.Contracts;

public interface IUnitOfWork
{
    IRepository<Module> Modules { get; set; }
    Task SaveChangesAsync();
}
