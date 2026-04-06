namespace Patches.Domain.Entities;

public class PatchMatrix: BaseEntity<Guid>
{
    public ICollection<Module> Modules { get; set; } = [];
    public ICollection<Patch> Patches { get; set; } = [];
}
