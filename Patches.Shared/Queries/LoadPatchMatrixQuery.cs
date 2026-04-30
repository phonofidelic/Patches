using Patches.Shared.Dtos;

namespace Patches.Shared.Queries;

public class LoadPatchMatrixQuery
{
    public int? PatchId { get; set; }

    public LoadPatchMatrixQuery()
    {
        
    }
    public LoadPatchMatrixQuery(int patchId)
    {
        PatchId = patchId;
    }
}

public class LoadPatchMatrixResult
{
    public IReadOnlyList<PatchMatrixItemDto> Modules { get; set; } = [];
    public IReadOnlyList<PatchMatrixConnectionPointDto> Inputs { get; set; } = [];
    public IReadOnlyList<PatchMatrixConnectionPointDto> Outputs { get; set; } = [];
    public IReadOnlyList<ConnectionDto> Connections { get; set; } = [];
}

public class PatchMatrixItemDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; } = string.Empty;
    public string? VendorName { get; set; } = string.Empty;
    public List<PatchMatrixConnectionPointDto> ConnectionPoints { get; set; } = [];
}

public class PatchMatrixConnectionPointDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string  ModuleName { get; set; } = string.Empty;
    public Guid ModuleId { get; set; }
    public PatchMatrixConnectionPointType Type { get; set; } = default!;
}

public class PatchMatrixConnectionPointType
{
    // Multiple,
    public string Name { get; set; } = string.Empty;
}