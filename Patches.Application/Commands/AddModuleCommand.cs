using System;

namespace Patches.Application.Commands;

public class AddModuleCommand
{
    public string Name { get; set; } = string.Empty;
    public int HorizontalPitch { get; set; }
    public int VerticalUnits { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? Vendor { get; set; }

    public AddModuleCommand()
    {
        
    }
    public AddModuleCommand(
        string name,
        int hp,
        int u,
        string? description,
        string? vendor
    )
    {
        Name = name;
        HorizontalPitch = hp;
        VerticalUnits = u;
        Description = description ?? string.Empty;
        Vendor = vendor;
    }
}

public class AddModuleResult(
    Guid id,
    string name,
    int hp,
    int u,
    IReadOnlyList<ModuleConnectionPoint> connectionPoints
)
{
    public Guid Id { get; set; } = id;
    public string Name { get; set; } = name;
    public int HorizontalPitch { get; set; } = hp;
    public int VerticalUnits { get; set; } = u;
    public IReadOnlyList<ModuleConnectionPoint> ConnectionPoints { get; set; } = connectionPoints;
}

public class ModuleConnectionPoint
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}