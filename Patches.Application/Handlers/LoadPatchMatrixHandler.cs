using System;
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
        var modules = unitOfWork.Modules.GetAll()
            .Select(mapper.Map<PatchMatrixItemDto>)
            .ToList();
        
        return new LoadPatchMatrixResult
        {
            Modules = modules
        };
    }
}
