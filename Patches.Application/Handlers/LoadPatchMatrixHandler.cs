using System.Collections.Immutable;
using AutoMapper;
using Patches.Application.Contracts;
using Patches.Shared.Queries;

namespace Patches.Application.Handlers;

public class LoadPatchMatrixHandler(
    IMapper mapper,
    IUnitOfWork unitOfWork) : IHandler<LoadPatchMatrixQuery, LoadPatchMatrixResult>
{
    private readonly IMapper mapper = mapper;
    private readonly IUnitOfWork unitOfWork = unitOfWork;
    public async Task<LoadPatchMatrixResult> HandleAsync(
        LoadPatchMatrixQuery request, 
        CancellationToken ct = default)
    {
        var modules = unitOfWork.Modules.GetAll();

        await unitOfWork.SaveChangesAsync();
        
        var connectionPoints = unitOfWork.ConnectionPoints
            .FindByCondition(c => modules.Contains(c.Module))
            .AsEnumerable();
        
        var inputs = connectionPoints.Where(c => c.Type.Name == "Input");
        var outputs = connectionPoints.Where(c => c.Type.Name == "Output");
        
        var connectionPointsDto = connectionPoints
            .Select(c => 
            {
                var dto = mapper.Map<PatchMatrixConnectionPointDto>(c);
                dto.Type = mapper.Map<PatchMatrixConnectionPointType>(c.Type);
                return dto;
            });
        
        return new LoadPatchMatrixResult
        {
            Modules = [.. modules.Select(mapper.Map<PatchMatrixItemDto>)],
            ConnectionPoints = [.. connectionPointsDto],
            Inputs = [.. inputs.Select(mapper.Map<PatchMatrixConnectionPointDto>)],
            Outputs = [.. outputs.Select(mapper.Map<PatchMatrixConnectionPointDto>)]
        };
    }
}
