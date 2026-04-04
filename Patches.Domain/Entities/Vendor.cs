namespace Patches.Domain.Entities;

public class Vendor: BaseEntity<Guid>
{
    public required string Name { get; set; }
    public ICollection<Module> Modules { get; set; } = [];
}
