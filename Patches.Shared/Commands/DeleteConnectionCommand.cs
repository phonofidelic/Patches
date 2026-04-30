namespace Patches.Shared.Commands;

public class DeleteConnectionCommand(int patchId, int inputId, int outputId)
{
    public int PatchId { get; } = patchId;
    public int InputId { get; } = inputId;
    public int OutputId { get; } = outputId;
}

public class DeleteConnectionResult
{
    public bool Success { get; set; }
}
