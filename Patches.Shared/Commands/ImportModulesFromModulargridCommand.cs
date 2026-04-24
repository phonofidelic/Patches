namespace Patches.Shared.Commands;

public class ImportModulesFromModulargridCommand
{
    public string EndpointUrl { get; set; } = string.Empty;
}

public class ImportModulesFromModulargridResult
{
    public int ImportedCount { get; set; }
    public int SkippedCount { get; set; }
}
