using Patches.Domain.ValueObjects;

namespace Patches.Domain.Entities;

public class ConnectionPoint: BaseEntity<int>
{
    public string Name { get; set; } = string.Empty;
    public required ConnectionPointType Type { get; set; }
}
