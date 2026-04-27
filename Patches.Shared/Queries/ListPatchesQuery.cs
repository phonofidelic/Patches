using System;

namespace Patches.Shared.Queries;

public class ListPatchesQuery
{

}

public class ListPatchesQueryResult
{
    public IReadOnlyList<PatchListItemDto> Patches { get; set; } = [];
}

public class PatchListItemDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}