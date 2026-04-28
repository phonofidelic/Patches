namespace Patches.Domain.Entities;

public class ConnectionPointType(string name): BaseEntity<int>
{
    public string Name { get; set; } = name;
}
