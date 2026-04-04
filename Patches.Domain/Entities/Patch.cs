using Patches.Domain.ValueObjects;

namespace Patches.Domain.Entities;

public class Patch: BaseEntity<int>
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ICollection<Module> Modules { get; set; } = [];
    public ICollection<Connection> Connections { get; set; } = [];
}
