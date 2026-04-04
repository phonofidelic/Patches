using Patches.Domain.Entities;

namespace Patches.Domain.ValueObjects;

public class Connection: BaseValueObject  
{
    public ConnectionPoint Input { get; set; } = default!;
    public ConnectionPoint Output { get; set; } = default!;
    public SignalType SignalType { get; set; } = default!;
}
