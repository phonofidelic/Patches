using Patches.Shared.Dtos;

namespace Patches.Shared.Queries;

public class ListPatchesQuery
{

}

public class ListPatchesQueryResult
{
    public IReadOnlyList<PatchListItemDto> Patches { get; set; } = [];
}
