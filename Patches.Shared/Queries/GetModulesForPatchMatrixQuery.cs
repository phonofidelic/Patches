using System;

namespace Patches.Shared.Queries;

public class GetModulesForPatchMatrixQuery
{

}

public class GetModulesForPatchMatrixQueryResult
{
    public IReadOnlyList<PatchMatrixItemDto> Modules { get; set; } = [];
}

public class PatchMatrixItemDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; } = string.Empty;
    public string VendorName { get; set; } = string.Empty;
    public List<PatchMatrixConnectionPointDto> ConnectionPoints { get; set; } = [];
}

public class PatchMatrixConnectionPointDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string  ModuleName { get; set; } = string.Empty;
    public PatchMatrixConnectionPointType Type { get; set; }
}

public enum PatchMatrixConnectionPointType
{
    Input,
    Output,
    Multiple
}