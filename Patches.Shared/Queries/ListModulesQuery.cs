using System;

namespace Patches.Shared.Queries;

public class ListModulesQuery
{

}

public class ListModulesQueryResult
{
    public IReadOnlyList<ModuleListItem> Modules = [];
}

public class ModuleListItem(
    Guid id,
    string name)
{
    public Guid Id { get; set; } = id;
    public string Name { get; set; } = name;
    public int HorizontalPitch { get; set; }
    public int VerticalUnits { get; set; }
    public string? Description { get; set; }
}