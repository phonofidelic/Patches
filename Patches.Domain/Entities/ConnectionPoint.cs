namespace Patches.Domain.Entities;

public class ConnectionPoint: BaseEntity<int>
{
    public string Name { get; set; } = string.Empty;
    public Guid ModuleId { get; set; }
    public Module Module { get; set; } = default!;    
    public required ConnectionPointType Type { get; set; }
}
