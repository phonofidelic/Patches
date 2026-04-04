namespace Patches.Domain.ValueObjects;

public class ConnectionPointType(string name): BaseValueObject
{
    public string Name { get; set; } = name;
}
