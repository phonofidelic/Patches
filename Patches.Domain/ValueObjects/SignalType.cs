namespace Patches.Domain.ValueObjects;

public class SignalType(string name): BaseValueObject
{
    public string Name { get; set; } = name;
}
