using Patches.Shared.Dtos;

namespace Patches.Shared.Commands;

public class AddConnectionCommand(
    ConnectionPointDto input, 
    ConnectionPointDto output,
    int patchId)
{
    public int PatchId { get; set; } = patchId;
    public ConnectionPointDto Input { get; set; } = input;
    public ConnectionPointDto Output { get; set; } = output;
}

public class AddConnectionResult
{
    
}