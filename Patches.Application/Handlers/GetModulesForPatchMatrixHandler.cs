using System;
using AutoMapper;
using Patches.Application.Contracts;
using Patches.Shared.Queries;

namespace Patches.Application.Handlers;

public class GetModulesForPatchMatrixHandler(
    IMapper mapper,
    IUnitOfWork unitOfWork) : IHandler<GetModulesForPatchMatrixQuery, GetModulesForPatchMatrixQueryResult>
{
    private readonly IMapper mapper = mapper;
    private readonly IUnitOfWork unitOfWork = unitOfWork;
    public async Task<GetModulesForPatchMatrixQueryResult> HandleAsync(GetModulesForPatchMatrixQuery request)
    {
        var modules = unitOfWork.Modules.GetAll()
            .Select(mapper.Map<PatchMatrixItemDto>)
            .ToList();
        
        return new GetModulesForPatchMatrixQueryResult
        {
            Modules = modules
        };
    }
}
