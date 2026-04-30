using System.Collections.Immutable;
using System.Data;
using AutoMapper;
using Patches.Application.Contracts;
using Patches.Shared.Dtos;
using Patches.Shared.Queries;

namespace Patches.Application.Handlers;

public class LoadPatchMatrixHandler(
    IMapper mapper,
    IUnitOfWork unitOfWork) : IHandler<LoadPatchMatrixQuery, LoadPatchMatrixResult>
{
    private readonly IMapper mapper = mapper;
    private readonly IUnitOfWork unitOfWork = unitOfWork;
    public async Task<LoadPatchMatrixResult> HandleAsync(
        LoadPatchMatrixQuery query, 
        CancellationToken ct = default)
    {
        var modules = unitOfWork.Modules.GetAll();

        await unitOfWork.SaveChangesAsync();
        
        var connectionPoints = unitOfWork.ConnectionPoints
            .FindByCondition(c => modules.Contains(c.Module))
            .AsEnumerable();

        var connections = unitOfWork.Connections
            .FindByCondition(c => c.PatchId == query.PatchId)
            .ToImmutableList();
        
        var inputs = connectionPoints.Where(c => c.Type.Name == "Input");
        var outputs = connectionPoints.Where(c => c.Type.Name == "Output");
        
        return new LoadPatchMatrixResult
        {
            Modules = [.. modules.Select(mapper.Map<PatchMatrixItemDto>)],
            Inputs = [.. inputs.Select(mapper.Map<PatchMatrixConnectionPointDto>)],
            Outputs = [.. outputs.Select(mapper.Map<PatchMatrixConnectionPointDto>)],
            Connections = [.. connections.Select(mapper.Map<ConnectionDto>)]
        };
    }
}
